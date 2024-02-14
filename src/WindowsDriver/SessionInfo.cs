using System.Diagnostics;

namespace WindowsDriver
{
    public class SessionInfo : IDisposable
    {
        public readonly string url;
        public readonly Process p;
        public readonly HttpClient httpClient;

        public SessionInfo(string url, Process p, int commandTimeout)
        {
            this.url = url;
            this.p = p;
            this.httpClient = new HttpClient();
            this.httpClient.Timeout = TimeSpan.FromMilliseconds(commandTimeout);
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
            this.p.Dispose();
        }
    }
}
