using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using WindowsDriver;

Console.WriteLine($"WinDriver Version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
if (args.Contains("-version")) return;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    string path = context.Request.Path;
    string method = context.Request.Method;

    bool needRedirect = false;
    string sessionId = "";
    if (path.StartsWith("/session/"))
    {
        string[] splt = path.Split("/");
        if (splt.Length > 3)
        {
            needRedirect = true;
            sessionId = splt[2];
        }
    }

    if (needRedirect)
    {
        var sessionManage = SessionManage.Instance();
        var sessionInfo = sessionManage.FindSessionInfo(sessionId);
        if (sessionInfo == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            Error err = new Error("invalid session id", "Session has been closed or already died");
            await context.Response.WriteAsJsonAsync(new Response(err));
            return;
            
        }
        string redirectBase = sessionInfo.url;
        string newUrl = redirectBase + path;
        var httpClient = sessionInfo.httpClient;
        var message = new HttpRequestMessage(HttpMethod.Parse(context.Request.Method), newUrl);
        using (StreamReader reader = new StreamReader(context.Request.Body))
        {
            string bodyContent = await reader.ReadToEndAsync();
            if (bodyContent != null && bodyContent.Length > 0)
            {
                var body = new StringContent(bodyContent, Encoding.UTF8, "application/json");
                message.Content = body;
            }
        }

        var response = await httpClient.SendAsync(message);
        var result = await response.Content.ReadAsStringAsync();
        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsJsonAsync(JsonNode.Parse(result));

        return;
    } else await next();

    if (context.Response.StatusCode == ((int)HttpStatusCode.NotFound))
    {
        try
        {
            using (StreamReader reader = new StreamReader(context.Response.Body))
            {
                string bodyContent = await reader.ReadToEndAsync();
                var respErro = JsonSerializer.Deserialize<Response>(bodyContent);
            }
        } catch
        {
            Error err = new Error("unknown command", "unknown command: " + method + " " + context.Request.Path);
            await context.Response.WriteAsJsonAsync(new Response(err));
        }
    }
});

app.MapGet("/status", async context =>
{
    await context.Response.WriteAsJsonAsync(new Response(new Status()));
});

app.MapPost("/session", async context =>
{
    await SessionManage.Instance().StartSession(context);

});

app.MapDelete("/session/{id}", async (string id, HttpContext context) =>
{
    await SessionManage.Instance().CloseSession(id, context);
});

app.MapGet("/quitquitquit", async () =>
{
    await SessionManage.Instance().CleanUp();
    _ = app.StopAsync();
});

app.Run();
