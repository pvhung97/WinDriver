using Interop.UIAutomationClient;
using System.Xml.Linq;
using System.Xml.XPath;
using UIADriver.services;

namespace UIADriver.uia3.sourcebuilder
{
    public abstract class PageSourceBuilder : PageSourceService<IUIAutomationElement>
    {
        protected IUIAutomation automation;

        public PageSourceBuilder(IUIAutomation automation, SessionCapabilities capabilities, ElementAttributeService<IUIAutomationElement> attrService) : base(capabilities, attrService)
        {
            this.automation = automation;
        }

        protected abstract void buildRecursive(XElement parent, Dictionary<XElement, IUIAutomationElement> mapping, IUIAutomationElement element, IUIAutomationTreeWalker walker, IUIAutomationCacheRequest request, int layer);
        protected abstract List<int> getPropertyList();
        protected abstract void findElementByPropertyRecursive(IUIAutomationElement element, string propertyName, string? propertyValue, bool stopAtFirst, int layer, IUIAutomationTreeWalker walker, IUIAutomationCacheRequest request, List<IUIAutomationElement> rs);

        protected XElement createXElement(IUIAutomationElement element)
        {
            string tagname = Utilities.GetControlTypeString((int)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_ControlTypePropertyId));
            double[] rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            if (double.IsInfinity(rect[2]))
            {
                rect = [0, 0, 0, 0];
            }

            XElement rs = new XElement(tagname,
                                new XAttribute("X", ((int)rect[0]).ToString()),
                                new XAttribute("Y", ((int)rect[1]).ToString()),
                                new XAttribute("Width", ((int)rect[2]).ToString()),
                                new XAttribute("Height", ((int)rect[3]).ToString()));

            foreach (var propId in getPropertyList())
            {
                switch (propId)
                {
                    case UIA_PropertyIds.UIA_ControlTypePropertyId:
                    case UIA_PropertyIds.UIA_BoundingRectanglePropertyId:
                        break;
                    default:
                        UIA3PropertyEnum propEnum = (UIA3PropertyEnum)propId;
                        string? propName = Enum.GetName(propEnum);
                        if (string.IsNullOrEmpty(propName)) break;
                        var value = attrService.GetAttributeString(element, propName);
                        if (!string.IsNullOrEmpty(value)) rs.SetAttributeValue(propName, value);
                        break;
                }

            }

            return rs;
        }

        protected bool checkIfElementCanCauseInfiniteLoop(IUIAutomationElement element, IUIAutomationTreeWalker walker)
        {
            var selectionPatternAvailable = (bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsSelectionPatternAvailablePropertyId);
            if (selectionPatternAvailable && automation.CompareElements(automation.GetRootElement(), walker.GetParentElement(element)) == 1)
            {
                return true;
            }
            return false;
        }

        public override List<IUIAutomationElement> ResolveXpath(IUIAutomationElement topLevelWindow, string xpath, bool stopAtFirst)
        {
            List<IUIAutomationElement> rs = [];
            var segements = XpathParser.Parse(xpath);

            var cacheRequest = automation.CreateCacheRequest();
            foreach (var item in getPropertyList())
            {
                cacheRequest.AddProperty(item);
            }
            var treeWalker = automation.CreateTreeWalker(automation.CreateTrueCondition());
            ResolveXpathRecursive(topLevelWindow.BuildUpdatedCache(cacheRequest), segements, stopAtFirst, 1, cacheRequest, treeWalker, rs);

            return rs;
        }

        protected void ResolveXpathRecursive(IUIAutomationElement parentPointer, List<XpathSegment> segments, bool stopAtFirst, int layer, IUIAutomationCacheRequest cacheRequest, IUIAutomationTreeWalker treeWalker, List<IUIAutomationElement> rsList)
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
                    Dictionary<XElement, IUIAutomationElement> mapping = [];
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
                    Dictionary<XElement, IUIAutomationElement> mapping = [];
                    var parentElement = createXElement(parentPointer);
                    var elementChild = treeWalker.GetFirstChildElementBuildCache(parentPointer, cacheRequest);
                    while (elementChild != null)
                    {
                        if (checkIfElementCanCauseInfiniteLoop(elementChild, treeWalker)) break;

                        var childXml = createXElement(elementChild);
                        parentElement.Add(childXml);
                        mapping.Add(childXml, elementChild);

                        elementChild = treeWalker.GetNextSiblingElementBuildCache(elementChild, cacheRequest);
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
                    if (!Utilities.GetControlTypeString(parentPointer.CachedControlType).Equals(tagname)) return;
                    ResolveXpathRecursive(parentPointer, segments.GetRange(1, segments.Count - 1), stopAtFirst, layer + 1, cacheRequest, treeWalker, rsList);
                }
                else
                {
                    IUIAutomationElement? foundElement = null;
                    var elementChild = treeWalker.GetFirstChildElementBuildCache(parentPointer, cacheRequest);
                    while (elementChild != null)
                    {
                        if (checkIfElementCanCauseInfiniteLoop(elementChild, treeWalker)) return;

                        if (Utilities.GetControlTypeString(elementChild.CachedControlType).Equals(tagname)) index--;
                        if (index == 0)
                        {
                            foundElement = elementChild;
                            break;
                        }

                        elementChild = treeWalker.GetNextSiblingElementBuildCache(elementChild, cacheRequest);
                    }
                    if (foundElement == null) return;

                    ResolveXpathRecursive(foundElement, segments.GetRange(1, segments.Count - 1), stopAtFirst, layer + 1, cacheRequest, treeWalker, rsList);
                }
            }
        }
    }
}
