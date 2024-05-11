using System.Management;
using UIADriver.dto.request;
using UIADriver.dto.response;

namespace UIADriver.services
{
    public abstract class WindowManageService<T, U>
    {
        protected HashSet<int> pids;
        protected ElementFinderService<T, U> elementFinder;

        public WindowManageService(ElementFinderService<T, U> elementFinder)
        {
            this.pids = [];
            this.elementFinder = elementFinder;
        }

        public abstract T getCurrentWindow(U? cacheRequest);
        public abstract T getCurrentWindowThenFocus(U? cacheRequest);
        public abstract string GetCurrentWindowTitle();
        public abstract string GetCurrentWindowHdl();
        public abstract List<string> CollectWindowHandles();
        public abstract List<string> CollectWindowHandles(bool includeIconic);
        public abstract RectResponse GetCurrentWindowRect();
        public abstract RectResponse MinimizeCurrentWindow();
        public abstract RectResponse MaximizeCurrentWindow();
        public abstract RectResponse SetWindowRect(SetRectRequest data);
        public abstract List<string> CloseCurrentWindow();
        public abstract void SwitchToWindow(SwitchWindowRequest windowHandle);
        public abstract List<WndHdlAndPid> CollectWindows();
        public abstract List<WndHdlAndPid> CollectWindows(bool includeIconic);


        public HashSet<int> InitPids(int pid)
        {
            pids.Add(pid);
            UpdateProcessList();
            return pids;
        }
        public abstract void InitCurrentWnd(T currentWnd);

        protected void UpdateProcessList()
        {
            if (pids.Count == 0) return;

            var newPids = new List<int>(pids);
            int i = 0;
            while (i < newPids.Count)
            {
                string query = "SELECT ProcessId, ParentProcessId FROM Win32_Process WHERE ParentProcessId = " + newPids[i];
                var searcher = new ManagementObjectSearcher(query);
                var results = searcher.Get();
                if (results != null && results.Count > 0)
                {
                    foreach (ManagementObject obj in results)
                    {
                        int pid = Convert.ToInt32((uint)obj["ProcessId"]);
                        if (!newPids.Contains(pid)) newPids.Add(pid);
                        obj.Dispose();
                    }

                    results.Dispose();
                }
                searcher.Dispose();

                i++;
            }
            pids = newPids.ToHashSet();
        }

        public class WndHdlAndPid
        {
            public int hdl;
            public int pid;
            public T window;

            public WndHdlAndPid(int hdl, int pid, T window)
            {
                this.hdl = hdl;
                this.pid = pid;
                this.window = window;
            }
        }
    }
}
