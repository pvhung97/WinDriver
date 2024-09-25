using Interop.UIAutomationClient;
using System.Xml.Linq;
using System.Xml.XPath;
using UIADriver.services;
using UIADriver.win32native;

namespace UIADriver.uia3.sourcebuilder
{
    public class RootPageSourceBuilder : PageSourceBuilder
    {
        public RootPageSourceBuilder(IUIAutomation automation, SessionCapabilities capabilities, ElementAttributeService<IUIAutomationElement> attrService) : base(automation, capabilities, attrService) { }

        public override PageSource BuildPageSource(IUIAutomationElement startElement)
        {
            var walker = automation.CreateTreeWalker(automation.CreateTrueCondition());
            var cacheRequest = automation.CreateCacheRequest();
            foreach (var item in GetPropertyList())
            {
                cacheRequest.AddProperty(item);
            }

            Dictionary<XElement, IUIAutomationElement> mapping = [];
            var rootUpdated = startElement.BuildUpdatedCache(cacheRequest);
            XElement root = createXElement(rootUpdated);
            mapping.Add(root, rootUpdated);
            buildRecursive(root, mapping, rootUpdated, walker, cacheRequest, 2);
            return new PageSource(new XDocument(root), mapping);
        }

        protected override void buildRecursive(XElement parent, Dictionary<XElement, IUIAutomationElement> mapping, IUIAutomationElement element, IUIAutomationTreeWalker walker, IUIAutomationCacheRequest request, int layer)
        {
            if (layer > capabilities.maxTreeDepth) return;
            var elementNode = walker.GetFirstChildElementBuildCache(element, request);

            while (elementNode != null)
            {
                try
                {
                    int hwnd = (int)elementNode.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
                    if (hwnd != 0 && Win32Methods.IsIconic(hwnd))
                    {
                        elementNode = walker.GetNextSiblingElementBuildCache(elementNode, request);
                        continue;
                    }

                    //  Layer = 2 means parent is automation root
                    if (layer > 2 && checkIfElementCanCauseInfiniteLoop(elementNode, walker))
                    {
                        break;
                    }

                    XElement elementXml = createXElement(elementNode);
                    if (elementXml != null)
                    {
                        parent.Add(elementXml);
                        mapping[elementXml] = elementNode;
                        buildRecursive(elementXml, mapping, elementNode, walker, request, layer + 1);
                    }
                }
                catch { }

                elementNode = walker.GetNextSiblingElementBuildCache(elementNode, request);
            }
        }

        public override List<IUIAutomationElement> FindElementByProperty(IUIAutomationElement topLevelWindow, string propertyName, string? propertyValue, bool stopAtFirst)
        {
            int propertyId = (int)Enum.Parse(typeof(UIA3PropertyEnum), propertyName);
            List<IUIAutomationElement> rs = [];
            var walker = automation.CreateTreeWalker(automation.CreateTrueCondition());
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(propertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsSelectionPatternAvailablePropertyId);

            try
            {
                var updated = topLevelWindow.BuildUpdatedCache(cacheRequest);
                findElementByPropertyRecursive(updated, propertyName, propertyValue, stopAtFirst, 1, walker, cacheRequest, rs);
            } catch { }

            return rs;
        }

        protected override void findElementByPropertyRecursive(IUIAutomationElement element, string propertyName, string? propertyValue, bool stopAtFirst, int layer, IUIAutomationTreeWalker walker, IUIAutomationCacheRequest request, List<IUIAutomationElement> rs)
        {
            if (layer > capabilities.maxTreeDepth) return;

            var propValue = attrService.GetAttributeString(element, propertyName, false);
            if (propertyValue == propValue || propValue != null && propValue.Equals(propertyValue))
            {
                rs.Add(element);
                if (stopAtFirst) return;
            }

            var child = walker.GetFirstChildElementBuildCache(element, request);
            while (child != null)
            {
                int hwnd = (int)child.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
                try
                {
                    if (hwnd != 0 && Win32Methods.IsIconic(hwnd))
                    {
                        child = walker.GetNextSiblingElementBuildCache(child, request);
                        continue;
                    }

                    //  Layer = 1 means element is automation root
                    if (layer > 1 && checkIfElementCanCauseInfiniteLoop(child, walker))
                    {
                        break;
                    }
                }
                catch { }
                
                findElementByPropertyRecursive(child, propertyName, propertyValue, stopAtFirst, layer + 1, walker, request, rs);
                if (rs.Count > 0 && stopAtFirst) return;

                child = walker.GetNextSiblingElementBuildCache(child, request);
            }
        }
    }
}
