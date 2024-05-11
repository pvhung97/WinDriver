using System.Windows.Automation;
using System.Xml.Linq;
using UIADriver.services;

namespace UIADriver.uia2.sourcebuilder
{
    public abstract class PageSourceBuilder : PageSourceService<AutomationElement>
    {
        public PageSourceBuilder(SessionCapabilities capabilities, ElementAttributeService<AutomationElement> attributeService) : base(capabilities, attributeService) { }

        protected abstract void buildRecursive(XElement parent, Dictionary<XElement, AutomationElement> mapping, AutomationElement element, TreeWalker treeWalker, CacheRequest cacheRequest, int layer);
        protected abstract List<AutomationProperty> getPropertyList();
        protected abstract void findElementByPropertyRecursive(AutomationElement element, string propertyName, string? propertyValue, bool stopAtFirst, int layer, TreeWalker walker, CacheRequest request, List<AutomationElement> rs);
    }
}
