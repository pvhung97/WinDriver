using System.Windows.Automation;
using System.Xml.XPath;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.uia2.sourcebuilder;

namespace UIADriver.uia2
{
    public class ElementFinder
    {
        private PageSourceBuilder sourceBuilder;
        private Dictionary<string, AutomationElement> cachedElement;

        public ElementFinder(PageSourceBuilder sourceBuilder)
        {
            this.sourceBuilder = sourceBuilder;
            this.cachedElement = [];
        }

        public void resetCache()
        {
            cachedElement.Clear();
        }

        public FindElementResponse FindElement(FindElementRequest request, AutomationElement topLevelWindow)
        {
            //  Both FindFirst and FindAll will miss any element that is neither ControlElement nor ContentElement
            var found = FindElements(request, topLevelWindow, true);
            if (found.Count == 0) throw new NoSuchElement("Cannot find element with given location strategy and selector");
            return found[0];
        }

        public List<FindElementResponse> FindElements(FindElementRequest request, AutomationElement topLevelWindow)
        {
            return FindElements(request, topLevelWindow, false);
        }

        private List<FindElementResponse> FindElements(FindElementRequest request, AutomationElement topLevelWindow, bool stopAtFirst)
        {
            List<AutomationElement> rs = [];
            switch (request.strategy)
            {
                case "xpath":
                    rs = FindElementsWithXpath(request.value, topLevelWindow);
                    break;
                case "name":
                    rs = FindElementsWithPropertIdAndValue(AutomationElement.NameProperty, request.value, topLevelWindow, stopAtFirst);
                    break;
                case "automation id":
                    rs = FindElementsWithPropertIdAndValue(AutomationElement.AutomationIdProperty, request.value, topLevelWindow, stopAtFirst);
                    break;
                case "id":
                    rs = FindElementsWithPropertIdAndValue(AutomationElement.RuntimeIdProperty, request.value, topLevelWindow, stopAtFirst);
                    break;
                case "tag name":
                    rs = FindElementsWithPropertIdAndValue(AutomationElement.ControlTypeProperty, request.value, topLevelWindow, stopAtFirst);
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

        private List<AutomationElement> FindElementsWithXpath(string xpath, AutomationElement topLevelWindow)
        {
            var source = sourceBuilder.buildPageSource(topLevelWindow);
            var nodes = source.pageSource.XPathSelectElements(xpath);
            var rs = new List<AutomationElement>();

            foreach (var node in nodes)
            {
                source.mapping.TryGetValue(node, out var result);
                if (result != null) rs.Add(result);
            }
            return rs;
        }

        private List<AutomationElement> FindElementsWithPropertIdAndValue(AutomationProperty propertyId, string value, AutomationElement topLevelWindow, bool stopAtFirst)
        {
            return sourceBuilder.findElementByProperty(topLevelWindow, propertyId, value, stopAtFirst);
        }

        public AutomationElement GetElement(string id)
        {
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            return GetElement(id, cacheRequest);
        }

        public AutomationElement GetElement(string id, CacheRequest cacheRequest)
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
            var cacheRequest = new CacheRequest();
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            var id = Guid.NewGuid().ToString();

            var active = AutomationElement.FocusedElement.GetUpdatedCache(cacheRequest);
            if (topLevelHdl == null)
            {
                cachedElement[id] = active;
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
                cachedElement[id] = active;
                return new FindElementResponse(id);
            }
            throw new NoSuchElement("No active element on current window");
        }
    }
}
