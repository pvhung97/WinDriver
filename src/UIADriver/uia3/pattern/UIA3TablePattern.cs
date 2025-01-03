using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3TablePattern : TablePatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3TablePattern(ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider, IUIAutomation automation) : base(serviceProvider)
        {
            this.automation = automation;
        }

        public override List<FindElementResponse> GetColumnHeaders(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_TableColumnHeadersPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationTablePattern)element.GetCachedPattern(UIA_PatternIds.UIA_TablePatternId);
            var elementArr = pattern.GetCachedColumnHeaders();
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                for (int i = 0; i < elementArr.Length; i++)
                {
                    try
                    {
                        string itemId = serviceProvider.GetElementFinderService().RegisterElement(elementArr.GetElement(i));
                        rs.Add(new FindElementResponse(itemId));
                    } catch { }
                }
            }
            return rs;
        }

        public override List<FindElementResponse> GetRowHeaders(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_TableRowHeadersPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationTablePattern)element.GetCachedPattern(UIA_PatternIds.UIA_TablePatternId);
            var elementArr = pattern.GetCachedRowHeaders();
            List<FindElementResponse> rs = [];
            if (elementArr != null)
            {
                for (int i = 0; i < elementArr.Length; i++)
                {
                    try
                    {
                        string itemId = serviceProvider.GetElementFinderService().RegisterElement(elementArr.GetElement(i));
                        rs.Add(new FindElementResponse(itemId));
                    }
                    catch { }
                }
            }
            return rs;
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsTablePatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_TablePatternId);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsTablePatternAvailablePropertyId))
            {
                throw new InvalidArgument("Table pattern is not available for this element");
            }
            return element;
        }
    }
}
