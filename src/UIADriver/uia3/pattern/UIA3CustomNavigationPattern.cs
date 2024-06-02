using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3CustomNavigationPattern : CustomNavigationPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3CustomNavigationPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override FindElementResponse Navigate(string elementId, string direction)
        {
            NavigateDirection d = NavigateDirection.NavigateDirection_FirstChild;
            switch (direction.ToLower())
            {
                case "previoussibling":
                    d = NavigateDirection.NavigateDirection_PreviousSibling;
                    break;
                case "nextsibling":
                    d = NavigateDirection.NavigateDirection_NextSibling;
                    break;
                case "parent":
                    d = NavigateDirection.NavigateDirection_Parent;
                    break;
                case "lastchild":
                    d = NavigateDirection.NavigateDirection_LastChild;
                    break;
                case "firstchild":
                    d = NavigateDirection.NavigateDirection_FirstChild;
                    break;
                default:
                    throw new InvalidArgument("Invalid direction");
            }
            var element = AssertPattern(elementId, automation.CreateCacheRequest());
            var pattern = (IUIAutomationCustomNavigationPattern)element.GetCachedPattern(UIA_PatternIds.UIA_CustomNavigationPatternId);
            FindElementResponse? rsp = null;
            try
            {
                var itemId = finderService.RegisterElement(pattern.Navigate(d));
                rsp = new FindElementResponse(itemId);
            }
            catch { }
            if (rsp == null) throw new NoSuchElement($"No item found when navigate with direction {direction}");
            return rsp;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsCustomNavigationPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_CustomNavigationPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsCustomNavigationPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Custom navigation pattern is not available for this element");
            }
            return element;
        }
    }
}
