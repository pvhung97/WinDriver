using System.Text.Json.Nodes;
using System.Text.Json;
using UIADriver.actions;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.uia2.attribute;
using System.Windows.Automation;
using UIADriver.services;
using UIADriver.services.pattern;
using UIADriver.uia2.pattern;

namespace UIADriver.uia2.session
{
    public abstract class UIA2Session : Session<AutomationElement, CacheRequest>
    {
        public UIA2Session(SessionCapabilities capabilities) : base(capabilities) { }

        protected abstract ActionOptions GetActionOption();

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

        protected override AnnotationPatternService<AutomationElement, CacheRequest> GetAnnotationPatternService()
        {
            throw new UnsupportedOperation("AnnotationPattern is not supported in UIA2");
        }
        protected override BasePatternService<AutomationElement, CacheRequest> GetBasePatternService()
        {
            if (BasePatternService == null)
            {
                BasePatternService = new UIA2BasePattern(GetElementFinderService(), GetElementAttributeService());
            }
            return BasePatternService;
        }
        protected override CustomNavigationPatternService<AutomationElement, CacheRequest> GetCustomNavigationPatternService()
        {
            throw new UnsupportedOperation("CustomNavigationPattern is not supported in UIA2");
        }
        protected override DockPatternService<AutomationElement, CacheRequest> GetDockPatternService()
        {
            if (DockPatternService == null)
            {
                DockPatternService = new UIA2DockPattern(GetElementFinderService(), GetElementAttributeService());
            }
            return DockPatternService;
        }
        protected override DragPatternService<AutomationElement, CacheRequest> GetDragPatternService()
        {
            throw new UnsupportedOperation("DragPattern is not supported in UIA2");
        }
        protected override DropTargetPatternService<AutomationElement, CacheRequest> GetDropTargetPatternService()
        {
            throw new UnsupportedOperation("DropTargetPattern is not supported in UIA2");
        }
        protected override ExpandCollapsePatternService<AutomationElement, CacheRequest> GetExpandCollapsePatternService()
        {
            if (ExpandCollapsePatternService == null)
            {
                ExpandCollapsePatternService = new UIA2ExpandCollapsePattern(GetElementFinderService(), GetElementAttributeService());
            }
            return ExpandCollapsePatternService;
        }
        protected override GridItemPatternService<AutomationElement, CacheRequest> GetGridItemPatternService()
        {
            if (GridItemPatternService == null)
            {
                GridItemPatternService = new UIA2GridItemPattern(GetElementFinderService(), GetElementAttributeService());
            }
            return GridItemPatternService;
        }
        protected override GridPatternService<AutomationElement, CacheRequest> GetGridPatternService()
        {
            if (GridPatternService == null)
            {
                GridPatternService = new UIA2GridPattern(GetElementFinderService(), GetElementAttributeService());
            }
            return GridPatternService;
        }
        protected override InvokePatternService<AutomationElement, CacheRequest> GetInvokePatternService()
        {
            if (InvokePatternService == null)
            {
                InvokePatternService = new UIA2InvokePattern(GetElementFinderService(), GetElementAttributeService());
            }
            return InvokePatternService;
        }
        protected override MultipleViewPatternService<AutomationElement, CacheRequest> GetMultipleViewPatternService()
        {
            if (MultipleViewPatternService == null)
            {
                MultipleViewPatternService = new UIA2MultipleViewPattern(GetElementFinderService(), GetElementAttributeService());
            }
            return MultipleViewPatternService;
        }
        protected override RangeValuePatternService<AutomationElement, CacheRequest> GetRangeValuePatternService()
        {
            if (RangeValuePatternService == null)
            {
                RangeValuePatternService = new UIA2RangeValuePattern(GetElementFinderService(), GetElementAttributeService());
            }
            return RangeValuePatternService;
        }
        protected override ScrollItemPatternService<AutomationElement, CacheRequest> GetScrollItemPatternService()
        {
            if (ScrollItemPatternService == null)
            {
                ScrollItemPatternService = new UIA2ScrollItemPattern(GetElementFinderService(), GetElementAttributeService());
            }
            return ScrollItemPatternService;
        }
        protected override ScrollPatternService<AutomationElement, CacheRequest> GetScrollPatternService()
        {
            if (ScrollPatternService == null)
            {
                ScrollPatternService = new UIA2ScrollPattern(GetElementFinderService(), GetElementAttributeService());
            }
            return ScrollPatternService;
        }
        protected override SelectionItemPatternService<AutomationElement, CacheRequest> GetSelectionItemPatternService()
        {
            if (SelectionItemPatternService == null)
            {
                SelectionItemPatternService = new UIA2SelectionItemPattern(GetElementFinderService(), GetElementAttributeService());
            }
            return SelectionItemPatternService;
        }
        protected override SelectionPattern2Service<AutomationElement, CacheRequest> GetSelectionPattern2Service()
        {
            throw new UnsupportedOperation("SelectionPattern2 is not supported in UIA2");
        }
        protected override SpreadSheetItemPatternService<AutomationElement, CacheRequest> GetSpreadSheetItemPatternService()
        {
            throw new UnsupportedOperation("SpreadSheetItemPattern is not supported in UIA2");
        }
        protected override SpreadSheetPatternService<AutomationElement, CacheRequest> GetSpreadSheetPatternService()
        {
            throw new UnsupportedOperation("SpreadSheetPattern is not supported in UIA2");
        }
        protected override TableItemPatternService<AutomationElement, CacheRequest> GetTableItemPatternService()
        {
            if (TableItemPatternService == null)
            {
                TableItemPatternService = new UIA2TableItemPattern(GetElementFinderService(), GetElementAttributeService());
            }
            return TableItemPatternService;
        }
        protected override TablePatternService<AutomationElement, CacheRequest> GetTablePatternService()
        {
            if (TablePatternService == null)
            {
                TablePatternService = new UIA2TablePattern(GetElementFinderService(), GetElementAttributeService());
            }
            return TablePatternService;
        }
        protected override TogglePatternService<AutomationElement, CacheRequest> GetTogglePatternService()
        {
            if (TogglePatternService == null)
            {
                TogglePatternService = new UIA2TogglePattern(GetElementFinderService(), GetElementAttributeService());
            }
            return TogglePatternService;
        }
        protected override TransformPattern2Service<AutomationElement, CacheRequest> GetTransformPattern2Service()
        {
            throw new UnsupportedOperation("TransformPattern2 is not supported in UIA2");

        }
        protected override TransformPatternService<AutomationElement, CacheRequest> GetTransformPatternService()
        {
            if (TransformPatternService == null)
            {
                TransformPatternService = new UIA2TransformPattern(GetElementFinderService(), GetElementAttributeService());
            }
            return TransformPatternService;
        }
        protected override ValuePatternService<AutomationElement, CacheRequest> GetValuePatternService()
        {
            if (ValuePatternService == null)
            {
                ValuePatternService = new UIA2ValuePattern(GetElementFinderService(), GetElementAttributeService());
            }
            return ValuePatternService;
        }
        protected override VirtualizedItemPatternService<AutomationElement, CacheRequest> GetVirtualizedItemPatternService()
        {
            if (VirtualizedItemPatternService == null)
            {
                VirtualizedItemPatternService = new UIA2VirtualizedItemPattern(GetElementFinderService(), GetElementAttributeService());
            }
            return VirtualizedItemPatternService;
        }
        protected override WindowPatternService<AutomationElement, CacheRequest> GetWindowPatternService()
        {
            if (WindowPatternService == null)
            {
                WindowPatternService = new UIA2WindowPattern(GetElementFinderService(), GetElementAttributeService());
            }
            return WindowPatternService;
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
            GetWindowManageService().getCurrentWindowThenFocus(null);
            var element = GetElementFinderService().GetElement(elementId);

            await GetActionsService().ElementClick(elementId, element, GetActionOption());
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

            await GetActionsService().ElementSendKeys(element, text.ToString(), GetActionOption());
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
