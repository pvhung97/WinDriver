using System.Management;
using System.Text;
using System.Text.Json.Nodes;
using UIADriver.dto.response;

namespace UIADriver
{
    public abstract class Session
    {
        protected SessionCapabilities capabilities;
        protected HashSet<int> pids = [];

        protected ScreenshotCapture screenCapture;
        
        public Session(SessionCapabilities capabilities)
        {
            this.capabilities = capabilities;
            this.screenCapture = new ScreenshotCapture();
        }

        protected void UpdateProcessList()
        {
            if (pids.Count == 0) return;
            var queryBuilder = new StringBuilder("SELECT ProcessId, ParentProcessId FROM Win32_Process WHERE ");
            int i = 0;
            foreach (var item in pids)
            {
                if (i > 0) queryBuilder.Append(" OR ");
                queryBuilder.Append("(ParentProcessId = " + item + ")");
                i++;
            }
            string query = queryBuilder.ToString();
            var newPids = new HashSet<int>(pids, pids.Comparer);
            using (var searcher = new ManagementObjectSearcher(query))
            {
                using var results = searcher.Get();
                if (results == null || results.Count == 0) return;

                foreach (ManagementObject obj in results.Cast<ManagementObject>())
                {
                    int pid = Convert.ToInt32((uint)obj["ProcessId"]);
                    newPids.Add(pid);
                }
            }
            pids = newPids;
        }

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
