using System.Text.Json.Nodes;
using System.Text.Json;
using UIADriver.actions;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using System.Windows.Automation;

namespace UIADriver.uia2.session
{
    public abstract class UIA2Session : Session<AutomationElement, CacheRequest>
    {
        public UIA2Session(SessionCapabilities capabilities) : base(capabilities) { }

        protected abstract ActionOptions GetActionOption();

        public override HashSet<string> CollectWindowHandles()
        {
            return GetServiceProvider().GetWindowManageService().CollectWindowHandles().ToHashSet();
        }

        public override RectResponse MinimizeCurrentWindow()
        {
            return GetServiceProvider().GetWindowManageService().MinimizeCurrentWindow();
        }

        public override RectResponse MaximizeCurrentWindow()
        {
            return GetServiceProvider().GetWindowManageService().MaximizeCurrentWindow();
        }

        public override RectResponse SetWindowRect(JsonObject data)
        {
            var rq = SetRectRequest.Validate(data);
            return GetServiceProvider().GetWindowManageService().SetWindowRect(rq);
        }

        public override HashSet<string> CloseCurrentWindow()
        {
            return GetServiceProvider().GetWindowManageService().CloseCurrentWindow().ToHashSet();
        }

        public override void SwitchToWindow(JsonObject windowHandle)
        {
            var rq = SwitchWindowRequest.Validate(windowHandle);
            GetServiceProvider().GetWindowManageService().SwitchToWindow(rq);
        }

        public override string GetCurrentWindowTitle()
        {
            return GetServiceProvider().GetWindowManageService().GetCurrentWindowTitle();
        }

        public override string GetCurrentWindowHdl()
        {
            return GetServiceProvider().GetWindowManageService().GetCurrentWindowHdl();
        }

        public override RectResponse GetCurrentWindowRect()
        {
            return GetServiceProvider().GetWindowManageService().GetCurrentWindowRect();
        }

        public override string GetScreenshot()
        {
            var currentHdl = GetCurrentWindowHdl();
            using (Bitmap bmp = GetServiceProvider().GetScreenCaptureService().CaptureWindowScreenshot(int.Parse(currentHdl)))
            {
                return GetServiceProvider().GetScreenCaptureService().ConvertToBase64(bmp);
            }
        }

        public override string GetPageSource()
        {
            var wnd = GetServiceProvider().GetWindowManageService().getCurrentWindow(null);
            return GetServiceProvider().GetPageSourceService().BuildPageSource(wnd).pageSource.ToString();
        }

        public override FindElementResponse FindElement(JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            return GetServiceProvider().GetElementFinderService().FindElement(rq, GetServiceProvider().GetWindowManageService().getCurrentWindow(null));
        }

        public override List<FindElementResponse> FindElements(JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            return GetServiceProvider().GetElementFinderService().FindElements(rq, GetServiceProvider().GetWindowManageService().getCurrentWindow(null));
        }

        public override FindElementResponse FindElementFromElement(string elementId, JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            return GetServiceProvider().GetElementFinderService().FindElementFromParentElement(rq, elementId);
        }

        public override List<FindElementResponse> FindElementsFromElement(string elementId, JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            return GetServiceProvider().GetElementFinderService().FindElementsFromParentElement(rq, elementId);
        }

        public override FindElementResponse GetActiveElement()
        {
            return GetServiceProvider().GetElementFinderService().GetActiveElement();
        }

        public override string? GetElementAttribute(string id, string attribute)
        {
            GetCurrentWindowHdl();
            var element = GetServiceProvider().GetElementFinderService().GetElement(id);
            return GetServiceProvider().GetElementAttributeService().GetAttributeString(element, attribute);
        }

        public override string GetElementTagName(string id)
        {
            GetCurrentWindowHdl();
            var element = GetServiceProvider().GetElementFinderService().GetElement(id);
            return GetServiceProvider().GetElementAttributeService().GetElementTagName(element);
        }

        public override string GetElementText(string id)
        {
            GetCurrentWindowHdl();
            var element = GetServiceProvider().GetElementFinderService().GetElement(id);
            return GetServiceProvider().GetElementAttributeService().GetElementText(element);
        }

        public override bool IsElementEnabled(string id)
        {
            GetCurrentWindowHdl();
            var element = GetServiceProvider().GetElementFinderService().GetElement(id);
            return GetServiceProvider().GetElementAttributeService().IsElementEnabled(element);
        }

        public override bool IsElementSelected(string id)
        {
            GetCurrentWindowHdl();
            var element = GetServiceProvider().GetElementFinderService().GetElement(id);
            return GetServiceProvider().GetElementAttributeService().IsElementSelected(element);
        }

        public override bool IsElementDisplayed(string id)
        {
            GetCurrentWindowHdl();
            var element = GetServiceProvider().GetElementFinderService().GetElement(id);
            return GetServiceProvider().GetElementAttributeService().IsElementDisplayed(element);
        }

        public override RectResponse GetElementRect(string id)
        {
            var wndRect = GetCurrentWindowRect();
            var element = GetServiceProvider().GetElementFinderService().GetElement(id);
            return GetServiceProvider().GetElementAttributeService().GetElementRect(element, new Point(wndRect.x, wndRect.y));
        }

        public override async Task PerformActions(JsonObject action)
        {
            InputState inputState = InputState.Instance();
            ActionOptions option = GetActionOption();
            var actionsByTick = inputState.ExtractActionSequence(action, option);
            await inputState.DispatchAction(actionsByTick, option);
        }

        public override async Task ReleaseActions()
        {
            InputState inputState = InputState.Instance();
            ActionOptions option = GetActionOption();
            await inputState.Release(option);
        }

        public override async Task ElementClick(string elementId)
        {
            GetServiceProvider().GetWindowManageService().getCurrentWindowThenFocus(null);
            var element = GetServiceProvider().GetElementFinderService().GetElement(elementId);

            await GetServiceProvider().GetActionsService().ElementClick(elementId, element, GetActionOption());
        }

        public override void ElementClear(string elementId)
        {
            GetServiceProvider().GetWindowManageService().getCurrentWindowThenFocus(null);
            var element = GetServiceProvider().GetElementFinderService().GetElement(elementId);

            GetServiceProvider().GetActionsService().ElementClear(element);
        }

        public override async Task ElementSendKeys(string elementId, JsonObject data)
        {
            data.TryGetPropertyValue("text", out var text);
            if (text == null || text.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("text must be string");
            }

            GetServiceProvider().GetWindowManageService().getCurrentWindowThenFocus(null);
            var element = GetServiceProvider().GetElementFinderService().GetElement(elementId);

            await GetServiceProvider().GetActionsService().ElementSendKeys(element, text.ToString(), GetActionOption());
        }

        public override string GetElementScreenshot(string elementId)
        {
            GetServiceProvider().GetWindowManageService().getCurrentWindow(null);
            //  Get element already include rect in cache
            var element = GetServiceProvider().GetElementFinderService().GetElement(elementId);
            var rect = element.Cached.BoundingRectangle;

            using (Bitmap bmp = GetServiceProvider().GetScreenCaptureService().CaptureElementScreenshot((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height))
            {
                return GetServiceProvider().GetScreenCaptureService().ConvertToBase64(bmp);
            }
        }

        
    }
}
