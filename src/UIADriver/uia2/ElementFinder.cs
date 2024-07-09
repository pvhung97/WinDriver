using System.Windows.Automation;
using System.Xml.XPath;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;

namespace UIADriver.uia2
{
    public class ElementFinder :ElementFinderService<AutomationElement, CacheRequest>
    {
        public ElementFinder(PageSourceService<AutomationElement> pageSourceService, ElementAttributeService<AutomationElement> attrService) : base(pageSourceService, attrService) { }

        public override FindElementResponse FindElement(FindElementRequest request, AutomationElement topLevelWindow)
        {
            //  Both FindFirst and FindAll will miss any element that is neither ControlElement nor ContentElement
            var found = FindElements(request, topLevelWindow, true);
            if (found.Count == 0) throw new NoSuchElement("Cannot find element with given location strategy and selector");
            return found[0];
        }

        public override List<FindElementResponse> FindElements(FindElementRequest request, AutomationElement topLevelWindow)
        {
            return FindElements(request, topLevelWindow, false);
        }

        private List<FindElementResponse> FindElements(FindElementRequest request, AutomationElement topLevelWindow, bool stopAtFirst)
        {
            List<AutomationElement> rs = [];
#pragma warning disable CS8604
            switch (request.strategy)
            {
                case "xpath":
                    rs = FindElementsWithXpath(request.value, topLevelWindow, stopAtFirst);
                    break;
                case "name":
                    rs = FindElementsWithPropertyNameAndValue(UIA2PropertyDictionary.GetAutomationPropertyName(AutomationElement.NameProperty.Id), request.value, topLevelWindow, stopAtFirst);
                    break;
                case "automation id":
                    rs = FindElementsWithPropertyNameAndValue(UIA2PropertyDictionary.GetAutomationPropertyName(AutomationElement.AutomationIdProperty.Id), request.value, topLevelWindow, stopAtFirst);
                    break;
                case "id":
                    rs = FindElementsWithPropertyNameAndValue(UIA2PropertyDictionary.GetAutomationPropertyName(AutomationElement.RuntimeIdProperty.Id), request.value, topLevelWindow, stopAtFirst);
                    break;
                case "tag name":
                    rs = FindElementsWithPropertyNameAndValue(UIA2PropertyDictionary.GetAutomationPropertyName(AutomationElement.ControlTypeProperty.Id), request.value, topLevelWindow, stopAtFirst);
                    break;
                default:
                    throw new InvalidArgument("Unsupported location strategy " + request.strategy);
            }
#pragma warning restore CS8604

            var resp = new List<FindElementResponse>();
            foreach (var item in rs)
            {
                string id = RegisterElement(item);
                resp.Add(new FindElementResponse(id));
            }
            return resp;
        }

        private List<AutomationElement> FindElementsWithXpath(string xpath, AutomationElement topLevelWindow, bool stopAtFirst)
        {
            return pageSourceService.ResolveXpath(topLevelWindow, xpath, stopAtFirst);
        }

        private List<AutomationElement> FindElementsWithPropertyNameAndValue(string propertyName, string value, AutomationElement topLevelWindow, bool stopAtFirst)
        {
            return pageSourceService.FindElementByProperty(topLevelWindow, propertyName, value, stopAtFirst);
        }

        public override AutomationElement GetElement(string id)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            return GetElement(id, cacheRequest);
        }

        public override AutomationElement GetElement(string id, CacheRequest cacheRequest)
        {
            cachedElement.TryGetValue(id, out var element);
            if (element == null) throw new StaleElementReference("element is stale");
            try
            {
                element = element.GetUpdatedCache(cacheRequest);
                return element;
            }
            catch
            {
                RemoveElement(id);
                throw new StaleElementReference("element is stale");
            }
        }

        public override FindElementResponse GetActiveElement()
        {
            return GetActiveElement(null);
        }

        public override FindElementResponse GetActiveElement(int? topLevelHdl)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);

            var active = AutomationElement.FocusedElement.GetUpdatedCache(cacheRequest);
            if (topLevelHdl == null)
            {
                var id = RegisterElement(active);
                return new FindElementResponse(id);
            }

            var prevHdl = 0;
            var hdl = 0;
            var walker = new TreeWalker(Condition.TrueCondition);
            var pointer = active;
            while (true)
            {
                if (pointer == null) break;

                prevHdl = hdl;
                hdl = pointer.Cached.NativeWindowHandle;
                pointer = walker.GetParent(pointer, cacheRequest);
            }
            if (topLevelHdl == prevHdl)
            {
                var id = RegisterElement(active);
                return new FindElementResponse(id);
            }
            throw new NoSuchElement("No active element on current window");
        }

        public override FindElementResponse FindElementFromParentElement(FindElementRequest request, string parentElementId)
        {
            AutomationElement? startPoint = null;
            try
            {
                startPoint = GetElement(parentElementId);
            }
            catch { }
            if (startPoint == null) throw new NoSuchElement("Cannot find any element with given parent element");

            return FindElement(request, startPoint);
        }

        public override List<FindElementResponse> FindElementsFromParentElement(FindElementRequest request, string parentElementId)
        {
            AutomationElement? startPoint = null;
            try
            {
                startPoint = GetElement(parentElementId);
            }
            catch { }
            if (startPoint == null) return [];

            return FindElements(request, startPoint);
        }
    }
}
