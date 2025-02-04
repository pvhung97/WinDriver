using System.Text.Json;
using System.Text.Json.Nodes;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;

namespace UIADriver
{
    public abstract class Session<T, U> : ISession
    {
        protected SessionCapabilities capabilities;
        protected ServiceProvider<T, U> serviceProvider;

        public Session(SessionCapabilities capabilities)
        {
            this.capabilities = capabilities;
        }

        protected abstract ServiceProvider<T, U> GetServiceProvider();
        
        //  W3C API
        public abstract HashSet<string> CollectWindowHandles();
        public abstract Task CloseSession();
        public abstract string GetCurrentWindowTitle();
        public abstract string GetCurrentWindowProcessPath();
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
            return GetServiceProvider().GetAnnotationPatternService().GetTarget(elementId);
        }
        public FindElementResponse GetLabeledBy(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetBasePatternService().GetLabeledBy(elementId);
        }
        public List<FindElementResponse> GetControllerFor(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetBasePatternService().GetControllerFor(elementId);
        }
        public List<FindElementResponse> GetDescribedBy(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetBasePatternService().GetDescribedBy(elementId);
        }
        public List<FindElementResponse> GetFlowsTo(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetBasePatternService().GetFlowsTo(elementId);
        }
        public void SetFocus(string elementId)
        {
            GetCurrentWindowHdl();
            GetServiceProvider().GetBasePatternService().SetFocus(elementId);
        }
        public FindElementResponse NavigateFollowDirection(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("value must be string");
            }
            return GetServiceProvider().GetCustomNavigationPatternService().Navigate(elementId, value.GetValue<string>());
        }
        public void SetDockPosition(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("value must be string");
            }
            GetServiceProvider().GetDockPatternService().SetDockPosition(elementId, value.GetValue<string>());
        }
        public List<string> GetDropEffects(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetDragPatternService().GetDropEffects(elementId);
        }
        public List<FindElementResponse> GetGrabbedItems(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetDragPatternService().GetGrabbedItems(elementId);
        }
        public List<string> GetDropTargetEffects(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetDropTargetPatternService().GetDropTargetEffects(elementId);
        }
        public void ExpandOrCollapseElement(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || (value.GetValueKind() != JsonValueKind.True && value.GetValueKind() != JsonValueKind.False))
            {
                throw new InvalidArgument("value must be boolean");
            }
            GetServiceProvider().GetExpandCollapsePatternService().ExpandOrCollapseElement(elementId, value.GetValue<bool>());
        }
        public FindElementResponse GetContainingGrid(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetGridItemPatternService().GetContainingGrid(elementId);
        }
        public FindElementResponse GetGridItem(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = GetGridItemRequest.Validate(data);
            return GetServiceProvider().GetGridPatternService().GetItem(elementId, req.row, req.column);
        }
        public void Invoke(string elementId)
        {
            GetCurrentWindowHdl();
            GetServiceProvider().GetInvokePatternService().Invoke(elementId);
        }
        public List<int> GetSupportedViewIds(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetMultipleViewPatternService().GetSupportedViewIds(elementId);
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
            return GetServiceProvider().GetMultipleViewPatternService().GetViewName(elementId, vid);
        }
        public void SetCurrentView(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("value must be number");
            }
            GetServiceProvider().GetMultipleViewPatternService().SetCurrentView(elementId, value.GetValue<int>());
        }
        public void SetRangeValue(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("value must be number");
            }
            GetServiceProvider().GetRangeValuePatternService().SetValue(elementId, value.GetValue<double>());
        }
        public void ScrollIntoView(string elementId)
        {
            GetCurrentWindowHdl();
            GetServiceProvider().GetScrollItemPatternService().ScrollIntoView(elementId);
        }
        public void Scroll(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = ScrollByAmountRequest.Validate(data);
            GetServiceProvider().GetScrollPatternService().Scroll(elementId, req.horizontalAmount, req.verticalAmount);
        }
        public void SetScrollPercent(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = SetScrollPercentRequest.Validate(data);
            GetServiceProvider().GetScrollPatternService().SetScrollPercent(elementId, req.horizontalPercent, req.verticalPercent);
        }
        public void Select(string elementId)
        {
            GetCurrentWindowHdl();
            GetServiceProvider().GetSelectionItemPatternService().Select(elementId);
        }
        public void AddToSelection(string elementId)
        {
            GetCurrentWindowHdl();
            GetServiceProvider().GetSelectionItemPatternService().AddToSelection(elementId);
        }
        public void RemoveFromSelection(string elementId)
        {
            GetCurrentWindowHdl();
            GetServiceProvider().GetSelectionItemPatternService().RemoveFromSelection(elementId);
        }
        public FindElementResponse GetSelectionContainer(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetSelectionItemPatternService().GetSelectionContainer(elementId);
        }
        public FindElementResponse GetFirstSelectedItem(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetSelectionPattern2Service().GetFirstSelectedItem(elementId);
        }
        public FindElementResponse GetLastSelectedItem(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetSelectionPattern2Service().GetLastSelectedItem(elementId);
        }
        public FindElementResponse GetCurrentSelectedItem(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetSelectionPattern2Service().GetCurrentSelectedItem(elementId);
        }
        public List<FindElementResponse> GetAnnotationObjects(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetSpreadSheetItemPatternService().GetAnnotationObjects(elementId);
        }
        public FindElementResponse GetSpreadSheetItemByName(string elementId, string name)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetSpreadSheetPatternService().GetItemByName(elementId, name);
        }
        public List<FindElementResponse> GetRowHeaderItems(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetTableItemPatternService().GetRowHeaderItems(elementId);
        }
        public List<FindElementResponse> GetColumnHeaderItems(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetTableItemPatternService().GetColumnHeaderItems(elementId);
        }
        public List<FindElementResponse> GetRowHeaders(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetTablePatternService().GetRowHeaders(elementId);
        }
        public List<FindElementResponse> GetColumnHeader(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetTablePatternService().GetColumnHeaders(elementId);
        }
        public void Toggle(string elementId)
        {
            GetCurrentWindowHdl();
            GetServiceProvider().GetTogglePatternService().Toggle(elementId);
        }
        public void Zoom(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("value must be number");
            }
            GetServiceProvider().GetTransformPattern2Service().Zoom(elementId, value.GetValue<double>());
        }
        public void ZoomByUnit(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("value must be string");
            }
            GetServiceProvider().GetTransformPattern2Service().ZoomByUnit(elementId, value.GetValue<string>());
        }
        public void Move(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = MoveRequest.Validate(data);
            GetServiceProvider().GetTransformPatternService().Move(elementId, req.x, req.y);
        }
        public void Resize(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = ResizeRequest.Validate(data);
            GetServiceProvider().GetTransformPatternService().Resize(elementId, req.width, req.height);
        }
        public void Rotate(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("value must be number");
            }
            GetServiceProvider().GetTransformPatternService().Rotate(elementId, value.GetValue<double>());
        }
        public void SetValue(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("value must be string");
            }
            GetServiceProvider().GetValuePatternService().SetValue(elementId, value.GetValue<string>());
        }
        public void Realize(string elementId)
        {
            GetCurrentWindowHdl();
            GetServiceProvider().GetVirtualizedItemPatternService().Realize(elementId);
        }
        public void SetVisualState(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.String)
            {
                throw new InvalidArgument("value must be string");
            }
            GetServiceProvider().GetWindowPatternService().SetVisualState(elementId, value.GetValue<string>());
        }
        public void WaitForInputIdle(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            data.TryGetPropertyValue("value", out var value);
            if (value == null || value.GetValueKind() != JsonValueKind.Number)
            {
                throw new InvalidArgument("value must be integer");
            }
            GetServiceProvider().GetWindowPatternService().WaitForInputIdle(elementId, value.GetValue<int>());
        }
        public void CloseWindow(string elementId)
        {
            GetCurrentWindowHdl();
            GetServiceProvider().GetWindowPatternService().Close(elementId);
        }
        public RectResponse GetWindowRect(string elementId)
        {
            GetCurrentWindowHdl();
            return GetServiceProvider().GetWindowPatternService().GetRect(elementId);
        }
        public void BringWindowToTop(string elementId)
        {
            GetCurrentWindowHdl();
            GetServiceProvider().GetWindowPatternService().BringToTop(elementId);
        }
        public void SetWindowRect(string elementId, JsonObject data)
        {
            GetCurrentWindowHdl();
            var req = SetRectRequest.Validate(data);
            GetServiceProvider().GetWindowPatternService().SetWindowRect(elementId, req);
        }
    }
}
