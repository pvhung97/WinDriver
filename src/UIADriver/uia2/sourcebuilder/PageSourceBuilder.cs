using System.Windows.Automation;
using System.Xml.Linq;
using UIADriver.uia2.attribute;

namespace UIADriver.uia2.sourcebuilder
{
    public abstract class PageSourceBuilder
    {
        protected SessionCapabilities capabilities;
        protected ElementAttributeGetter attributeGetter;

        public PageSourceBuilder(SessionCapabilities capabilities, ElementAttributeGetter attributeGetter)
        {
            this.capabilities = capabilities;
            this.attributeGetter = attributeGetter;
        }

        public abstract PageSource buildPageSource(AutomationElement startElement);
        protected abstract void buildRecursive(XElement parent, Dictionary<XElement, AutomationElement> mapping, AutomationElement element, TreeWalker treeWalker, CacheRequest cacheRequest, int layer);
        protected abstract List<AutomationProperty> getPropertyList();
        public abstract List<AutomationElement> findElementByProperty(AutomationElement topLevelWindow, AutomationProperty propertyId, string? propertyValue, bool stopAtFirst);
        protected abstract void findElementByPropertyRecursive(AutomationElement element, AutomationProperty propertyId, string? propertyValue, bool stopAtFirst, int layer, TreeWalker walker, CacheRequest request, List<AutomationElement> rs);

        public class PageSource
        {
            public XDocument pageSource;
            public Dictionary<XElement, AutomationElement> mapping;

            public PageSource(XDocument pageSource, Dictionary<XElement, AutomationElement> mapping)
            {
                this.pageSource = pageSource;
                this.mapping = mapping;
            }
        }
    }
}
