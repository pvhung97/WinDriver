﻿using System.Windows.Automation;
using System.Xml.Linq;
using UIADriver.uia2.attribute;
using UIADriver.win32native;

namespace UIADriver.uia2.sourcebuilder
{
    public class RootPageSourceBuilder : PageSourceBuilder
    {
        public RootPageSourceBuilder(ElementAttributeGetter attributeGetter, SessionCapabilities capabilities) : base(capabilities, attributeGetter) { }

        public override PageSource buildPageSource(AutomationElement startElement)
        {
            var walker = new TreeWalker(Condition.TrueCondition);
            var cacheRequest = new CacheRequest();
            foreach (var item in getPropertyList())
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

        protected override List<AutomationProperty> getPropertyList()
        {
            return [
                AutomationElement.AcceleratorKeyProperty,
                AutomationElement.ItemStatusProperty,
                AutomationElement.ItemTypeProperty,
                AutomationElement.LocalizedControlTypeProperty,
                AutomationElement.NameProperty,
                AutomationElement.NativeWindowHandleProperty,
                AutomationElement.OrientationProperty,
                AutomationElement.PositionInSetProperty,
                AutomationElement.ProcessIdProperty,
                AutomationElement.RuntimeIdProperty,
                AutomationElement.SizeOfSetProperty,
                AutomationElement.AccessKeyProperty,
                AutomationElement.AutomationIdProperty,
                AutomationElement.BoundingRectangleProperty,
                AutomationElement.ClassNameProperty,
                AutomationElement.ControlTypeProperty,
                AutomationElement.FrameworkIdProperty,
                AutomationElement.HasKeyboardFocusProperty,
                AutomationElement.HeadingLevelProperty,
                AutomationElement.HelpTextProperty,
                AutomationElement.CultureProperty,
                AutomationElement.IsKeyboardFocusableProperty,
                AutomationElement.IsControlElementProperty,
                AutomationElement.IsDialogProperty,
                AutomationElement.IsEnabledProperty,
                AutomationElement.IsContentElementProperty,
                AutomationElement.IsOffscreenProperty,
                AutomationElement.IsPasswordProperty,
                AutomationElement.IsRequiredForFormProperty,

                AutomationElement.IsSelectionItemPatternAvailableProperty,
                AutomationElement.IsSelectionPatternAvailableProperty,
                AutomationElement.IsSynchronizedInputPatternAvailableProperty,
                AutomationElement.IsTableItemPatternAvailableProperty,
                AutomationElement.IsTablePatternAvailableProperty,
                AutomationElement.IsTextPatternAvailableProperty,
                AutomationElement.IsTogglePatternAvailableProperty,
                AutomationElement.IsTransformPatternAvailableProperty,
                AutomationElement.IsValuePatternAvailableProperty,
                AutomationElement.IsVirtualizedItemPatternAvailableProperty,
                AutomationElement.IsWindowPatternAvailableProperty,
                AutomationElement.IsScrollPatternAvailableProperty,
                AutomationElement.IsScrollItemPatternAvailableProperty,
                AutomationElement.IsRangeValuePatternAvailableProperty,
                AutomationElement.IsDockPatternAvailableProperty,
                AutomationElement.IsExpandCollapsePatternAvailableProperty,
                AutomationElement.IsGridItemPatternAvailableProperty,
                AutomationElement.IsGridPatternAvailableProperty,
                AutomationElement.IsInvokePatternAvailableProperty,
                AutomationElement.IsItemContainerPatternAvailableProperty,
                AutomationElement.IsMultipleViewPatternAvailableProperty,
            ];
        }

        protected XElement createXElement(AutomationElement element)
        {
            string tagname = Utilities.GetControlTypeString(element.Cached.ControlType.Id);
            var rect = element.Cached.BoundingRectangle;
            if (double.IsInfinity(rect.Width))
            {
                rect = System.Windows.Rect.Empty;
            }

            XElement rs = new XElement(tagname,
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
                    var value = attributeGetter.basicAttr.GetPropertyStrValue(element, propId);
                    if (attrName != null)
                    {
                        if (attrName != null && !string.IsNullOrEmpty(value)) rs.SetAttributeValue(attrName, value);
                    }
                }
            }

            return rs;
        }

        public override List<AutomationElement> findElementByProperty(AutomationElement topLevelWindow, AutomationProperty propertyId, string? propertyValue, bool stopAtFirst)
        {
            List<AutomationElement> rs = [];
            var walker = new TreeWalker(Condition.TrueCondition);
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(propertyId);

            findElementByPropertyRecursive(topLevelWindow, propertyId, propertyValue, stopAtFirst, 1, walker, cacheRequest, rs);
            return rs;
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

            var wndHdlCache = new CacheRequest();
            wndHdlCache.Add(AutomationElement.NativeWindowHandleProperty);
            var child = walker.GetFirstChild(element, wndHdlCache);
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

                findElementByPropertyRecursive(child, propertyId, propertyValue, stopAtFirst, layer + 1, walker, request, rs);
                if (rs.Count > 0 && stopAtFirst) return;

                child = walker.GetNextSibling(child);
            }
        }
    }
}
