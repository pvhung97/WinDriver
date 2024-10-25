using Interop.UIAutomationClient;
using System.Diagnostics;
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

            foreach (var propId in GetPropertyList())
            {
                try
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
                            var value = attrService.GetAttributeString(element, propName, false);
                            if (!string.IsNullOrEmpty(value)) rs.SetAttributeValue(propName, value);
                            break;
                    }
                } catch (Exception ex)
                {
                    Debug.WriteLine(ex);
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
            foreach (var item in GetPropertyList())
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

        protected List<int> GetPropertyList()
        {
            return AppendAddionalPatternProperty(GetBasePropertyList());
        }

        protected List<int> GetBasePropertyList()
        {
            return [
                UIA_PropertyIds.UIA_AcceleratorKeyPropertyId,
                UIA_PropertyIds.UIA_AccessKeyPropertyId,
                UIA_PropertyIds.UIA_AriaPropertiesPropertyId,
                UIA_PropertyIds.UIA_AriaRolePropertyId,
                UIA_PropertyIds.UIA_AutomationIdPropertyId,
                UIA_PropertyIds.UIA_BoundingRectanglePropertyId,
                UIA_PropertyIds.UIA_ClassNamePropertyId,
                UIA_PropertyIds.UIA_ControlTypePropertyId,
                UIA_PropertyIds.UIA_CulturePropertyId,
                UIA_PropertyIds.UIA_FillColorPropertyId,
                //UIA_PropertyIds.UIA_FillTypePropertyId,
                UIA_PropertyIds.UIA_FrameworkIdPropertyId,
                UIA_PropertyIds.UIA_FullDescriptionPropertyId,
                UIA_PropertyIds.UIA_HasKeyboardFocusPropertyId,
                //UIA_PropertyIds.UIA_HeadingLevelPropertyId,
                UIA_PropertyIds.UIA_HelpTextPropertyId,
                UIA_PropertyIds.UIA_IsContentElementPropertyId,
                UIA_PropertyIds.UIA_IsControlElementPropertyId,
                UIA_PropertyIds.UIA_IsDataValidForFormPropertyId,
                UIA_PropertyIds.UIA_IsDialogPropertyId,
                UIA_PropertyIds.UIA_IsEnabledPropertyId,
                UIA_PropertyIds.UIA_IsKeyboardFocusablePropertyId,
                UIA_PropertyIds.UIA_IsOffscreenPropertyId,
                UIA_PropertyIds.UIA_IsPasswordPropertyId,
                UIA_PropertyIds.UIA_IsPeripheralPropertyId,
                UIA_PropertyIds.UIA_IsRequiredForFormPropertyId,
                UIA_PropertyIds.UIA_ItemStatusPropertyId,
                UIA_PropertyIds.UIA_ItemTypePropertyId,
                UIA_PropertyIds.UIA_LandmarkTypePropertyId,
                UIA_PropertyIds.UIA_LevelPropertyId,
                UIA_PropertyIds.UIA_LiveSettingPropertyId,
                UIA_PropertyIds.UIA_LocalizedControlTypePropertyId,
                UIA_PropertyIds.UIA_LocalizedLandmarkTypePropertyId,
                UIA_PropertyIds.UIA_NamePropertyId,
                UIA_PropertyIds.UIA_NativeWindowHandlePropertyId,
                UIA_PropertyIds.UIA_OptimizeForVisualContentPropertyId,
                UIA_PropertyIds.UIA_OrientationPropertyId,
                UIA_PropertyIds.UIA_ProcessIdPropertyId,
                UIA_PropertyIds.UIA_ProviderDescriptionPropertyId,
                UIA_PropertyIds.UIA_VisualEffectsPropertyId,
                UIA_PropertyIds.UIA_RuntimeIdPropertyId,

                UIA_PropertyIds.UIA_IsDockPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsExpandCollapsePatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsGridItemPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsGridPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsInvokePatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsMultipleViewPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsRangeValuePatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsScrollPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsScrollItemPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsSelectionItemPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsSelectionPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsTablePatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsTableItemPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsTextPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsTogglePatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsTransformPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsValuePatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsWindowPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsLegacyIAccessiblePatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsItemContainerPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsVirtualizedItemPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsSynchronizedInputPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsObjectModelPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsAnnotationPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsTextPattern2AvailablePropertyId,
                UIA_PropertyIds.UIA_IsStylesPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsSpreadsheetPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsSpreadsheetItemPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsTransformPattern2AvailablePropertyId,
                UIA_PropertyIds.UIA_IsTextChildPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsDragPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsDropTargetPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsTextEditPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsCustomNavigationPatternAvailablePropertyId,
                UIA_PropertyIds.UIA_IsSelectionPattern2AvailablePropertyId
            ];
        }

        protected List<int> AppendAddionalPatternProperty(List<int> basePropertyList)
        {
            foreach (var item in capabilities.additionalPageSourcePattern)
            {
                switch (item)
                {
                    case "Value":
                    case "RangeValue":
                    case "Scroll":
                    case "Grid":
                    case "Dock":
                    case "ExpandCollapse":
                    case "Window":
                    case "Toggle":
                    case "Transform":
                    case "Styles":
                    case "Transform2":
                        foreach (var pattern in Enum.GetValues<UIA3PropertyEnum>())
                        {
                            if (Enum.GetName(pattern).StartsWith($"{item}_"))
                            {
                                basePropertyList.Add((int)pattern);
                            }
                        }
                        break;
                    case "Selection":
                        basePropertyList.Add(UIA_PropertyIds.UIA_SelectionCanSelectMultiplePropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_SelectionIsSelectionRequiredPropertyId);
                        break;
                    case "GridItem":
                        basePropertyList.Add(UIA_PropertyIds.UIA_GridItemColumnPropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_GridItemColumnSpanPropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_GridItemRowPropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_GridItemRowSpanPropertyId);
                        break;
                    case "MultipleView":
                        basePropertyList.Add(UIA_PropertyIds.UIA_MultipleViewCurrentViewPropertyId);
                        break;
                    case "SelectionItem":
                        basePropertyList.Add(UIA_PropertyIds.UIA_SelectionItemIsSelectedPropertyId);
                        break;
                    case "Table":
                        basePropertyList.Add(UIA_PropertyIds.UIA_TableRowOrColumnMajorPropertyId);
                        break;
                    case "LegacyIAccessible":
                        basePropertyList.Add(UIA_PropertyIds.UIA_LegacyIAccessibleChildIdPropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_LegacyIAccessibleNamePropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_LegacyIAccessibleValuePropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_LegacyIAccessibleDescriptionPropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_LegacyIAccessibleRolePropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_LegacyIAccessibleStatePropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_LegacyIAccessibleHelpPropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_LegacyIAccessibleKeyboardShortcutPropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_LegacyIAccessibleDefaultActionPropertyId);
                        break;
                    case "Annotation":
                        basePropertyList.Add(UIA_PropertyIds.UIA_AnnotationAnnotationTypeIdPropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_AnnotationAnnotationTypeNamePropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_AnnotationAuthorPropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_AnnotationDateTimePropertyId);
                        break;
                    case "SpreadsheetItem":
                        basePropertyList.Add(UIA_PropertyIds.UIA_SpreadsheetItemFormulaPropertyId);
                        break;
                    case "Drag":
                        basePropertyList.Add(UIA_PropertyIds.UIA_DragIsGrabbedPropertyId);
                        basePropertyList.Add(UIA_PropertyIds.UIA_DragDropEffectPropertyId);
                        break;
                    case "DropTarget":
                        basePropertyList.Add(UIA_PropertyIds.UIA_DropTargetDropTargetEffectPropertyId);
                        break;
                    case "Selection2":
                        basePropertyList.Add(UIA_PropertyIds.UIA_Selection2ItemCountPropertyId);
                        break;
                }
            }
            return basePropertyList;
        }
    }
}
