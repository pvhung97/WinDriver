using Interop.UIAutomationClient;
using System.Xml.Linq;
using UIADriver.uia3.attribute;
using UIADriver.win32native;

namespace UIADriver.uia3.sourcebuilder
{
    public class RootPageSourceBuilder : PageSourceBuilder
    {
        public RootPageSourceBuilder(IUIAutomation automation, ElementAttributeGetter attributeGetter, SessionCapabilities capabilities) : base(automation, attributeGetter, capabilities) { }

        public override PageSource buildPageSource(IUIAutomationElement startElement)
        {
            var walker = automation.CreateTreeWalker(automation.CreateTrueCondition());
            var cacheRequest = automation.CreateCacheRequest();
            foreach (var item in getPropertyList())
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
                    if (Win32Methods.IsIconic((int)elementNode.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId)))
                    {
                        elementNode = walker.GetNextSiblingElementBuildCache(elementNode, request);
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
                catch { }

                elementNode = walker.GetNextSiblingElementBuildCache(elementNode, request);
            }
        }

        protected override List<int> getPropertyList()
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
                        var value = attributeGetter.basicAttr.GetPropertyStrValue(element, propId);
                        if (!string.IsNullOrEmpty(value)) rs.SetAttributeValue(propName, value);
                        break;
                }

            }

            return rs;
        }

        public override List<IUIAutomationElement> findElementByProperty(IUIAutomationElement topLevelWindow, int propertyId, string? propertyValue, bool stopAtFirst)
        {
            List<IUIAutomationElement> rs = [];
            var walker = automation.CreateTreeWalker(automation.CreateTrueCondition());
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(propertyId);

            findElementByPropertyRecursive(topLevelWindow, propertyId, propertyValue, stopAtFirst, 1, walker, cacheRequest, rs);
            return rs;
        }

        protected override void findElementByPropertyRecursive(IUIAutomationElement element, int propertyId, string? propertyValue, bool stopAtFirst, int layer, IUIAutomationTreeWalker walker, IUIAutomationCacheRequest request, List<IUIAutomationElement> rs)
        {
            if (layer > capabilities.maxTreeDepth) return;

            try
            {
                var updated = element.BuildUpdatedCache(request);
                var propValue = attributeGetter.basicAttr.GetPropertyStrValue(updated, propertyId);
                if (propertyValue == propValue || propValue != null && propValue.Equals(propertyValue))
                {
                    rs.Add(updated);
                    if (stopAtFirst) return;
                }
            }
            catch { }

            var wndHdlCache = automation.CreateCacheRequest();
            wndHdlCache.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            var child = walker.GetFirstChildElementBuildCache(element, wndHdlCache);
            while (child != null)
            {
                if (layer == 1)
                {
                    try
                    {
                        if (Win32Methods.IsIconic((int)child.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId)))
                        {
                            child = walker.GetNextSiblingElement(child);
                            continue;
                        }
                    }
                    catch { }
                }

                findElementByPropertyRecursive(child, propertyId, propertyValue, stopAtFirst, layer + 1, walker, request, rs);
                if (rs.Count > 0 && stopAtFirst) return;

                child = walker.GetNextSiblingElement(child);
            }
        }
    }
}
