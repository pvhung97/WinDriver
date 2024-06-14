using Interop.UIAutomationClient;
using System.Windows;
using System.Xml.Linq;
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
                    int hwnd = (int)elementNode.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
                    if (hwnd != 0 && Win32Methods.IsIconic(hwnd))
                    {
                        elementNode = walker.GetNextSiblingElementBuildCache(elementNode, request);
                        continue;
                    }

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
                        var value = attrService.GetAttributeString(element, propName);
                        if (!string.IsNullOrEmpty(value)) rs.SetAttributeValue(propName, value);
                        break;
                }

            }

            return rs;
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

                    //  Skip this check if direct child of root
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
