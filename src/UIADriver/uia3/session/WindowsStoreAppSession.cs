using Interop.UIAutomationClient;
using System.Text;
using UIADriver.exception;
using UIADriver.win32native;

namespace UIADriver.uia3.session
{
    public class WindowsStoreAppSession : MultipleWindowsSession
    {
        public WindowsStoreAppSession(SessionCapabilities capabilities) : base(capabilities)
        {
            if (capabilities.aumid == null || capabilities.aumid.Length == 0) throw new SessionNotStartException("Session cannot be created. Cannot launch app with aumid " + capabilities.aumid);
            var launcher = new ApplicationActivationManager();
            var argumentBuilder = new StringBuilder();
            foreach (var argument in capabilities.appArgument)
            {
                argumentBuilder.Append("\"");
                string escaped = argument.Replace("\\", "\\\\").Replace("\"", "\\\"");
                argumentBuilder.Append(escaped);
                argumentBuilder.Append("\" ");
            }
            launcher.ActivateApplication(capabilities.aumid, argumentBuilder.ToString(), ActivateOptions.None, out var pidW32);
            if (pidW32 == 0) throw new SessionNotStartException("Session cannot be created. Cannot launch app with aumid " + capabilities.aumid);
            Thread.Sleep(capabilities.delayAfterOpenApp);
            int pid = Convert.ToInt32(pidW32);
            pids.Add(pid);
            UpdateProcessList();
            var foundWindow = SearchForWindowLaunchedByApp();
            if (foundWindow == null) throw new SessionNotStartException("Session cannot be created. Cannot find any window");
            pids.Add(foundWindow.pid);
            currentHdl = foundWindow.hdl;
            Utilities.BringWindowToTop(currentHdl);
        }

        private WndHdlAndPid? SearchForWindowLaunchedByApp()
        {
            var rootElement = automation.GetRootElement();
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ProcessIdPropertyId);
            var walker = automation.CreateTreeWalker(automation.CreateTrueCondition());

            var element = walker.GetFirstChildElementBuildCache(rootElement, cacheRequest);
            while (element != null)
            {
                var rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
                var nativeHdl = (int)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
                var pid = (int)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId);

                if (!Win32Methods.IsIconic(nativeHdl) && !double.IsInfinity(rect[2]) && rect[2] != 0)
                {
                    if (pids.Contains(pid)) return new WndHdlAndPid(nativeHdl, pid, element);

                    var childPointer = walker.GetFirstChildElementBuildCache(element, cacheRequest);
                    while (childPointer != null)
                    {
                        var pidChild = (int)childPointer.GetCachedPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId);

                        if (pids.Contains(pidChild)) return new WndHdlAndPid(nativeHdl, pid, element);

                        childPointer = walker.GetNextSiblingElementBuildCache(childPointer, cacheRequest);
                    }

                }


                element = walker.GetNextSiblingElementBuildCache(element, cacheRequest);
            }
            return null;
        }

    }
}
