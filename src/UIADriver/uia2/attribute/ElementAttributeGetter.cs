using System.Windows.Automation;
using UIADriver.dto.response;
using UIADriver.services;

namespace UIADriver.uia2.attribute
{
    public class ElementAttributeGetter : ElementAttributeService<AutomationElement>
    {
        public BasicElementPropertyValueGetter basicAttr = new BasicElementPropertyValueGetter();

        public override object? GetAttributeObject(AutomationElement element, string attribute)
        {
            var found = UIA2PropertyDictionary.GetAutomationProperty(attribute);
            if (found == null) return null;

            var cacheRequest = new CacheRequest();
            cacheRequest.Add(found);
            element = element.GetUpdatedCache(cacheRequest);
            return element.GetCachedPropertyValue(found);
        }

        public override string? GetAttributeString(AutomationElement element, string attribute)
        {
            return GetAttributeString(element, attribute, true);
        }

        public override string? GetAttributeString(AutomationElement element, string attribute, bool updateCache)
        {
            var found = UIA2PropertyDictionary.GetAutomationProperty(attribute);
            if (found == null) return null;

            if (updateCache)
            {
                var cacheRequest = new CacheRequest();
                cacheRequest.Add(found);
                element = element.GetUpdatedCache(cacheRequest);
            }
            
            return basicAttr.GetPropertyStrValue(element, found);
        }

        public override RectResponse GetElementRect(AutomationElement element, Point rootPoint)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            element = element.GetUpdatedCache(cacheRequest);

            var rect = element.Cached.BoundingRectangle;
            return new RectResponse((int)rect.X - rootPoint.X, (int)rect.Y - rootPoint.Y, double.IsInfinity(rect.Width) ? 0 : (int)rect.Width, double.IsInfinity(rect.Height) ? 0 : (int)rect.Height);
        }

        public override string GetElementTagName(AutomationElement element)
        {
#pragma warning disable CS8604
            var tagName = GetAttributeString(element, UIA2PropertyDictionary.GetAutomationPropertyName(AutomationElement.ControlTypeProperty.Id));
            return tagName == null ? "Unknown" : tagName;
#pragma warning restore CS8604
        }

        public override string GetElementText(AutomationElement element)
        {
            var treeWalker = new TreeWalker(Condition.TrueCondition);
            var firstChild = treeWalker.GetFirstChild(element);
            if (firstChild != null) return "";
#pragma warning disable CS8604
            var name = GetAttributeString(element, UIA2PropertyDictionary.GetAutomationPropertyName(AutomationElement.NameProperty.Id));
            return name == null ? "" : name;
#pragma warning restore CS8604
        }

        public override bool IsElementDisplayed(AutomationElement element)
        {
            var cachedRequest = new CacheRequest();
            cachedRequest.Add(AutomationElement.BoundingRectangleProperty);
            cachedRequest.Add(AutomationElement.IsOffscreenProperty);
            var updatedElement = element.GetUpdatedCache(cachedRequest);

            var boundingRect = updatedElement.Cached.BoundingRectangle;
            if (updatedElement.Cached.IsOffscreen || boundingRect.Width == 0 || boundingRect.Height == 0) return false;
            return true;
        }

        public override bool IsElementEnabled(AutomationElement element)
        {
#pragma warning disable CS8604
            var isEnabled = GetAttributeObject(element, UIA2PropertyDictionary.GetAutomationPropertyName(AutomationElement.IsEnabledProperty.Id));
            return isEnabled == null ? false : (bool)isEnabled;
#pragma warning restore CS8604
        }

        public override bool IsElementSelected(AutomationElement element)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.IsSelectionItemPatternAvailableProperty);
            cacheRequest.Add(SelectionItemPattern.IsSelectedProperty);
            cacheRequest.Add(AutomationElement.IsTogglePatternAvailableProperty);
            cacheRequest.Add(TogglePattern.ToggleStateProperty);
            element = element.GetUpdatedCache(cacheRequest);

            if ((bool)element.GetCachedPropertyValue(AutomationElement.IsSelectionItemPatternAvailableProperty))
            {
                return (bool)element.GetCachedPropertyValue(SelectionItemPattern.IsSelectedProperty);
            }

            if ((bool)element.GetCachedPropertyValue(AutomationElement.IsTogglePatternAvailableProperty))
            {
                return (ToggleState)element.GetCachedPropertyValue(TogglePattern.ToggleStateProperty) == ToggleState.On;
            }

            return false;
        }
    }
}
