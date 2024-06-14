using System.Windows.Automation;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2WindowPattern : WindowPatternService<AutomationElement, CacheRequest>
    {
        public UIA2WindowPattern(ElementFinderService<AutomationElement, CacheRequest> finderService, ElementAttributeService<AutomationElement> attributeService) : base(finderService, attributeService) { }

        public override void Close(string elementId)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (WindowPattern)element.GetCachedPattern(WindowPattern.Pattern);
            pattern.Close();
        }

        public override RectResponse GetRect(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            var element = AssertPattern(elementId, cacheRequest);
            var rect = element.Cached.BoundingRectangle;
            return new RectResponse((int)rect.X, (int)rect.Y, double.IsInfinity(rect.Width) ? 0 : (int)rect.Width, double.IsInfinity(rect.Height) ? 0 : (int)rect.Height);
        }

        public override void SetVisualState(string elementId, string state)
        {
            WindowVisualState vs;
            switch (state.ToLower())
            {
                case "normal":
                    vs = WindowVisualState.Normal;
                    break;
                case "maximized":
                    vs = WindowVisualState.Maximized;
                    break;
                case "minimized":
                    vs = WindowVisualState.Minimized;
                    break;
                default:
                    throw new InvalidArgument("Invalid visual state");
            }
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (WindowPattern)element.GetCachedPattern(WindowPattern.Pattern);
            pattern.SetWindowVisualState(vs);
        }

        public override void WaitForInputIdle(string elementId, int timeout)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (WindowPattern)element.GetCachedPattern(WindowPattern.Pattern);
            pattern.WaitForInputIdle(timeout);
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsWindowPatternAvailableProperty);
            cacheRequest.Add(WindowPattern.Pattern);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsWindowPatternAvailableProperty))
            {
                throw new InvalidArgument("Window pattern is not available for this element");
            }
            return element;
        }

        protected override nint GetHandle(string elementId)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            var element = AssertPattern(elementId, cacheRequest);
            return element.Cached.NativeWindowHandle;
        }
    }
}
