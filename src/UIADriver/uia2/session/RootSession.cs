using System.Text.Json.Nodes;
using System.Windows.Automation;
using UIADriver.actions;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.uia2.sourcebuilder;

namespace UIADriver.uia2.session
{
    public class RootSession : UIA2Session
    {
        protected override PageSourceBuilder PageSourceBuilder => new RootPageSourceBuilder(attrGetter, capabilities);

        public RootSession(SessionCapabilities capabilities) : base(capabilities) { }

        public override Task CloseSession()
        {

            return Task.Run(() => { });
        }

        public override HashSet<string> CollectWindowHandles()
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            var rootElement = AutomationElement.RootElement.GetUpdatedCache(cacheRequest);
            return new HashSet<string>() { rootElement.Cached.NativeWindowHandle.ToString() };
        }

        protected override AutomationElement getCurrentWindow(CacheRequest? cacheRequest)
        {
            return cacheRequest == null ? AutomationElement.RootElement : AutomationElement.RootElement.GetUpdatedCache(cacheRequest);
        }

        protected override AutomationElement getCurrentWindowThenFocus(CacheRequest? cacheRequest)
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
            var rect = element.Cached.BoundingRectangle;
            return new RectResponse((int)rect.X, (int)rect.Y, double.IsInfinity(rect.Width) ? 0 : (int)rect.Width, double.IsInfinity(rect.Height) ? 0 : (int)rect.Height);
        }

        protected override ActionOptions getActionOption()
        {
            return new RootSessionActionOptions(getCurrentWindow(null), elementFinder);
        }
    }
}
