using Interop.UIAutomationClient;
using System.Xml.Linq;
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

        protected bool checkIfElementCanCauseInfiniteLoop(IUIAutomationElement element, IUIAutomationTreeWalker walker)
        {
            var selectionPatternAvailable = (bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsSelectionPatternAvailablePropertyId);
            if (selectionPatternAvailable && automation.CompareElements(automation.GetRootElement(), walker.GetParentElement(element)) == 1)
            {
                return true;
            }
            return false;
        }
    }
}
