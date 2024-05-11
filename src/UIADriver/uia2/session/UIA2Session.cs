using System.Text.Json.Nodes;
using System.Text.Json;
using UIADriver.actions;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.uia2.attribute;
using System.Windows.Automation;
using UIADriver.services;

namespace UIADriver.uia2.session
{
    public abstract class UIA2Session : Session<AutomationElement, CacheRequest>
    {
        public UIA2Session(SessionCapabilities capabilities) : base(capabilities) { }

        protected abstract ActionOptions getActionOption();

        protected override ScreenshotCapture GetScreenCaptureService()
        {
            if (ScreenCaptureService == null)
            {
                ScreenCaptureService = new ScreenshotCapture();
            }
            return ScreenCaptureService;
        }
        protected override ElementAttributeService<AutomationElement> GetElementAttributeService()
        {
            if (AttrService == null)
            {
                AttrService = new ElementAttributeGetter();
            }
            return AttrService;
        }
        protected override ElementFinderService<AutomationElement, CacheRequest> GetElementFinderService()
        {
            if (ElementFinderService == null)
            {
                ElementFinderService = new ElementFinder(GetPageSourceService(), GetElementAttributeService());
            }
            return ElementFinderService;
        }
        protected override ActionsService<AutomationElement> GetActionsService()
        {
            if (ActionsService == null)
            {
                ActionsService = new UIA2ActionsService(capabilities);
            }
            return ActionsService;
        }

        public override HashSet<string> CollectWindowHandles()
        {
            return GetWindowManageService().CollectWindowHandles().ToHashSet();
        }

        public override RectResponse MinimizeCurrentWindow()
        {
            return GetWindowManageService().MinimizeCurrentWindow();
        }

        public override RectResponse MaximizeCurrentWindow()
        {
            return GetWindowManageService().MaximizeCurrentWindow();
        }

        public override RectResponse SetWindowRect(JsonObject data)
        {
            var rq = SetRectRequest.Validate(data);
            return GetWindowManageService().SetWindowRect(rq);
        }

        public override HashSet<string> CloseCurrentWindow()
        {
            return GetWindowManageService().CloseCurrentWindow().ToHashSet();
        }

        public override void SwitchToWindow(JsonObject windowHandle)
        {
            var rq = SwitchWindowRequest.Validate(windowHandle);
            GetWindowManageService().SwitchToWindow(rq);
        }

        public override string GetCurrentWindowTitle()
        {
            return GetWindowManageService().GetCurrentWindowTitle();
        }

        public override string GetCurrentWindowHdl()
        {
            return GetWindowManageService().GetCurrentWindowHdl();
        }

        public override RectResponse GetCurrentWindowRect()
        {
            return GetWindowManageService().GetCurrentWindowRect();
        }

        public override string GetScreenshot()
        {
            var currentHdl = GetCurrentWindowHdl();
            using (Bitmap bmp = GetScreenCaptureService().CaptureWindowScreenshot(int.Parse(currentHdl)))
            {
                return GetScreenCaptureService().ConvertToBase64(bmp);
            }
        }

        public override string GetPageSource()
        {
            var wnd = GetWindowManageService().getCurrentWindow(null);
            return GetPageSourceService().BuildPageSource(wnd).pageSource.ToString();
        }

        public override FindElementResponse FindElement(JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            return GetElementFinderService().FindElement(rq, GetWindowManageService().getCurrentWindow(null));
        }

        public override List<FindElementResponse> FindElements(JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            return GetElementFinderService().FindElements(rq, GetWindowManageService().getCurrentWindow(null));
        }

        public override FindElementResponse FindElementFromElement(string elementId, JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            return GetElementFinderService().FindElementFromParentElement(rq, elementId);
        }

        public override List<FindElementResponse> FindElementsFromElement(string elementId, JsonObject data)
        {
            FindElementRequest rq = FindElementRequest.Validate(data);
            return GetElementFinderService().FindElementsFromParentElement(rq, elementId);
        }

        public override FindElementResponse GetActiveElement()
        {
            return GetElementFinderService().GetActiveElement();
        }

        public override string? GetElementAttribute(string id, string attribute)
        {
            GetCurrentWindowHdl();
            var element = GetElementFinderService().GetElement(id);
            return GetElementAttributeService().GetAttributeString(element, attribute);
        }

        public override string GetElementTagName(string id)
        {
            GetCurrentWindowHdl();
            var element = GetElementFinderService().GetElement(id);
            return GetElementAttributeService().GetElementTagName(element);
        }

        public override string GetElementText(string id)
        {
            GetCurrentWindowHdl();
            var element = GetElementFinderService().GetElement(id);
            return GetElementAttributeService().GetElementText(element);
        }

        public override bool IsElementEnabled(string id)
        {
            GetCurrentWindowHdl();
            var element = GetElementFinderService().GetElement(id);
            return GetElementAttributeService().IsElementEnabled(element);
        }

        public override bool IsElementSelected(string id)
        {
            GetCurrentWindowHdl();
            var element = GetElementFinderService().GetElement(id);
            return GetElementAttributeService().IsElementSelected(element);
        }

        public override bool IsElementDisplayed(string id)
        {
            GetCurrentWindowHdl();
            var element = GetElementFinderService().GetElement(id);
            return GetElementAttributeService().IsElementDisplayed(element);
        }

        public override RectResponse GetElementRect(string id)
        {
            var wndRect = GetCurrentWindowRect();
            var element = GetElementFinderService().GetElement(id);
            return GetElementAttributeService().GetElementRect(element, new Point(wndRect.x, wndRect.y));
        }

        public override async Task PerformActions(JsonObject action)
        {
            InputState inputState = InputState.Instance();
            ActionOptions option = getActionOption();
            var actionsByTick = inputState.ExtractActionSequence(action, option);
            await inputState.DispatchAction(actionsByTick, option);
        }

        public override async Task ReleaseActions()
        {
            InputState inputState = InputState.Instance();
            ActionOptions option = getActionOption();
            await inputState.Release(option);
        }

        public override async Task ElementClick(string elementId)
        {
            GetWindowManageService().getCurrentWindowThenFocus(null);
            var element = GetElementFinderService().GetElement(elementId);

            await GetActionsService().ElementClick(elementId, element, getActionOption());
        }

        public override void ElementClear(string elementId)
        {
            GetWindowManageService().getCurrentWindowThenFocus(null);
            var element = GetElementFinderService().GetElement(elementId);

            GetActionsService().ElementClear(element);
        }

        public override async Task ElementSendKeys(string elementId, JsonObject data)
        {
            data.TryGetPropertyValue("text", out var text);
            if (text == null || text.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("text must be string");
            }

            GetWindowManageService().getCurrentWindowThenFocus(null);
            var element = GetElementFinderService().GetElement(elementId);

            await GetActionsService().ElementSendKeys(element, text.ToString(), getActionOption());
        }

        public override string GetElementScreenshot(string elementId)
        {
            GetWindowManageService().getCurrentWindow(null);
            //  Get element already include rect in cache
            var element = GetElementFinderService().GetElement(elementId);
            var rect = element.Cached.BoundingRectangle;

            using (Bitmap bmp = GetScreenCaptureService().CaptureElementScreenshot((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height))
            {
                return GetScreenCaptureService().ConvertToBase64(bmp);
            }
        }

        
    }
}
