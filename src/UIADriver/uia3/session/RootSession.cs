
using Interop.UIAutomationClient;
using System.Text.Json.Nodes;
using UIA3Driver;
using UIA3Driver.actions;
using UIA3Driver.dto.response;
using UIA3Driver.exception;
using UIADriver.uia3.sourcebuilder;

namespace UIADriver.uia3.session
{
    public class RootSession : UIA3Session
    {
        protected override PageSourceBuilder PageSourceBuilder => new RootPageSourceBuilder(automation, attrGetter, capabilities);

        public RootSession(SessionCapabilities capabilities) : base(capabilities) { }

        public override Task CloseSession()
        {
            
            return Task.Run(() => { });
        }

        public override HashSet<string> CollectWindowHandles()
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            var rootElement = automation.GetRootElementBuildCache(cacheRequest);
            return new HashSet<string>() { ((int)rootElement.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId)).ToString() };
        }

        protected override IUIAutomationElement getCurrentWindow(IUIAutomationCacheRequest? cacheRequest)
        {
            return cacheRequest == null ? automation.GetRootElement() : automation.GetRootElementBuildCache(cacheRequest);
        }

        protected override IUIAutomationElement getCurrentWindowThenFocus(IUIAutomationCacheRequest? cacheRequest)
        {
            return getCurrentWindow(cacheRequest);
        }

        public override RectResponse MinimizeCurrentWindow()
        {
            throw new UnsupportedOperation("Cannot minimize in root session");
        }

        public override RectResponse MaximizeCurrentWindow()
        {
            throw new UnsupportedOperation("Cannot maximize in root session");
        }

        public override RectResponse SetWindowRect(JsonObject data)
        {
            throw new UnsupportedOperation("Cannot set window rect in root session");
        }

        public override HashSet<string> CloseCurrentWindow()
        {
            throw new UnsupportedOperation("Cannot close window in root session");
        }

        public override void SwitchToWindow(JsonObject windowHandle)
        {
            throw new UnsupportedOperation("Cannot switch window on root sessopm");
        }

        public override FindElementResponse GetActiveElement()
        {
            return elementFinder.GetActiveElement();
        }

        public override RectResponse GetElementRect(string id)
        {
            var element = elementFinder.GetElement(id);
            double[] rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            return new RectResponse((int)rect[0], (int)rect[1], double.IsInfinity(rect[2]) ? 0 : (int)rect[2], double.IsInfinity(rect[3]) ? 0 : (int)rect[3]);
        }

        protected override ActionOptions getActionOption()
        {
            return new RootSessionActionOptions(automation, getCurrentWindow(null), elementFinder);
        }
    }
}
