using System.Text.Json.Nodes;
using UIADriver.dto.response;

namespace UIADriver
{
    public interface ISession
    {
        //  W3C API
        public HashSet<string> CollectWindowHandles();
        public Task CloseSession();
        public string GetCurrentWindowTitle();
        public string GetCurrentWindowHdl();
        public RectResponse GetCurrentWindowRect();
        public RectResponse MinimizeCurrentWindow();
        public RectResponse MaximizeCurrentWindow();
        public RectResponse SetWindowRect(JsonObject data);
        public HashSet<string> CloseCurrentWindow();
        public void SwitchToWindow(JsonObject windowHandle);
        public string GetScreenshot();
        public string GetPageSource();
        public FindElementResponse FindElement(JsonObject data);
        public List<FindElementResponse> FindElements(JsonObject data);
        public FindElementResponse FindElementFromElement(string elementId, JsonObject data);
        public List<FindElementResponse> FindElementsFromElement(string elementId, JsonObject data);
        public FindElementResponse GetActiveElement();
        public string? GetElementAttribute(string id, string attribute);
        public string GetElementTagName(string id);
        public RectResponse GetElementRect(string id);
        public string GetElementText(string id);
        public bool IsElementEnabled(string id);
        public bool IsElementSelected(string id);
        public bool IsElementDisplayed(string id);
        public Task PerformActions(JsonObject action);
        public Task ReleaseActions();
        public Task ElementClick(string elementId);
        public void ElementClear(string elementId);
        public Task ElementSendKeys(string elementId, JsonObject data);
        public string GetElementScreenshot(string elementId);

        //  Element pattern API
        public FindElementResponse GetAnnotationTarget(string elementId);
        public FindElementResponse GetLabeledBy(string elementId);
        public List<FindElementResponse> GetControllerFor(string elementId);
        public List<FindElementResponse> GetDescribedBy(string elementId);
        public List<FindElementResponse> GetFlowsTo(string elementId);
        public void SetFocus(string elementId);
        public FindElementResponse NavigateFollowDirection(string elementId, JsonObject data);
        public void SetDockPosition(string elementId, JsonObject data);
        public List<string> GetDropEffects(string elementId);
        public List<FindElementResponse> GetGrabbedItems(string elementId);
        public List<string> GetDropTargetEffects(string elementId);
        public void ExpandOrCollapseElement(string elementId, JsonObject data);
        public FindElementResponse GetContainingGrid(string elementId);
        public FindElementResponse GetGridItem(string elementId, JsonObject data);
        public void Invoke(string elementId);
        public List<int> GetSupportedViewIds(string elementId);
        public string GetViewName(string elementId, string viewName);
        public void SetCurrentView(string elementId, JsonObject data);
        public void SetRangeValue(string elementId, JsonObject data);
        public void ScrollIntoView(string elementId);
        public void Scroll(string elementId, JsonObject data);
        public void SetScrollPercent(string elementId, JsonObject data);
        public void Select(string elementId);
        public void AddToSelection(string elementId);
        public void RemoveFromSelection(string elementId);
        public FindElementResponse GetSelectionContainer(string elementId);
        public FindElementResponse GetFirstSelectedItem(string elementId);
        public FindElementResponse GetLastSelectedItem(string elementId);
        public FindElementResponse GetCurrentSelectedItem(string elementId);
        public List<FindElementResponse> GetAnnotationObjects(string elementId);
        public FindElementResponse GetSpreadSheetItemByName(string elementId, string name);
        public List<FindElementResponse> GetRowHeaderItems(string elementId);
        public List<FindElementResponse> GetColumnHeaderItems(string elementId);
        public List<FindElementResponse> GetRowHeaders(string elementId);
        public List<FindElementResponse> GetColumnHeader(string elementId);
        public void Toggle(string elementId);
        public void Zoom(string elementId, JsonObject data);
        public void ZoomByUnit(string elementId, JsonObject data);
        public void Move(string elementId, JsonObject data);
        public void Resize(string elementId, JsonObject data);
        public void Rotate(string elementId, JsonObject data);
        public void SetValue(string elementId, JsonObject data);
        public void Realize(string elementId);
        public void SetVisualState(string elementId, JsonObject data);
        public void WaitForInputIdle(string elementId, JsonObject data);
        public void CloseWindow(string elementId);
        public RectResponse GetWindowRect(string elementId);
        public void BringWindowToTop(string elementId);
        public void SetWindowRect(string elementId, JsonObject data);
    }
}
