using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3DragPattern : DragPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3DragPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override List<string> GetDropEffects(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_DragDropEffectsPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationDragPattern)element.GetCachedPattern(UIA_PatternIds.UIA_DragPatternId);
            return pattern.CachedDropEffects.ToList();
        }

        public override List<FindElementResponse> GetGrabbedItems(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_DragGrabbedItemsPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationDragPattern)element.GetCachedPattern(UIA_PatternIds.UIA_DragPatternId);
            var elementArr = pattern.GetCachedGrabbedItems();
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                for (int i = 0; i < elementArr.Length; i++)
                {
                    try
                    {
                        string itemId = finderService.RegisterElement(elementArr.GetElement(i));
                        rs.Add(new FindElementResponse(itemId));
                    }
                    catch { }
                }
            }
            return rs;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsDragPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_DragPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsDragPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Drag pattern is not available for this element");
            }
            return element;
        }
    }
}
