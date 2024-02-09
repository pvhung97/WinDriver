using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using UIADriver;
using UIADriver.actions;
using UIADriver.dto.response;
using UIADriver.exception;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Session? session = null;

Func<Session> getSession = () =>
{
    if (session == null) throw new SessionNotStartException("Session has been closed or already died");
    return session;
};

Func<HttpContext, Task<JsonObject>> parseBody = async (context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    try
    {
        var result = JsonSerializer.Deserialize<JsonObject>(body);
        if (result != null) return result;
    }
    catch { }
    throw new InvalidArgument("Invalid request body");
};

app.UseExceptionHandler(e =>
{
    e.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature != null)
        {
            if (exceptionHandlerPathFeature.Error is WebDriverException ex)
            {
                context.Response.StatusCode = (int) ex.GetStatusCode();
                await context.Response.WriteAsJsonAsync(new Response(ex.GetError()));
            } else
            {
                string stacktrace = exceptionHandlerPathFeature.Error.StackTrace != null ? exceptionHandlerPathFeature.Error.StackTrace : string.Empty;
                var err = new Error("unknown error", exceptionHandlerPathFeature.Error.Message, stacktrace);
                await context.Response.WriteAsJsonAsync(new Response(err));
            }
        }
    });

    app.UseHsts();
});

app.Use(async (context, next) =>
{
    string path = context.Request.Path;
    string method = context.Request.Method;

    await next();

    if (context.Response.StatusCode == ((int)HttpStatusCode.NotFound))
    {
        try
        {
            using var reader = new StreamReader(context.Response.Body);
            string bodyContent = await reader.ReadToEndAsync();
            var respErro = JsonSerializer.Deserialize<Response>(bodyContent);
        }
        catch
        {
            var err = new Error("unknown command", "unknown command: " + method + " " + context.Request.Path);
            await context.Response.WriteAsJsonAsync(new Response(err));
        }
    }
});

app.MapGet("/", async ctx => 
{
    await ctx.Response.WriteAsync("ready");
});

app.MapPost("/session", async context =>
{
    if (session != null)
    {
        await context.Response.WriteAsJsonAsync(new Response(new Error("unknown error", "Session has already started")));
        return;
    }

    var capabilities = await parseBody(context);
    SessionCapabilities cap = SessionCapabilities.ParseCapabilities(capabilities);

    if (string.Equals("uia2", cap.automationName, StringComparison.OrdinalIgnoreCase))
    {
        if (cap.appPath != null)
        {
            session = new UIADriver.uia2.session.AppPathSession(cap);
            return;
        }
        if (cap.aumid != null)
        {
            session = new UIADriver.uia2.session.WindowsStoreAppSession(cap);
            return;
        }
        if (cap.nativeWindowHandle != null)
        {
            session = new UIADriver.uia2.session.InjectWindowSession(cap);
            return;
        }
        session = new UIADriver.uia2.session.RootSession(cap);
    } else if (string.Equals("uia3", cap.automationName, StringComparison.OrdinalIgnoreCase))
    {
        if (cap.appPath != null)
        {
            session = new UIADriver.uia3.session.AppPathSession(cap);
            return;
        }
        if (cap.aumid != null)
        {
            session = new UIADriver.uia3.session.WindowsStoreAppSession(cap);
            return;
        }
        if (cap.nativeWindowHandle != null)
        {
            session = new UIADriver.uia3.session.InjectWindowSession(cap);
            return;
        }
        session = new UIADriver.uia3.session.RootSession(cap);
    } else
    {
        await context.Response.WriteAsJsonAsync(new Response(new Error("unknown error", "Invalid automation name")));
    }

});

app.MapDelete("/session/{id}", async context =>
{
    if (session != null)
    {
        await session.CloseSession();
        InputState.Instance().Dispose();
    }
    _ = app.StopAsync();
});

app.MapGet("/session/{id}/title", async context =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetCurrentWindowTitle()));
});

app.MapGet("/session/{id}/window", async context =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetCurrentWindowHdl()));
});

app.MapDelete("/session/{id}/window", async context =>
{
    var afterClosed = getSession().CloseCurrentWindow();
    await context.Response.WriteAsJsonAsync(new Response(afterClosed));
    if (afterClosed.Count == 0)
    {
        _ = app.StopAsync();
    }
});

app.MapPost("/session/{id}/window", async context =>
{
    var data = await parseBody(context);
    getSession().SwitchToWindow(data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapGet("/session/{id}/window/handles", async context =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().CollectWindowHandles()));
});

app.MapGet("/session/{id}/window/rect", async context =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetCurrentWindowRect()));
});

app.MapPost("/session/{id}/window/rect", async context =>
{
    var data = await parseBody(context);
    await context.Response.WriteAsJsonAsync(new Response(getSession().SetWindowRect(data)));
});

app.MapPost("/session/{id}/window/minimize", async context =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().MinimizeCurrentWindow()));
});

app.MapPost("/session/{id}/window/maximize", async context =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().MaximizeCurrentWindow()));
});

app.MapGet("/session/{id}/screenshot", async context =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetScreenshot()));
});

app.MapGet("/session/{id}/source", async context =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetPageSource()));
});

app.MapPost("/session/{id}/element", async context =>
{
    var data = await parseBody(context);
    await context.Response.WriteAsJsonAsync(new Response(getSession().FindElement(data)));
});

app.MapPost("/session/{id}/elements", async context =>
{
    var data = await parseBody(context);
    await context.Response.WriteAsJsonAsync(new Response(getSession().FindElements(data)));
});

app.MapPost("/session/{id}/element/{elementId}/element", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    await context.Response.WriteAsJsonAsync(new Response(getSession().FindElementFromElement(elementId, data)));
});

app.MapPost("/session/{id}/element/{elementId}/elements", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    await context.Response.WriteAsJsonAsync(new Response(getSession().FindElementsFromElement(elementId, data)));
});

app.MapGet("/session/{id}/element/active", async context =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetActiveElement()));
});

app.MapGet("/session/{id}/element/{elementId}/attribute/{attributeName}", async (HttpContext context, string elementId, string attributeName) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetElementAttribute(elementId, attributeName)));
});

app.MapGet("/session/{id}/element/{elementId}/name", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetElementTagName(elementId)));
});

app.MapGet("/session/{id}/element/{elementId}/rect", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetElementRect(elementId)));
});

app.MapGet("/session/{id}/element/{elementId}/text", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetElementText(elementId)));
});

app.MapGet("/session/{id}/element/{elementId}/selected", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().IsElementSelected(elementId)));
});

app.MapGet("/session/{id}/element/{elementId}/enabled", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().IsElementEnabled(elementId)));
});

app.MapGet("/session/{id}/element/{elementId}/displayed", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().IsElementDisplayed(elementId)));
});

app.MapPost("/session/{id}/actions", async context =>
{
    var data = await parseBody(context);
    await getSession().PerformActions(data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapDelete("/session/{id}/actions", async context =>
{
    await getSession().ReleaseActions();
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/element/{elementId}/click", async (HttpContext context, string elementId) =>
{
    await getSession().ElementClick(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/element/{elementId}/clear", async (HttpContext context, string elementId) =>
{
    getSession().ElementClear(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/element/{elementId}/value", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    await getSession().ElementSendKeys(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapGet("/session/{id}/element/{elementId}/screenshot", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetElementScreenshot(elementId)));
});

app.Run();
