using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3WindowPattern : WindowPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3WindowPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override bool CanMaximize(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_WindowCanMaximizePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationWindowPattern)element.GetCachedPattern(UIA_PatternIds.UIA_WindowPatternId);
            return pattern.CachedCanMaximize != 0;
        }

        public override bool CanMinimize(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_WindowCanMinimizePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationWindowPattern)element.GetCachedPattern(UIA_PatternIds.UIA_WindowPatternId);
            return pattern.CachedCanMinimize != 0;
        }

        public override void Close(string elementId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationWindowPattern)element.GetCachedPattern(UIA_PatternIds.UIA_WindowPatternId);
            pattern.Close();
        }

        public override string GetInteractionState(string elementId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var state = attributeService.GetAttributeString(element, nameof(UIA3PropertyEnum.Window_WindowInteractionState));
            return state == null ? "" : state;
        }

        public override RectResponse GetRect(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            double[] rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            return new RectResponse((int)rect[0], (int)rect[1], double.IsInfinity(rect[2]) ? 0 : (int)rect[2], double.IsInfinity(rect[3]) ? 0 : (int)rect[3]);
        }

        public override string GetVisualState(string elementId)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var state = attributeService.GetAttributeString(element, nameof(UIA3PropertyEnum.Window_WindowVisualState));
            return state == null ? "" : state;
        }

        public override bool IsModal(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_WindowIsModalPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationWindowPattern)element.GetCachedPattern(UIA_PatternIds.UIA_WindowPatternId);
            return pattern.CachedIsModal != 0;
        }

        public override bool IsTopmost(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_WindowIsTopmostPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationWindowPattern)element.GetCachedPattern(UIA_PatternIds.UIA_WindowPatternId);
            return pattern.CachedIsModal != 0;
        }

        public override void SetVisualState(string elementId, string state)
        {
            WindowVisualState visualState = WindowVisualState.WindowVisualState_Normal;
            switch (state.ToLower())
            {
                case "normal":
                    visualState = WindowVisualState.WindowVisualState_Normal;
                    break;
                case "maximized":
                    visualState = WindowVisualState.WindowVisualState_Maximized;
                    break;
                case "minimized":
                    visualState = WindowVisualState.WindowVisualState_Minimized;
                    break;
                default:
                    throw new InvalidArgument("Invalid visual state");
            }
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationWindowPattern)element.GetCachedPattern(UIA_PatternIds.UIA_WindowPatternId);
            pattern.SetWindowVisualState(visualState);
        }

        public override void WaitForInputIdle(string elementId, int timeout)
        {
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationWindowPattern)element.GetCachedPattern(UIA_PatternIds.UIA_WindowPatternId);
            pattern.WaitForInputIdle(timeout);
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsWindowPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_WindowPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsWindowPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Window pattern is not available for this element");
            }

            return element;
        }

        protected override nint GetHandle(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            return element.CachedNativeWindowHandle;
        }
    }
}
