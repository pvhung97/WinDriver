using System.Windows.Automation;
using System.Xml.Linq;
using UIADriver.uia2.attribute;

namespace UIADriver.uia2.sourcebuilder
{
    public class WindowPageSourceBuilder : RootPageSourceBuilder
    {
        public WindowPageSourceBuilder(ElementAttributeGetter attributeGetter, SessionCapabilities capabilities) : base(attributeGetter, capabilities) { }

        public override PageSource buildPageSource(AutomationElement startElement)
        {
            var rs = base.buildPageSource(startElement);
            if (rs.pageSource.Root != null)
            {
                modifyRect(rs.pageSource.Root);
            }
            return rs;
        }

        protected override void buildRecursive(XElement parent, Dictionary<XElement, AutomationElement> mapping, AutomationElement element, TreeWalker walker, CacheRequest request, int layer)
        {
            if (layer > capabilities.maxTreeDepth) return;
            var elementNode = walker.GetFirstChild(element, request);

            while (elementNode != null)
            {
                try
                {
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

        protected override void findElementByPropertyRecursive(AutomationElement element, AutomationProperty propertyId, string? propertyValue, bool stopAtFirst, int layer, TreeWalker walker, CacheRequest request, List<AutomationElement> rs)
        {
            if (layer > capabilities.maxTreeDepth) return;

            try
            {
                var updated = element.GetUpdatedCache(request);
                var propValue = attributeGetter.basicAttr.GetPropertyStrValue(updated, propertyId);
                if (propertyValue == propValue || propValue != null && propValue.Equals(propertyValue))
                {
                    rs.Add(updated);
                    if (stopAtFirst) return;
                }
            }
            catch { }

            var child = walker.GetFirstChild(element);
            while (child != null)
            {
                findElementByPropertyRecursive(child, propertyId, propertyValue, stopAtFirst, layer + 1, walker, request, rs);
                if (rs.Count > 0 && stopAtFirst) return;

                child = walker.GetNextSibling(child);
            }
        }
    }
}
