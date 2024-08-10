using Interop.UIAutomationClient;
using UIADriver.exception;
using UIADriver.win32native;

namespace UIADriver.uia3.session
{
    public class InjectWindowSession : MultipleWindowsSession
    {
        public InjectWindowSession(SessionCapabilities capabilities) : base(capabilities)
        {
            if (capabilities.nativeWindowHandle == null) throw new SessionNotStartException("Session cannot be created. Cannot find window with handle " + capabilities.aumid);

            var rootElement = automation.GetRootElement();
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ProcessIdPropertyId);
            var walker = automation.CreateTreeWalker(automation.CreateTrueCondition());

            Tuple<int, int, IUIAutomationElement>? foundWindow = null;
            var element = walker.GetFirstChildElementBuildCache(rootElement, cacheRequest);
            while (element != null)
            {
                var rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
                var nativeHdl = (int)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
                var pid = (int)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId);

                if (!Win32Methods.IsIconic(nativeHdl) && !double.IsInfinity(rect[2]) && rect[2] != 0 && nativeHdl == capabilities.nativeWindowHandle)
                {
                    foundWindow = Tuple.Create(nativeHdl, pid, element);
                    break;
                }

                element = walker.GetNextSiblingElementBuildCache(element, cacheRequest);
            }

            if (foundWindow == null) throw new SessionNotStartException("Session cannot be created. Cannot find any window");
            GetWindowManageService().InitPids(foundWindow.Item2);
            GetWindowManageService().InitCurrentWnd(foundWindow.Item3);
            Utilities.BringWindowToTop(foundWindow.Item1);
            foundWindow.Item3.SetFocus();
        }

        public override Task CloseSession()
        {
            return Task.Run(() => { });
        }

    }
    
}
