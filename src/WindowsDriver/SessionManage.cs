using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text;

namespace WindowsDriver
{
    public class SessionManage
    {
        private static SessionManage? instance;
        private static readonly int DEFAULT_COMMAND_TIMEOUT = 100000;
        private SessionManage() 
        {
            this.sessionUrl = [];
        }

        public static SessionManage Instance()
        {
            if (instance == null) instance = new SessionManage();
            return instance;
        }

        private Dictionary<string, SessionInfo> sessionUrl;

        public async Task StartSession(HttpContext context)
        {
            var driverOptions = await new CapabilitiesValidator().ValidateCap(context);
            if (context.Response.StatusCode != (int)HttpStatusCode.OK) return;

            string currentExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string parentFolder = Directory.GetParent(currentExePath).FullName;
            string path = Path.Join(parentFolder, "UIADriver.exe");

            #if DEBUG
            string currentFolder = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            path = Path.Join(currentFolder, "UIADriver", "bin", "x64", "Release", "net8.0-windows", "UIADriver.exe");
            #endif

            Debug.WriteLine(currentExePath);
            if (driverOptions != null)
            {
                driverOptions.TryGetPropertyValue("windriver:commandTimeout", out var commandTimeoutJson);
                var commandTimeout = commandTimeoutJson != null ? (int)commandTimeoutJson.AsValue() : DEFAULT_COMMAND_TIMEOUT;
                driverOptions.TryGetPropertyValue("windriver:automationName", out var automationName);
                if (automationName != null)
                {
                    //  Start driver process
                    string sessionId = Guid.NewGuid().ToString();
                    int port = GetFreePort();
                    string url = "http://localhost:" + port;
                    var p = new Process();
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                    p.StartInfo.FileName = Path.GetFileName(path);
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.StartInfo.ArgumentList.Add("--urls");
                    p.StartInfo.ArgumentList.Add(url);
                    p.Start();

                    //  Wait until server ready
                    using (var httpClient = GetHttpClient(30000))
                    {
                        var start = DateTime.Now;
                        while (true)
                        {
                            if (DateTime.Now - start > TimeSpan.FromSeconds(30))
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                var err = new Error("session not created", "Timeout waiting for driver server to start");
                                await context.Response.WriteAsJsonAsync(new Response(err));
                                return;
                            }

                            try
                            {
                                var response = await httpClient.GetAsync(url);
                                if (response.StatusCode == HttpStatusCode.OK) break;
                                Thread.Sleep(200);
                            }
                            catch { }
                        }
                    }

                    //  Send cap to init session
                    using (var httpClient = GetHttpClient(commandTimeout)) 
                    {
                        string newSessionUrl = url + "/session";
                        var content = new StringContent(driverOptions.ToJsonString(), Encoding.UTF8, "application/json");
                        try
                        {
                            var response = await httpClient.PostAsync(newSessionUrl, content);
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                context.Response.StatusCode = (int)response.StatusCode;
                                p.Kill();

                                string bodyContent = await response.Content.ReadAsStringAsync();
                                await context.Response.WriteAsJsonAsync(JsonNode.Parse(bodyContent));
                                return;
                            }
                        } catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                        {
                            p.Kill();

                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            var err = new Error("session not created", "Command timeout");
                            await context.Response.WriteAsJsonAsync(new Response(err));
                            return;
                        } catch (Exception ex)
                        {
                            p.Kill();

                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            var err = new Error("session not created", ex.Message);
                            await context.Response.WriteAsJsonAsync(new Response(err));
                            return;
                        }
                    }

                    //  Response
                    sessionUrl.Add(sessionId, new SessionInfo(url, p, commandTimeout));
                    var sessionRsp = new { sessionId, capabilities = driverOptions };
                    await context.Response.WriteAsJsonAsync(new Response(sessionRsp));
                }
            } else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var err = new Error("session not created", "Session cannot be created, no suitable capabilities");
                await context.Response.WriteAsJsonAsync(new Response(err));
            }
        }

        public async Task CloseSession(string sessionId, HttpContext context)
        {
            await CloseSession(sessionId);
            sessionUrl.Remove(sessionId, out var removed);
            if (removed != null) removed.Dispose();
            await context.Response.WriteAsJsonAsync(new Response(null));
        }

        private async Task CloseSession(string sessionId)
        {
            bool hasSession = sessionUrl.TryGetValue(sessionId, out var sessionInfo);
            if (hasSession && sessionInfo != null)
            {
                //  Tell driver to shutdown
                string url = sessionInfo.url + "/session/" + sessionId;
                var httpClient = sessionInfo.httpClient;
                try
                {
                    await httpClient.DeleteAsync(url);
                } catch { }

                //  Wait for shutdown until timeout
                bool hasExited = sessionInfo.p.WaitForExit(5000);
                if (!hasExited)
                {
                    sessionInfo.p.Kill();
                }
            }
        }

        public SessionInfo? FindSessionInfo(string sessionId)
        {
            sessionUrl.TryGetValue(sessionId, out var sessionInfo);
            if (sessionInfo == null || sessionInfo.p.HasExited) return null;
            return sessionInfo;
        }

        private int GetFreePort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        public Task CleanUp()
        {
            var cleanAll = new List<Task>();
            foreach (var item in sessionUrl)
            {
                cleanAll.Add(CloseSession(item.Key));
            }
            return Task.WhenAll(cleanAll);
        }

        private HttpClient GetHttpClient(int timeout)
        {
            var handler = new HttpClientHandler()
            {
                UseProxy = false,
            };
            var httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromMilliseconds(timeout);
            return httpClient;
        }

    }
    
}
