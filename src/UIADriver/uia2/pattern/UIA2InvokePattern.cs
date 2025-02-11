﻿using System.Windows.Automation;
using UIADriver.exception;
using UIADriver.services;
using UIADriver.services.pattern;

namespace UIADriver.uia2.pattern
{
    public class UIA2InvokePattern : InvokePatternService<AutomationElement, CacheRequest>
    {
        public UIA2InvokePattern(ServiceProvider<AutomationElement, CacheRequest> serviceProvider) : base(serviceProvider) { }

        public override void Invoke(string elementId)
        {
            var element = AssertPattern(elementId, new CacheRequest());
            var pattern = (InvokePattern)element.GetCachedPattern(InvokePattern.Pattern);
            pattern.Invoke();
        }

        protected override AutomationElement AssertPattern(string elementId, CacheRequest cacheRequest)
        {
            cacheRequest.Add(AutomationElement.IsInvokePatternAvailableProperty);
            cacheRequest.Add(InvokePattern.Pattern);
            var element = serviceProvider.GetElementFinderService().GetElement(elementId, cacheRequest);
            if (!(bool)element.GetCachedPropertyValue(AutomationElement.IsInvokePatternAvailableProperty))
            {
                throw new InvalidArgument("Invoke pattern is not available for this element");
            }
            return element;
        }
    }
}
