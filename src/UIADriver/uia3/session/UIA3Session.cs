using Interop.UIAutomationClient;
using System.Text.Json;
using System.Text.Json.Nodes;
using UIADriver.actions;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;
using UIADriver.uia3.attribute;
using UIADriver.uia3.pattern;

namespace UIADriver.uia3.session
{
    public abstract class UIA3Session : Session<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;

        public UIA3Session(SessionCapabilities capabilities) : base(capabilities)
        {
            try
            {
                automation = new CUIAutomation8();
            }
            catch
            {
                automation = new CUIAutomation();
            }
        }

        protected abstract ActionOptions GetActionOption();

        protected override ScreenshotCapture GetScreenCaptureService()
        {
            if (ScreenCaptureService == null)
            {
                ScreenCaptureService = new ScreenshotCapture();
            }
            return ScreenCaptureService;
        }
        protected override ElementAttributeService<IUIAutomationElement> GetElementAttributeService()
        {
            if (AttrService == null)
            {
                AttrService = new ElementAttributeGetter(automation);
            }
            return AttrService;
        }
        protected override ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> GetElementFinderService()
        {
            if (ElementFinderService == null)
            {
                ElementFinderService = new ElementFinder(automation, GetPageSourceService(), GetElementAttributeService());
            }
            return ElementFinderService;
        }
        protected override ActionsService<IUIAutomationElement> GetActionsService()
        {
            if (ActionsService == null)
            {
                ActionsService = new UIA3ActionsService(automation, capabilities);
            }
            return ActionsService;
        }

        protected override AnnotationPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetAnnotationPatternService()
        {
            if (AnnotationPatternService == null)
            {
                AnnotationPatternService = new UIA3AnnotationPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return AnnotationPatternService;
        }
        protected override BasePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetBasePatternService()
        {
            if (BasePatternService == null)
            {
                BasePatternService = new UIA3BasePattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return BasePatternService;
        }
        protected override CustomNavigationPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetCustomNavigationPatternService()
        {
            if (CustomNavigationPatternService == null)
            {
                CustomNavigationPatternService = new UIA3CustomNavigationPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return CustomNavigationPatternService;
        }
        protected override DockPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetDockPatternService()
        {
            if (DockPatternService == null)
            {
                DockPatternService = new UIA3DockPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return DockPatternService;
        }
        protected override DragPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetDragPatternService()
        {
            if (DragPatternService == null)
            {
                DragPatternService = new UIA3DragPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return DragPatternService;
        }
        protected override DropTargetPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetDropTargetPatternService()
        {
            if (DropTargetPatternService == null)
            {
                DropTargetPatternService = new UIA3DropTargetPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return DropTargetPatternService;
        }
        protected override ExpandCollapsePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetExpandCollapsePatternService()
        {
            if (ExpandCollapsePatternService == null)
            {
                ExpandCollapsePatternService = new UIA3ExpandCollapsePattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return ExpandCollapsePatternService;
        }
        protected override GridItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetGridItemPatternService()
        {
            if (GridItemPatternService == null)
            {
                GridItemPatternService = new UIA3GridItemPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return GridItemPatternService;
        }
        protected override GridPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetGridPatternService()
        {
            if (GridPatternService == null)
            {
                GridPatternService = new UIA3GridPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return GridPatternService;
        }
        protected override InvokePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetInvokePatternService()
        {
            if (InvokePatternService == null)
            {
                InvokePatternService = new UIA3InvokePattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return InvokePatternService;
        }
        protected override MultipleViewPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetMultipleViewPatternService()
        {
            if (MultipleViewPatternService == null)
            {
                MultipleViewPatternService = new UIA3MultipleViewPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return MultipleViewPatternService;
        }
        protected override RangeValuePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetRangeValuePatternService()
        {
            if (RangeValuePatternService == null)
            {
                RangeValuePatternService = new UIA3RangeValuePattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return RangeValuePatternService;
        }
        protected override ScrollItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetScrollItemPatternService()
        {
            if (ScrollItemPatternService == null)
            {
                ScrollItemPatternService = new UIA3ScrollItemPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return ScrollItemPatternService;
        }
        protected override ScrollPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetScrollPatternService()
        {
            if (ScrollPatternService == null)
            {
                ScrollPatternService = new UIA3ScrollPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return ScrollPatternService;
        }
        protected override SelectionItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetSelectionItemPatternService()
        {
            if (SelectionItemPatternService == null)
            {
                SelectionItemPatternService = new UIA3SelectionItemPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return SelectionItemPatternService;
        }
        protected override SelectionPattern2Service<IUIAutomationElement, IUIAutomationCacheRequest> GetSelectionPattern2Service()
        {
            if (SelectionPattern2Service == null)
            {
                SelectionPattern2Service = new UIA3SelectionPattern2(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return SelectionPattern2Service;
        }
        protected override SpreadSheetItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetSpreadSheetItemPatternService()
        {
            if (SpreadSheetItemPatternService == null)
            {
                SpreadSheetItemPatternService = new UIA3SpreadSheetItemPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return SpreadSheetItemPatternService;
        }
        protected override SpreadSheetPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetSpreadSheetPatternService()
        {
            if (SpreadSheetPatternService == null)
            {
                SpreadSheetPatternService = new UIA3SpreadSheetPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return SpreadSheetPatternService;
        }
        protected override TableItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetTableItemPatternService()
        {
            if (TableItemPatternService == null)
            {
                TableItemPatternService = new UIA3TableItemPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return TableItemPatternService;
        }
        protected override TablePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetTablePatternService()
        {
            if (TablePatternService == null)
            {
                TablePatternService = new UIA3TablePattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return TablePatternService;
        }
        protected override TogglePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetTogglePatternService()
        {
            if (TogglePatternService == null)
            {
                TogglePatternService = new UIA3TogglePattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return TogglePatternService;
        }
        protected override TransformPattern2Service<IUIAutomationElement, IUIAutomationCacheRequest> GetTransformPattern2Service()
        {
            if (TransformPattern2Service == null)
            {
                TransformPattern2Service = new UIA3TransformPattern2(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return TransformPattern2Service;

        }
        protected override TransformPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetTransformPatternService()
        {
            if (TransformPatternService == null)
            {
                TransformPatternService = new UIA3TransformPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return TransformPatternService;
        }
        protected override ValuePatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetValuePatternService()
        {
            if (ValuePatternService == null)
            {
                ValuePatternService = new UIA3ValuePattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return ValuePatternService;
        }
        protected override VirtualizedItemPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetVirtualizedItemPatternService()
        {
            if (VirtualizedItemPatternService == null)
            {
                VirtualizedItemPatternService = new UIA3VirtualizedItemPattern(GetElementFinderService(), GetElementAttributeService(), automation);
            }
            return VirtualizedItemPatternService;
        }
        protected override WindowPatternService<IUIAutomationElement, IUIAutomationCacheRequest> GetWindowPatternService()
        {
            if (WindowPatternService == null)
            {
                WindowPatternService = new UIA3WindowPattern(GetElementFinderService(), GetElementAttributeService(), automation);
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
            double[] rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);

            using (Bitmap bmp = GetScreenCaptureService().CaptureElementScreenshot((int)rect[0], (int)rect[1], (int)rect[2], (int)rect[3]))
            {
                return GetScreenCaptureService().ConvertToBase64(bmp);
            }
        }

    }
}
