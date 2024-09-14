﻿using Interop.UIAutomationClient;
using System.Text.RegularExpressions;
using UIADriver.exception;
using UIADriver.win32native;

namespace UIADriver.uia3.session
{
    public class InjectByWindowNameSession : MultipleWindowsSession
    {
        public InjectByWindowNameSession(SessionCapabilities capabilities) : base(capabilities)
        {
            if (capabilities.windowNameRegex == null) throw new SessionNotStartException("Session cannot be created. Cannot find window with name match regex " + capabilities.windowNameRegex);

            var rootElement = automation.GetRootElement();
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NamePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ProcessIdPropertyId);
            var walker = automation.CreateTreeWalker(automation.CreateTrueCondition());

            List<Tuple<int, int, IUIAutomationElement>> foundWindows = [];
            var element = walker.GetFirstChildElementBuildCache(rootElement, cacheRequest);
            while (element != null)
            {
                var rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
                var windowName = (string)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_NamePropertyId);
                var nativeHdl = (int)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
                var pid = (int)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId);

                if (!Win32Methods.IsIconic(nativeHdl) && !double.IsInfinity(rect[2]) && rect[2] != 0)
                {
                    var match = Regex.Match(windowName, capabilities.windowNameRegex);
                    if (match.Success && match.Value.Length == windowName.Length)
                    {
                        foundWindows.Add(Tuple.Create(nativeHdl, pid, element));
                    }
                }
                element = walker.GetNextSiblingElementBuildCache(element, cacheRequest);
            }

            if (foundWindows.Count == 0) throw new SessionNotStartException("Session cannot be created. Cannot find any window");
            GetWindowManageService().InitPids(foundWindows.Select(item => item.Item2).ToList());
            GetWindowManageService().InitCurrentWnd(foundWindows[0].Item3);
            Utilities.BringWindowToTop(foundWindows[0].Item1);
            foundWindows[0].Item3.SetFocus();
        }

        public override Task CloseSession()
        {
            return Task.Run(() => { });
        }
    }
}