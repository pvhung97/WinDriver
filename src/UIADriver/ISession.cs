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

    }
}
