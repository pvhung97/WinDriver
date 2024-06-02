using Interop.UIAutomationClient;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia3.pattern
{
    public class UIA3AnnotationPattern : AnnotationPatternService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        protected IUIAutomation automation;
        public UIA3AnnotationPattern(ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest> finderService, ElementAttributeService<IUIAutomationElement> attributeService, IUIAutomation automation) : base(finderService, attributeService)
        {
            this.automation = automation;
        }

        public override int GetAnnotationTypeId(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_AnnotationTypesPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationAnnotationPattern)element.GetCachedPattern(UIA_PatternIds.UIA_AnnotationPatternId);
            return pattern.CachedAnnotationTypeId;
        }

        public override string GetAnnotationTypeName(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_AnnotationAnnotationTypeNamePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationAnnotationPattern)element.GetCachedPattern(UIA_PatternIds.UIA_AnnotationPatternId);
            return pattern.CachedAnnotationTypeName;
        }

        public override string GetAuthor(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_AnnotationAuthorPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationAnnotationPattern)element.GetCachedPattern(UIA_PatternIds.UIA_AnnotationPatternId);
            return pattern.CachedAuthor;
        }

        public override string GetDateTime(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_AnnotationDateTimePropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationAnnotationPattern)element.GetCachedPattern(UIA_PatternIds.UIA_AnnotationPatternId);
            return pattern.CachedDateTime;
        }

        public override FindElementResponse GetTarget(string elementId)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_AnnotationTargetPropertyId);
            var element = AssertPattern(elementId, cacheRequest);
            var pattern = (IUIAutomationAnnotationPattern)element.GetCachedPattern(UIA_PatternIds.UIA_AnnotationPatternId);
            return new FindElementResponse(finderService.RegisterElement(pattern.CachedTarget));
        }

        protected override IUIAutomationElement AssertPattern(string elementId, IUIAutomationCacheRequest cacheRequest)
        {
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_IsAnnotationPatternAvailablePropertyId);
            cacheRequest.AddPattern(UIA_PatternIds.UIA_AnnotationPatternId);
            var element = finderService.GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(UIA_PropertyIds.UIA_IsAnnotationPatternAvailablePropertyId))
            {
                throw new InvalidArgument("Annotation pattern is not available for this element");
            }
            return element;
        }
    }
}
