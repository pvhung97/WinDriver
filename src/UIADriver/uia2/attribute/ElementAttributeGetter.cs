using System.Windows.Automation;

namespace UIADriver.uia2.attribute
{
    public class ElementAttributeGetter
    {
        public BasicElementPropertyValueGetter basicAttr = new BasicElementPropertyValueGetter();

        public string? getAttribute(AutomationElement element, string attribute)
        {
            var found = UIA2PropertyDictionary.GetAutomationProperty(attribute);
            if (found == null) return null;

            var cacheRequest = new CacheRequest();
            cacheRequest.Add(found);
            element = element.GetUpdatedCache(cacheRequest);
            return basicAttr.GetPropertyStrValue(element, found);
        }
    }
}
