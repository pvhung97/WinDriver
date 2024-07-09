using System.Windows.Automation;
using System.Xml.Linq;
using System.Xml.XPath;
using UIADriver.services;

namespace UIADriver.uia2.sourcebuilder
{
    public abstract class PageSourceBuilder : PageSourceService<AutomationElement>
    {
        public PageSourceBuilder(SessionCapabilities capabilities, ElementAttributeService<AutomationElement> attributeService) : base(capabilities, attributeService) { }

        protected abstract void buildRecursive(XElement parent, Dictionary<XElement, AutomationElement> mapping, AutomationElement element, TreeWalker treeWalker, CacheRequest cacheRequest, int layer);
        protected abstract List<AutomationProperty> getPropertyList();
        protected abstract void findElementByPropertyRecursive(AutomationElement element, string propertyName, string? propertyValue, bool stopAtFirst, int layer, TreeWalker walker, CacheRequest request, List<AutomationElement> rs);

        protected XElement createXElement(AutomationElement element)
        {
            string tagname = Utilities.GetControlTypeString(element.Cached.ControlType.Id);
            var rect = element.Cached.BoundingRectangle;
            if (double.IsInfinity(rect.Width))
            {
                rect = System.Windows.Rect.Empty;
            }

            var rs = new XElement(tagname,
                                new XAttribute("X", ((int)rect.X).ToString()),
                                new XAttribute("Y", ((int)rect.Y).ToString()),
                                new XAttribute("Width", ((int)rect.Width).ToString()),
                                new XAttribute("Height", ((int)rect.Height).ToString()));

            int controlTypeId = AutomationElement.ControlTypeProperty.Id;
            var rectId = AutomationElement.BoundingRectangleProperty.Id;

            foreach (var propId in getPropertyList())
            {
                if (propId.Id != controlTypeId && propId.Id != rectId)
                {
                    string? attrName = UIA2PropertyDictionary.GetAutomationPropertyName(propId.Id);
                    if (string.IsNullOrEmpty(attrName)) continue;
                    var value = attrService.GetAttributeString(element, attrName);
                    if (!string.IsNullOrEmpty(value)) rs.SetAttributeValue(attrName, value);
                }
            }

            return rs;
        }

        public override List<AutomationElement> ResolveXpath(AutomationElement topLevelWindow, string xpath, bool stopAtFirst)
        {
            List<AutomationElement> rs = [];
            var segements = XpathParser.Parse(xpath);

            var cacheRequest = new CacheRequest();
            foreach (var item in getPropertyList())
            {
                cacheRequest.Add(item);
            }
            var treeWalker = new TreeWalker(Condition.TrueCondition);
            ResolveXpathRecursive(topLevelWindow.GetUpdatedCache(cacheRequest), segements, stopAtFirst, 1, cacheRequest, treeWalker, rs);

            return rs;
        }

        protected void ResolveXpathRecursive(AutomationElement parentPointer, List<XpathSegment> segments, bool stopAtFirst, int layer, CacheRequest cacheRequest, TreeWalker treeWalker, List<AutomationElement> rsList)
        {
            if (segments.Count == 0)
            {
                rsList.Add(parentPointer);
                return;
            }
            if (layer > capabilities.maxTreeDepth) return;

            var segment = segments[0];
            if (segment.type.Equals(ResolveType.FULL_SOURCE))
            {
                if (layer == 1)
                {
                    var fullSource = BuildPageSource(parentPointer);
                    var foundElements = fullSource.pageSource.XPathSelectElements(segment.xpath);
                    foreach (var item in foundElements)
                    {
                        rsList.Add(fullSource.mapping[item]);
                    }
                }
                else
                {
                    Dictionary<XElement, AutomationElement> mapping = [];
                    var parentElement = createXElement(parentPointer);
                    buildRecursive(parentElement, mapping, parentPointer, treeWalker, cacheRequest, layer);
                    var partialSource = new XDocument(parentElement);
                    var foundElements = partialSource.XPathSelectElements($"/{parentElement.Name.LocalName}{segment.xpath}");
                    foreach (var item in foundElements)
                    {
                        rsList.Add(mapping[item]);
                    }
                }
            }
            else if (segment.type.Equals(ResolveType.PARTIAL_SOURCE))
            {
                if (layer == 1)
                {
                    var partialSource = new XDocument(createXElement(parentPointer));
                    if (partialSource.XPathSelectElements(segment.xpath).Count() == 0) return;
                    else ResolveXpathRecursive(parentPointer, segments.GetRange(1, segments.Count - 1), stopAtFirst, layer + 1, cacheRequest, treeWalker, rsList);
                }
                else
                {
                    Dictionary<XElement, AutomationElement> mapping = [];
                    var parentElement = createXElement(parentPointer);
                    var elementChild = treeWalker.GetFirstChild(parentPointer, cacheRequest);
                    while (elementChild != null)
                    {
                        var childXml = createXElement(elementChild);
                        parentElement.Add(childXml);
                        mapping.Add(childXml, elementChild);

                        elementChild = treeWalker.GetNextSibling(elementChild, cacheRequest);
                    }
                    var partialSource = new XDocument(parentElement);
                    var foundElements = partialSource.XPathSelectElements($"/{parentElement.Name.LocalName}{segment.xpath}");

                    foreach (var item in foundElements)
                    {
                        ResolveXpathRecursive(mapping[item], segments.GetRange(1, segments.Count - 1), stopAtFirst, layer + 1, cacheRequest, treeWalker, rsList);
                        if (stopAtFirst && rsList.Count > 0) return;
                    }
                }
            }
            else
            {
                string tagname = segment.getMeta<string>("tagName");
                int index = segment.getMeta<int>("index");
                if (layer == 1)
                {
                    if (index != 1) return;
                    if (!Utilities.GetControlTypeString(parentPointer.Cached.ControlType.Id).Equals(tagname)) return;
                    ResolveXpathRecursive(parentPointer, segments.GetRange(1, segments.Count - 1), stopAtFirst, layer + 1, cacheRequest, treeWalker, rsList);
                }
                else
                {
                    AutomationElement? foundElement = null;
                    var elementChild = treeWalker.GetFirstChild(parentPointer, cacheRequest);
                    while (elementChild != null)
                    {
                        if (Utilities.GetControlTypeString(elementChild.Cached.ControlType.Id).Equals(tagname)) index--;
                        if (index == 0)
                        {
                            foundElement = elementChild;
                            break;
                        }

                        elementChild = treeWalker.GetNextSibling(elementChild, cacheRequest);
                    }
                    if (foundElement == null) return;

                    ResolveXpathRecursive(foundElement, segments.GetRange(1, segments.Count - 1), stopAtFirst, layer + 1, cacheRequest, treeWalker, rsList);
                }
            }
        }

    }
}
