using Interop.UIAutomationClient;
using System.Xml.Linq;
using UIA3Driver;
using UIA3Driver.attribute;

namespace UIADriver.uia3.sourcebuilder
{
    public abstract class PageSourceBuilder
    {
        protected IUIAutomation automation;
        protected SessionCapabilities capabilities;
        protected ElementAttributeGetter attributeGetter;

        public PageSourceBuilder(IUIAutomation automation, ElementAttributeGetter attributeGetter, SessionCapabilities capabilities)
        {
            this.automation = automation;
            this.attributeGetter = attributeGetter;
            this.capabilities = capabilities;
        }

        public abstract PageSource buildPageSource(IUIAutomationElement startElement);
        protected abstract void buildRecursive(XElement parent, Dictionary<XElement, IUIAutomationElement> mapping, IUIAutomationElement element, IUIAutomationTreeWalker walker, IUIAutomationCacheRequest request, int layer);
        protected abstract List<int> getPropertyList();
        public abstract List<IUIAutomationElement> findElementByProperty(IUIAutomationElement topLevelWindow, int propertyId, string? propertyValue, bool stopAtFirst);
        protected abstract void findElementByPropertyRecursive(IUIAutomationElement element, int propertyId, string? propertyValue, bool stopAtFirst, int layer, IUIAutomationTreeWalker walker, IUIAutomationCacheRequest request, List<IUIAutomationElement> rs);

        public class PageSource
        {
            public XDocument pageSource;
            public Dictionary<XElement, IUIAutomationElement> mapping;

            public PageSource(XDocument pageSource, Dictionary<XElement, IUIAutomationElement> mapping)
            {
                this.pageSource = pageSource;
                this.mapping = mapping;
            }
        }
    }
}
