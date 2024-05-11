using System.Text.Json.Nodes;
using UIADriver.dto.response;
using UIADriver.services;

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
    }
}
