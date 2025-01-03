using System.Windows.Automation;
using System.Xml.Linq;
using UIADriver.services;
using UIADriver.win32native;

namespace UIADriver.uia2.sourcebuilder
{
    public class RootPageSourceBuilder : PageSourceBuilder
    {
        public RootPageSourceBuilder(SessionCapabilities capabilities, ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(capabilities, serviceProvider) { }

        public override PageSource BuildPageSource(AutomationElement startElement)
        {
            var walker = new TreeWalker(Condition.TrueCondition);
            var cacheRequest = new CacheRequest();
            foreach (var item in GetPropertyList())
            {
                cacheRequest.Add(item);
            }

            Dictionary<XElement, AutomationElement> mapping = [];
            var rootUpdated = startElement.GetUpdatedCache(cacheRequest);
            XElement root = createXElement(rootUpdated);
            mapping.Add(root, rootUpdated);
            buildRecursive(root, mapping, rootUpdated, walker, cacheRequest, 2);
            return new PageSource(new XDocument(root), mapping);
        }

        protected override void buildRecursive(XElement parent, Dictionary<XElement, AutomationElement> mapping, AutomationElement element, TreeWalker walker, CacheRequest request, int layer)
        {
            if (layer > capabilities.maxTreeDepth) return;
            var elementNode = walker.GetFirstChild(element, request);

            while (elementNode != null)
            {
                try
                {
                    if (Win32Methods.IsIconic(elementNode.Cached.NativeWindowHandle))
                    {
                        elementNode = walker.GetNextSibling(elementNode, request);
                        continue;
                    }

                    XElement elementXml = createXElement(elementNode);
                    if (elementXml != null)
                    {
                        parent.Add(elementXml);
                        mapping[elementXml] = elementNode;
                        buildRecursive(elementXml, mapping, elementNode, walker, request, layer + 1);
                    }
                }
                catch {}

                elementNode = walker.GetNextSibling(elementNode, request);
            }
        }

        public override List<AutomationElement> FindElementByProperty(AutomationElement topLevelWindow, string propertyName, string? propertyValue, bool stopAtFirst)
        {
            var property = UIA2PropertyDictionary.GetAutomationProperty(propertyName);
            List<AutomationElement> rs = [];
            var walker = new TreeWalker(Condition.TrueCondition);
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(property);
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);

            try
            {
                var updated = topLevelWindow.GetUpdatedCache(cacheRequest);
                findElementByPropertyRecursive(topLevelWindow, propertyName, propertyValue, stopAtFirst, 1, walker, cacheRequest, rs);
            } catch { }
            
            return rs;
        }

        protected override void findElementByPropertyRecursive(AutomationElement element, string propertyName, string? propertyValue, bool stopAtFirst, int layer, TreeWalker walker, CacheRequest request, List<AutomationElement> rs)
        {
            if (layer > capabilities.maxTreeDepth) return;

            var propValue = serviceProvider.GetElementAttributeService().GetAttributeString(element, propertyName, false);
            if (propertyValue == propValue || propValue != null && propValue.Equals(propertyValue))
            {
                rs.Add(element);
                if (stopAtFirst) return;
            }

            var child = walker.GetFirstChild(element, request);
            while (child != null)
            {
                if (layer == 1)
                {
                    try
                    {
                        if (Win32Methods.IsIconic(child.Cached.NativeWindowHandle))
                        {
                            child = walker.GetNextSibling(child);
                            continue;
                        }
                    }
                    catch { }
                }

                findElementByPropertyRecursive(child, propertyName, propertyValue, stopAtFirst, layer + 1, walker, request, rs);
                if (rs.Count > 0 && stopAtFirst) return;

                child = walker.GetNextSibling(child, request);
            }
        }
    }
}
