using Interop.UIAutomationClient;
using System.Xml.XPath;
using UIA3Driver.dto.request;
using UIA3Driver.dto.response;
using UIA3Driver.exception;
using UIADriver.uia3.sourcebuilder;

namespace UIADriver.uia3
{
    public class ElementFinder
    {
        private IUIAutomation automation;
        private PageSourceBuilder sourceBuilder;
        private Dictionary<string, IUIAutomationElement> cachedElement;

        public ElementFinder(IUIAutomation automation, PageSourceBuilder sourceBuilder)
        {
            this.automation = automation;
            cachedElement = [];
            this.sourceBuilder = sourceBuilder;
        }

        public void resetCache()
        {
            cachedElement.Clear();
        }

        public FindElementResponse FindElement(FindElementRequest request, IUIAutomationElement topLevelWindow)
        {
            //  Both FindFirst and FindAll will miss any element that is neither ControlElement nor ContentElement
            var found = FindElements(request, topLevelWindow, true);
            if (found.Count == 0) throw new NoSuchElement("Cannot find element with given location strategy and selector");
            return found[0];
        }

        public List<FindElementResponse> FindElements(FindElementRequest request, IUIAutomationElement topLevelWindow)
        {
            return FindElements(request, topLevelWindow, false);
        }

        private List<FindElementResponse> FindElements(FindElementRequest request, IUIAutomationElement topLevelWindow, bool stopAtFirst)
        {
            List<IUIAutomationElement> rs = [];
            switch (request.strategy)
            {
                case "xpath":
                    rs = FindElementsWithXpath(request.value, topLevelWindow);
                    break;
                case "name":
                    rs = FindElementsWithPropertIdAndValue(UIA_PropertyIds.UIA_NamePropertyId, request.value, topLevelWindow, stopAtFirst);
                    break;
                case "automation id":
                    rs = FindElementsWithPropertIdAndValue(UIA_PropertyIds.UIA_AutomationIdPropertyId, request.value, topLevelWindow, stopAtFirst);
                    break;
                case "id":
                    rs = FindElementsWithPropertIdAndValue(UIA_PropertyIds.UIA_RuntimeIdPropertyId, request.value, topLevelWindow, stopAtFirst);
                    break;
                case "tag name":
                    rs = FindElementsWithPropertIdAndValue(UIA_PropertyIds.UIA_ControlTypePropertyId, request.value, topLevelWindow, stopAtFirst);
                    break;
                default:
                    throw new InvalidArgument("Unsupported location strategy " + request.strategy);
            }

            var resp = new List<FindElementResponse>();
            foreach (var item in rs)
            {
                string id = Guid.NewGuid().ToString();
                cachedElement[id] = item;
                resp.Add(new FindElementResponse(id));
            }
            return resp;
        }

        private List<IUIAutomationElement> FindElementsWithXpath(string xpath, IUIAutomationElement topLevelWindow)
        {
            var source = sourceBuilder.buildPageSource(topLevelWindow);
            var nodes = source.pageSource.XPathSelectElements(xpath);
            var rs = new List<IUIAutomationElement>();

            foreach (var node in nodes)
            {
                source.mapping.TryGetValue(node, out var result);
                if (result != null) rs.Add(result);
            }
            return rs;
        }

        private List<IUIAutomationElement> FindElementsWithPropertIdAndValue(int propertyId, string value, IUIAutomationElement topLevelWindow, bool stopAtFirst)
        {
            return sourceBuilder.findElementByProperty(topLevelWindow, propertyId, value, stopAtFirst);
        }

        public IUIAutomationElement GetElement(string id)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            return GetElement(id, cacheRequest);
        }

        public IUIAutomationElement GetElement(string id, IUIAutomationCacheRequest cacheRequest)
        {
            cachedElement.TryGetValue(id, out var element);
            if (element == null) throw new StaleElementReference("element is stale");
            try
            {
                element = element.BuildUpdatedCache(cacheRequest);
                return element;
            }
            catch
            {
                cachedElement.Remove(id);
                throw new StaleElementReference("element is stale");
            }
        }

        public FindElementResponse GetActiveElement()
        {
            return GetActiveElement(null);
        }

        public FindElementResponse GetActiveElement(int? topLevelHdl)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
            var id = Guid.NewGuid().ToString();

            var active = automation.GetFocusedElementBuildCache(cacheRequest);
            if (topLevelHdl == null)
            {
                cachedElement[id] = active;
                return new FindElementResponse(id);
            }

            var prevHdl = 0;
            var hdl = 0;
            var walker = automation.CreateTreeWalker(automation.CreateTrueCondition());
            var pointer = active;
            while (true)
            {
                if (pointer == null) break;

                prevHdl = hdl;
                hdl = (int)pointer.GetCachedPropertyValue(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);
                pointer = walker.GetParentElementBuildCache(pointer, cacheRequest);
            }
            if (topLevelHdl == prevHdl)
            {
                cachedElement[id] = active;
                return new FindElementResponse(id);
            }
            throw new NoSuchElement("No active element on current window");
        }
    }
}
