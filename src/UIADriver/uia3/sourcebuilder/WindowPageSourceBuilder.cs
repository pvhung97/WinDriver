using Interop.UIAutomationClient;
using System.Xml.Linq;
using UIADriver.services;

namespace UIADriver.uia3.sourcebuilder
{
    public class WindowPageSourceBuilder : RootPageSourceBuilder
    {
        public WindowPageSourceBuilder(IUIAutomation automation, SessionCapabilities capabilities, ElementAttributeService<IUIAutomationElement> attrService) : base(automation, capabilities, attrService) { }

        public override PageSource BuildPageSource(IUIAutomationElement startElement)
        {
            var rs = base.BuildPageSource(startElement);
            if (rs.pageSource.Root != null)
            {
                modifyRect(rs.pageSource.Root);
            }
            return rs;
        }

        protected override void buildRecursive(XElement parent, Dictionary<XElement, IUIAutomationElement> mapping, IUIAutomationElement element, IUIAutomationTreeWalker walker, IUIAutomationCacheRequest request, int layer)
        {
            if (layer > capabilities.maxTreeDepth) return;
            var elementNode = walker.GetFirstChildElementBuildCache(element, request);

            while (elementNode != null)
            {
                try
                {
                    if (checkIfElementCanCauseInfiniteLoop(elementNode, walker))
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

        private void modifyRect(XElement root)
        {
            var xAtt = root.Attribute("X");
            var yAtt = root.Attribute("Y");
            if (xAtt == null || yAtt == null) return;
            int x = int.Parse(xAtt.Value);
            int y = int.Parse(yAtt.Value);
            root.SetAttributeValue("X", 0);
            root.SetAttributeValue("Y", 0);

            foreach (var item in root.Descendants())
            {
                var itemXAtt = item.Attribute("X");
                var itemYAtt = item.Attribute("Y");
                var itemWidthAtt = item.Attribute("Width");
                var itemHeightAtt = item.Attribute("Height");

                if (itemXAtt == null || itemYAtt == null || itemWidthAtt == null || itemHeightAtt == null) continue;
                int itemX = int.Parse(itemXAtt.Value);
                int itemY = int.Parse(itemYAtt.Value);
                int itemW = int.Parse(itemWidthAtt.Value);
                int itemH = int.Parse(itemHeightAtt.Value);
                if (itemX == 0 && itemY == 0 && itemW == 0 && itemH == 0) continue;

                item.SetAttributeValue("X", itemX - x);
                item.SetAttributeValue("Y", itemY - y);
            }
        }

        protected override void findElementByPropertyRecursive(IUIAutomationElement element, string propertyName, string? propertyValue, bool stopAtFirst, int layer, IUIAutomationTreeWalker walker, IUIAutomationCacheRequest request, List<IUIAutomationElement> rs)
        {
            if (layer > capabilities.maxTreeDepth) return;

            var propValue = attrService.GetAttributeString(element, propertyName);
            if (propertyValue == propValue || propValue != null && propValue.Equals(propertyValue))
            {
                rs.Add(element);
                if (stopAtFirst) return;
            }

            var child = walker.GetFirstChildElementBuildCache(element, request);
            while (child != null)
            {
                if (checkIfElementCanCauseInfiniteLoop(child, walker))
                {
                    break;
                }

                findElementByPropertyRecursive(child, propertyName, propertyValue, stopAtFirst, layer + 1, walker, request, rs);
                if (rs.Count > 0 && stopAtFirst) return;

                child = walker.GetNextSiblingElementBuildCache(child, request);
            }
        }

    }
}
