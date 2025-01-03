using Microsoft.AspNetCore.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using UIADriver;
using UIADriver.actions;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.win32native;
using ISession = UIADriver.ISession;

Win32Methods.SetProcessDpiAwareness(Win32Enum.PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

ISession? session = null;

Func<ISession> getSession = () =>
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

//  Base W3C API

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
        if (cap.windowNameRegex != null)
        {
            session = new UIADriver.uia2.session.InjectByWindowNameSession(cap);
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

            //var capjson = new JsonObject();
            //capjson["platformName"] = "windows";
            //capjson["windriver:automationName"] = "uia3";
            //capjson["windriver:aumid"] = "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App";
            //var sessionRsp = new { sessionId = "debug", capabilities = capjson };
            //await context.Response.WriteAsJsonAsync(new Response(sessionRsp));
            return;
        }
        if (cap.nativeWindowHandle != null)
        {
            session = new UIADriver.uia3.session.InjectWindowSession(cap);
            return;
        }
        if (cap.windowNameRegex != null)
        {
            session = new UIADriver.uia3.session.InjectByWindowNameSession(cap);
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

//  Element Pattern API

app.MapGet("/session/{id}/annotationPattern/{elementId}/target", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetAnnotationTarget(elementId)));
});

app.MapGet("/session/{id}/basePattern/{elementId}/labeledBy", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetLabeledBy(elementId)));
});

app.MapGet("/session/{id}/basePattern/{elementId}/controllerFor", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetControllerFor(elementId)));
});

app.MapGet("/session/{id}/basePattern/{elementId}/describedBy", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetDescribedBy(elementId)));
});

app.MapGet("/session/{id}/basePattern/{elementId}/flowsTo", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetFlowsTo(elementId)));
});

app.MapPost("/session/{id}/basePattern/{elementId}/focus", async (HttpContext context, string elementId) =>
{
    getSession().SetFocus(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/customNavigationPattern/{elementId}/navigate", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    await context.Response.WriteAsJsonAsync(new Response(getSession().NavigateFollowDirection(elementId, data)));
});

app.MapPost("/session/{id}/dockPattern/{elementId}/position", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().SetDockPosition(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapGet("/session/{id}/dragPattern/{elementId}/dropEffects", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetDropEffects(elementId)));
});

app.MapGet("/session/{id}/dragPattern/{elementId}/grabbedItems", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetGrabbedItems(elementId)));
});

app.MapGet("/session/{id}/dropTargetPattern/{elementId}/dropTargetEffects", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetDropTargetEffects(elementId)));
});

app.MapPost("/session/{id}/expandCollapsePattern/{elementId}/expandCollapse", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().ExpandOrCollapseElement(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapGet("/session/{id}/gridItemPattern/{elementId}/containingGrid", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetContainingGrid(elementId)));
});

app.MapPost("/session/{id}/gridPattern/{elementId}/item", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetGridItem(elementId, data)));
});

app.MapPost("/session/{id}/invokePattern/{elementId}/invoke", async (HttpContext context, string elementId) =>
{
    getSession().Invoke(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapGet("/session/{id}/multipleViewPattern/{elementId}/supportedViews", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetSupportedViewIds(elementId)));
});

app.MapGet("/session/{id}/multipleViewPattern/{elementId}/viewName/{viewId}", async (HttpContext context, string elementId, string viewId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetViewName(elementId, viewId)));
});

app.MapPost("/session/{id}/multipleViewPattern/{elementId}/currentView", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().SetCurrentView(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/rangeValuePattern/{elementId}/value", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().SetRangeValue(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/scrollItemPattern/{elementId}/scrollIntoView", async (HttpContext context, string elementId) =>
{
    getSession().ScrollIntoView(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/scrollPattern/{elementId}/scroll", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().Scroll(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/scrollPattern/{elementId}/scrollPercent", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().SetScrollPercent(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/selectionItemPattern/{elementId}/select", async (HttpContext context, string elementId) =>
{
    getSession().Select(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/selectionItemPattern/{elementId}/selection", async (HttpContext context, string elementId) =>
{
    getSession().AddToSelection(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapDelete("/session/{id}/selectionItemPattern/{elementId}/selection", async (HttpContext context, string elementId) =>
{
    getSession().RemoveFromSelection(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapGet("/session/{id}/selectionItemPattern/{elementId}/selectionContainer", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetSelectionContainer(elementId)));
});

app.MapGet("/session/{id}/selectionPattern2/{elementId}/firstSelected", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetFirstSelectedItem(elementId)));
});

app.MapGet("/session/{id}/selectionPattern2/{elementId}/lastSelected", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetLastSelectedItem(elementId)));
});

app.MapGet("/session/{id}/selectionPattern2/{elementId}/currentSelected", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetCurrentSelectedItem(elementId)));
});

app.MapGet("/session/{id}/spreadSheetItemPattern/{elementId}/annotationObjects", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetAnnotationObjects(elementId)));
});

app.MapGet("/session/{id}/spreadSheetPattern/{elementId}/item/{name}", async (HttpContext context, string elementId, string name) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetSpreadSheetItemByName(elementId, name)));
});

app.MapGet("/session/{id}/tableItemPattern/{elementId}/rowHeaderItems", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetRowHeaderItems(elementId)));
});

app.MapGet("/session/{id}/tableItemPattern/{elementId}/columnHeaderItems", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetColumnHeaderItems(elementId)));
});

app.MapGet("/session/{id}/tablePattern/{elementId}/rowHeaders", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetRowHeaderItems(elementId)));
});

app.MapGet("/session/{id}/tablePattern/{elementId}/columnHeaders", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetColumnHeaderItems(elementId)));
});

app.MapPost("/session/{id}/togglePattern/{elementId}/toggle", async (HttpContext context, string elementId) =>
{
    getSession().Toggle(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/transformPattern2/{elementId}/zoom", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().Zoom(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/transformPattern2/{elementId}/zoomByUnit", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().ZoomByUnit(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/transformPattern/{elementId}/move", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().Move(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/transformPattern/{elementId}/resize", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().Resize(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/transformPattern/{elementId}/rotate", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().Rotate(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/valuePattern/{elementId}/value", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().SetValue(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/virtualizedItemPattern/{elementId}/realize", async (HttpContext context, string elementId) =>
{
    getSession().Realize(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/windowPattern/{elementId}/visualState", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().SetVisualState(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/windowPattern/{elementId}/waitForInputIdle", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().WaitForInputIdle(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapDelete("/session/{id}/windowPattern/{elementId}", async (HttpContext context, string elementId) =>
{
    getSession().CloseWindow(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapGet("/session/{id}/windowPattern/{elementId}/rect", async (HttpContext context, string elementId) =>
{
    await context.Response.WriteAsJsonAsync(new Response(getSession().GetWindowRect(elementId)));
});

app.MapPost("/session/{id}/windowPattern/{elementId}/bringToTop", async (HttpContext context, string elementId) =>
{
    getSession().BringWindowToTop(elementId);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.MapPost("/session/{id}/windowPattern/{elementId}/rect", async (HttpContext context, string elementId) =>
{
    var data = await parseBody(context);
    getSession().SetWindowRect(elementId, data);
    await context.Response.WriteAsJsonAsync(new Response(null));
});

app.Run();
