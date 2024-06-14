using System.Text.Json;
using System.Text.Json.Nodes;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver
{
    public abstract class Session<T, U> : ISession
    {
        protected SessionCapabilities capabilities;

        protected ScreenshotCapture? ScreenCaptureService;
        protected ElementAttributeService<T>? AttrService;
        protected PageSourceService<T>? PageSourceService;
        protected ElementFinderService<T, U>? ElementFinderService;
        protected WindowManageService<T, U>? WindowManageService;
        protected ActionsService<T>? ActionsService;

        protected AnnotationPatternService<T, U>? AnnotationPatternService;
        protected BasePatternService<T, U>? BasePatternService;
        protected CustomNavigationPatternService<T, U>? CustomNavigationPatternService;
        protected DockPatternService<T, U>? DockPatternService;
        protected DragPatternService<T, U>? DragPatternService;
        protected DropTargetPatternService<T, U>? DropTargetPatternService;
        protected ExpandCollapsePatternService<T, U>? ExpandCollapsePatternService;
        protected GridItemPatternService<T, U>? GridItemPatternService;
        protected GridPatternService<T, U>? GridPatternService;
        protected InvokePatternService<T, U>? InvokePatternService;
        protected MultipleViewPatternService<T, U>? MultipleViewPatternService;
        protected RangeValuePatternService<T, U>? RangeValuePatternService;
        protected ScrollItemPatternService<T, U>? ScrollItemPatternService;
        protected ScrollPatternService<T, U>? ScrollPatternService;
        protected SelectionItemPatternService<T, U>? SelectionItemPatternService;
        protected SelectionPattern2Service<T, U>? SelectionPattern2Service;
        protected SpreadSheetItemPatternService<T, U>? SpreadSheetItemPatternService;
        protected SpreadSheetPatternService<T, U>? SpreadSheetPatternService;
        protected TableItemPatternService<T, U>? TableItemPatternService;
        protected TablePatternService<T, U>? TablePatternService;
        protected TogglePatternService<T, U>? TogglePatternService;
        protected TransformPattern2Service<T, U>? TransformPattern2Service;
        protected TransformPatternService<T, U>? TransformPatternService;
        protected ValuePatternService<T, U>? ValuePatternService;
        protected VirtualizedItemPatternService<T, U>? VirtualizedItemPatternService;
        protected WindowPatternService<T, U>? WindowPatternService;

        public Session(SessionCapabilities capabilities)
        {
            this.capabilities = capabilities;
        }

        protected abstract ScreenshotCapture GetScreenCaptureService();
        protected abstract ElementAttributeService<T> GetElementAttributeService();
        protected abstract PageSourceService<T> GetPageSourceService();
        protected abstract ElementFinderService<T, U> GetElementFinderService();
        protected abstract WindowManageService<T, U> GetWindowManageService();
        protected abstract ActionsService<T> GetActionsService();

        protected abstract AnnotationPatternService<T, U> GetAnnotationPatternService();
        protected abstract BasePatternService<T, U> GetBasePatternService();
        protected abstract CustomNavigationPatternService<T, U> GetCustomNavigationPatternService();
        protected abstract DockPatternService<T, U> GetDockPatternService();
        protected abstract DragPatternService<T, U> GetDragPatternService();
        protected abstract DropTargetPatternService<T, U> GetDropTargetPatternService();
        protected abstract ExpandCollapsePatternService<T, U> GetExpandCollapsePatternService();
        protected abstract GridItemPatternService<T, U> GetGridItemPatternService();
        protected abstract GridPatternService<T, U> GetGridPatternService();
        protected abstract InvokePatternService<T, U> GetInvokePatternService();
        protected abstract MultipleViewPatternService<T, U> GetMultipleViewPatternService();
        protected abstract RangeValuePatternService<T, U> GetRangeValuePatternService();
        protected abstract ScrollItemPatternService<T, U> GetScrollItemPatternService();
        protected abstract ScrollPatternService<T, U> GetScrollPatternService();
        protected abstract SelectionItemPatternService<T, U> GetSelectionItemPatternService();
        protected abstract SelectionPattern2Service<T, U> GetSelectionPattern2Service();
        protected abstract SpreadSheetItemPatternService<T, U> GetSpreadSheetItemPatternService();
        protected abstract SpreadSheetPatternService<T, U> GetSpreadSheetPatternService();
        protected abstract TableItemPatternService<T, U> GetTableItemPatternService();
        protected abstract TablePatternService<T, U> GetTablePatternService();
        protected abstract TogglePatternService<T, U> GetTogglePatternService();
        protected abstract TransformPattern2Service<T, U> GetTransformPattern2Service();
        protected abstract TransformPatternService<T, U> GetTransformPatternService();
        protected abstract ValuePatternService<T, U> GetValuePatternService();
        protected abstract VirtualizedItemPatternService<T, U> GetVirtualizedItemPatternService();
        protected abstract WindowPatternService<T, U> GetWindowPatternService();

        //  W3C API
        public abstract HashSet<string> CollectWindowHandles();
        public abstract Task CloseSession();
        public abstract string GetCurrentWindowTitle();
        public abstract string GetCurrentWindowHdl();
        public abstract RectResponse GetCurrentWindowRect();
        public abstract RectResponse MinimizeCurrentWindow();
        public abstract RectResponse MaximizeCurrentWindow();
        public abstract RectResponse SetWindowRect(JsonObject data);
        public abstract HashSet<string> CloseCurrentWindow();
        public abstract void SwitchToWindow(JsonObject windowHandle);
        public abstract string GetScreenshot();
        public abstract string GetPageSource();
        public abstract FindElementResponse FindElement(JsonObject data);
        public abstract List<FindElementResponse> FindElements(JsonObject data);
        public abstract FindElementResponse FindElementFromElement(string elementId, JsonObject data);
        public abstract List<FindElementResponse> FindElementsFromElement(string elementId, JsonObject data);
        public abstract FindElementResponse GetActiveElement();
        public abstract string? GetElementAttribute(string id, string attribute);
        public abstract string GetElementTagName(string id);
        public abstract RectResponse GetElementRect(string id);
        public abstract string GetElementText(string id);
        public abstract bool IsElementEnabled(string id);
        public abstract bool IsElementSelected(string id);
        public abstract bool IsElementDisplayed(string id);
        public abstract Task PerformActions(JsonObject action);
        public abstract Task ReleaseActions();
        public abstract Task ElementClick(string elementId);
        public abstract void ElementClear(string elementId);
        public abstract Task ElementSendKeys(string elementId, JsonObject data);
        public abstract string GetElementScreenshot(string elementId);

        //  Element Pattern API
        public FindElementResponse GetAnnotationTarget(string elementId)
        {
            GetCurrentWindowHdl();
            return GetAnnotationPatternService().GetTarget(elementId);
        }
        public FindElementResponse GetLabeledBy(string elementId)
        {
            GetCurrentWindowHdl();
            return GetBasePatternService().GetLabeledBy(elementId);
        }
        public List<FindElementResponse> GetControllerFor(string elementId)
        {
            GetCurrentWindowHdl();
            return GetBasePatternService().GetControllerFor(elementId);
        }
        public List<FindElementResponse> GetDescribedBy(string elementId)
        {
            GetCurrentWindowHdl();
            return GetBasePatternService().GetDescribedBy(elementId);
        }
        public List<FindElementResponse> GetFlowsTo(string elementId)
        {
            GetCurrentWindowHdl();
            return GetBasePatternService().GetFlowsTo(elementId);
        }
        public void SetFocus(string elementId)
        {
            GetCurrentWindowHdl();
            GetBasePatternService().SetFocus(elementId);
        }
        public FindElementResponse NavigateFollowDirection(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("value must be string");
            }
            return GetCustomNavigationPatternService().Navigate(elementId, value.GetValue<string>());
        }
        public void SetDockPosition(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("value must be string");
            }
            GetDockPatternService().SetDockPosition(elementId, value.GetValue<string>());
        }
        public List<string> GetDropEffects(string elementId)
        {
            GetCurrentWindowHdl();
            return GetDragPatternService().GetDropEffects(elementId);
        }
        public List<FindElementResponse> GetGrabbedItems(string elementId)
        {
            GetCurrentWindowHdl();
            return GetDragPatternService().GetGrabbedItems(elementId);
        }
        public List<string> GetDropTargetEffects(string elementId)
        {
            GetCurrentWindowHdl();
            return GetDropTargetPatternService().GetDropTargetEffects(elementId);
        }
        public void ExpandOrCollapseElement(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || (value.GetValueKind() != JsonValueKind.True && value.GetValueKind() != JsonValueKind.False))
            {
                throw new InvalidArgument("value must be boolean");
            }
            GetExpandCollapsePatternService().ExpandOrCollapseElement(elementId, value.GetValue<bool>());
        }
        public FindElementResponse GetContainingGrid(string elementId)
        {
            GetCurrentWindowHdl();
            return GetGridItemPatternService().GetContainingGrid(elementId);
        }
        public FindElementResponse GetGridItem(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = GetGridItemRequest.Validate(data);
            return GetGridPatternService().GetItem(elementId, req.row, req.column);
        }
        public void Invoke(string elementId)
        {
            GetCurrentWindowHdl();
            GetInvokePatternService().Invoke(elementId);
        }
        public List<int> GetSupportedViewIds(string elementId)
        {
            GetCurrentWindowHdl();
            return GetMultipleViewPatternService().GetSupportedViewIds(elementId);
        }
        public string GetViewName(string elementId, string viewId)
        {
            GetCurrentWindowHdl();
            int vid;
            try
            {
                vid = int.Parse(viewId);
            } catch 
            {
                throw new InvalidArgument("viewId must be integer");
            }
            return GetMultipleViewPatternService().GetViewName(elementId, vid);
        }
        public void SetCurrentView(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("value must be number");
            }
            GetMultipleViewPatternService().SetCurrentView(elementId, value.GetValue<int>());
        }
        public void SetRangeValue(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("value must be number");
            }
            GetRangeValuePatternService().SetValue(elementId, value.GetValue<double>());
        }
        public void ScrollIntoView(string elementId)
        {
            GetCurrentWindowHdl();
            GetScrollItemPatternService().ScrollIntoView(elementId);
        }
        public void Scroll(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = ScrollByAmountRequest.Validate(data);
            GetScrollPatternService().Scroll(elementId, req.horizontalAmount, req.verticalAmount);
        }
        public void SetScrollPercent(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = SetScrollPercentRequest.Validate(data);
            GetScrollPatternService().SetScrollPercent(elementId, req.horizontalPercent, req.verticalPercent);
        }
        public void Select(string elementId)
        {
            GetCurrentWindowHdl();
            GetSelectionItemPatternService().Select(elementId);
        }
        public void AddToSelection(string elementId)
        {
            GetCurrentWindowHdl();
            GetSelectionItemPatternService().AddToSelection(elementId);
        }
        public void RemoveFromSelection(string elementId)
        {
            GetCurrentWindowHdl();
            GetSelectionItemPatternService().RemoveFromSelection(elementId);
        }
        public FindElementResponse GetSelectionContainer(string elementId)
        {
            GetCurrentWindowHdl();
            return GetSelectionItemPatternService().GetSelectionContainer(elementId);
        }
        public FindElementResponse GetFirstSelectedItem(string elementId)
        {
            GetCurrentWindowHdl();
            return GetSelectionPattern2Service().GetFirstSelectedItem(elementId);
        }
        public FindElementResponse GetLastSelectedItem(string elementId)
        {
            GetCurrentWindowHdl();
            return GetSelectionPattern2Service().GetLastSelectedItem(elementId);
        }
        public FindElementResponse GetCurrentSelectedItem(string elementId)
        {
            GetCurrentWindowHdl();
            return GetSelectionPattern2Service().GetCurrentSelectedItem(elementId);
        }
        public List<FindElementResponse> GetAnnotationObjects(string elementId)
        {
            GetCurrentWindowHdl();
            return GetSpreadSheetItemPatternService().GetAnnotationObjects(elementId);
        }
        public FindElementResponse GetSpreadSheetItemByName(string elementId, string name)
        {
            GetCurrentWindowHdl();
            return GetSpreadSheetPatternService().GetItemByName(elementId, name);
        }
        public List<FindElementResponse> GetRowHeaderItems(string elementId)
        {
            GetCurrentWindowHdl();
            return GetTableItemPatternService().GetRowHeaderItems(elementId);
        }
        public List<FindElementResponse> GetColumnHeaderItems(string elementId)
        {
            GetCurrentWindowHdl();
            return GetTableItemPatternService().GetColumnHeaderItems(elementId);
        }
        public List<FindElementResponse> GetRowHeaders(string elementId)
        {
            GetCurrentWindowHdl();
            return GetTablePatternService().GetRowHeaders(elementId);
        }
        public List<FindElementResponse> GetColumnHeader(string elementId)
        {
            GetCurrentWindowHdl();
            return GetTablePatternService().GetColumnHeaders(elementId);
        }
        public void Toggle(string elementId)
        {
            GetCurrentWindowHdl();
            GetTogglePatternService().Toggle(elementId);
        }
        public void Zoom(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("value must be number");
            }
            GetTransformPattern2Service().Zoom(elementId, value.GetValue<double>());
        }
        public void ZoomByUnit(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("value must be string");
            }
            GetTransformPattern2Service().ZoomByUnit(elementId, value.GetValue<string>());
        }
        public void Move(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = MoveRequest.Validate(data);
            GetTransformPatternService().Move(elementId, req.x, req.y);
        }
        public void Resize(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = ResizeRequest.Validate(data);
            GetTransformPatternService().Resize(elementId, req.width, req.height);
        }
        public void Rotate(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("value must be number");
            }
            GetTransformPatternService().Rotate(elementId, value.GetValue<double>());
        }
        public void SetValue(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("value must be string");
            }
            GetValuePatternService().SetValue(elementId, value.GetValue<string>());
        }
        public void Realize(string elementId)
        {
            GetCurrentWindowHdl();
            GetVirtualizedItemPatternService().Realize(elementId);
        }
        public void SetVisualState(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("value must be string");
            }
            GetWindowPatternService().SetVisualState(elementId, value.GetValue<string>());
        }
        public void WaitForInputIdle(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("value must be integer");
            }
            GetWindowPatternService().WaitForInputIdle(elementId, value.GetValue<int>());
        }
        public void CloseWindow(string elementId)
        {
            GetCurrentWindowHdl();
            GetWindowPatternService().Close(elementId);
        }
        public RectResponse GetWindowRect(string elementId)
        {
            GetCurrentWindowHdl();
            return GetWindowPatternService().GetRect(elementId);
        }
        public void BringWindowToTop(string elementId)
        {
            GetCurrentWindowHdl();
            GetWindowPatternService().BringToTop(elementId);
        }
        public void SetWindowRect(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = SetRectRequest.Validate(data);
            GetWindowPatternService().SetWindowRect(elementId, req);
        }
    }
}
