using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.services;

namespace UIADriver.uia3.attribute
{
    public class ElementAttributeGetter : ElementAttributeService<IUIAutomationElement>
    {
        protected IUIAutomation automation;
        protected BasicElementPropertyValueGetter basicAttr;

        public ElementAttributeGetter(IUIAutomation automation)
        {
            this.automation = automation;
            this.basicAttr = new BasicElementPropertyValueGetter();
        }

        public override object? GetAttributeObject(IUIAutomationElement element, string attribute)
        {
            var found = Enum.TryParse<UIA3PropertyEnum>(attribute, out var propertyId);
            if (found == false) return null;

            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty((int)propertyId);
            element = element.BuildUpdatedCache(cacheRequest);
            return element.GetCachedPropertyValue((int)propertyId);
        }

        public override string? GetAttributeString(IUIAutomationElement element, string attribute)
        {
            return GetAttributeString(element, attribute, true);
        }

        public override string? GetAttributeString(IUIAutomationElement element, string attribute, bool updateCache)
        {
            var found = Enum.TryParse<UIA3PropertyEnum>(attribute, out var propertyId);
            if (found == false) return null;

            if (updateCache)
            {
                var cacheRequest = automation.CreateCacheRequest();
                cacheRequest.AddProperty((int)propertyId);
                element = element.BuildUpdatedCache(cacheRequest);
            }

            return basicAttr.GetPropertyStrValue(element, (int)propertyId);
        }

        public override RectResponse GetElementRect(IUIAutomationElement element, Point rootPoint)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            element = element.BuildUpdatedCache(cacheRequest);

            var rect = (double[])element.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            return new RectResponse((int)rect[0] - rootPoint.X, (int)rect[1] - rootPoint.Y, double.IsInfinity(rect[2]) ? 0 : (int)rect[2], double.IsInfinity(rect[3]) ? 0 : (int)rect[3]);
        }

        public override string GetElementTagName(IUIAutomationElement element)
        {
            var tagName = GetAttributeString(element, Enum.GetName(UIA3PropertyEnum.ControlType));
            return tagName == null ? "Unknown" : tagName;
        }

        public override string GetElementText(IUIAutomationElement element)
        {
            var treeWalker = automation.CreateTreeWalker(automation.CreateTrueCondition());
            var firstChild = treeWalker.GetFirstChildElement(element);
            if (firstChild != null) return "";

            var name = GetAttributeString(element, Enum.GetName(UIA3PropertyEnum.Name));
            return name == null ? "" : name;
        }

        public override bool IsElementDisplayed(IUIAutomationElement element)
        {
            var cachedRequest = automation.CreateCacheRequest();
            cachedRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            cachedRequest.AddProperty(UIA_PropertyIds.UIA_IsOffscreenPropertyId);
            var updatedElement = element.BuildUpdatedCache(cachedRequest);

            var isOffscreen = (bool)updatedElement.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsOffscreenPropertyId);
            var boundingRect = (double[])updatedElement.GetCachedPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            if (isOffscreen || boundingRect[2] == 0 || boundingRect[3] == 0) return false;
            return true;
        }

        public override bool IsElementEnabled(IUIAutomationElement element)
        {
            var isEnabled = GetAttributeObject(element, Enum.GetName(UIA3PropertyEnum.IsEnabled));
            return isEnabled == null ? false : (bool)isEnabled;
        }

        public override bool IsElementSelected(IUIAutomationElement element)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsSelectionItemPatternAvailablePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_SelectionItemIsSelectedPropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsTogglePatternAvailablePropertyId);
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_ToggleToggleStatePropertyId);
            element = element.BuildUpdatedCache(cacheRequest);

            if ((bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsSelectionItemPatternAvailablePropertyId))
            {
                return (bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_SelectionItemIsSelectedPropertyId);
            }

            if ((bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsTogglePatternAvailablePropertyId))
            {
                return (ToggleState)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_ToggleToggleStatePropertyId) == ToggleState.ToggleState_On;
            }

            return false;
        }
    }
}
