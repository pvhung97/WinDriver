using Interop.UIAutomationClient;

namespace UIADriver.attribute
{
    public class ElementAttributeGetter
    {
        public BasicElementPropertyValueGetter basicAttr = new BasicElementPropertyValueGetter();

        public string? getAttribute(IUIAutomation automation, IUIAutomationElement element, string attribute)
        {
            var found = Enum.TryParse<UIAPropertyEnum>(attribute, out var propertyId);
            if (found == false) return null;

            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty((int)propertyId);
            element = element.BuildUpdatedCache(cacheRequest);
            return basicAttr.GetPropertyStrValue(element, (int)propertyId);
        }
    }
}
