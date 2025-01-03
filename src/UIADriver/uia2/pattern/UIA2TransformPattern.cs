using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2TransformPattern : TransformPatternService<AutomationElement, CacheRequest>
    {
        public UIA2TransformPattern(ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(serviceProvider) { }

        public override void Move(string elementId, double x, double y)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (TransformPattern)element.GetCachedPattern(TransformPattern.Pattern);
            pattern.Move(x, y);
        }

        public override void Resize(string elementId, double width, double height)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (TransformPattern)element.GetCachedPattern(TransformPattern.Pattern);
            pattern.Resize(width, height);
        }

        public override void Rotate(string elementId, double degrees)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (TransformPattern)element.GetCachedPattern(TransformPattern.Pattern);
            pattern.Rotate(degrees);
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsTransformPatternAvailableProperty);
            cacheRequest.Add(TransformPattern.Pattern);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsTransformPatternAvailableProperty))
            {
                throw new InvalidArgument("Transform pattern is not available for this element");
            }
            return element;
        }
    }
}
