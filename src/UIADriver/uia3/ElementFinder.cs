using Interop.UIAutomationClient;
using UIADriver.dto.request;
using UIADriver.dto.response;
using UIADriver.exception;
using UIADriver.services;

namespace UIADriver.uia3
{
    public class ElementFinder : ElementFinderService<IUIAutomationElement, IUIAutomationCacheRequest>
    {
        private IUIAutomation automation;

        public ElementFinder(IUIAutomation automation, ServiceProvider<IUIAutomationElement, IUIAutomationCacheRequest> serviceProvider) : base(serviceProvider)
        {
            this.automation = automation;
        }

        public override FindElementResponse FindElement(FindElementRequest request, IUIAutomationElement topLevelWindow)
        {
            //  Both FindFirst and FindAll will miss any element that is neither ControlElement nor ContentElement
            var found = FindElements(request, topLevelWindow, true);
            if (found.Count == 0) throw new NoSuchElement("Cannot find element with given location strategy and selector");
            return found[0];
        }

        public override List<FindElementResponse> FindElements(FindElementRequest request, IUIAutomationElement topLevelWindow)
        {
            return FindElements(request, topLevelWindow, false);
        }

        private List<FindElementResponse> FindElements(FindElementRequest request, IUIAutomationElement topLevelWindow, bool stopAtFirst)
        {
            List<IUIAutomationElement> rs = [];
            switch (request.strategy)
            {
                case "xpath":
                    rs = FindElementsWithXpath(request.value, topLevelWindow, stopAtFirst);
                    break;
                case "name":
                    rs = FindElementsWithPropertyNameAndValue(Enum.GetName(UIA3PropertyEnum.Name), request.value, topLevelWindow, stopAtFirst);
                    break;
                case "automation id":
                    rs = FindElementsWithPropertyNameAndValue(Enum.GetName(UIA3PropertyEnum.AutomationId), request.value, topLevelWindow, stopAtFirst);
                    break;
                case "id":
                    rs = FindElementsWithPropertyNameAndValue(Enum.GetName(UIA3PropertyEnum.RuntimeId), request.value, topLevelWindow, stopAtFirst);
                    break;
                case "tag name":
                    rs = FindElementsWithPropertyNameAndValue(Enum.GetName(UIA3PropertyEnum.ControlType), request.value, topLevelWindow, stopAtFirst);
                    break;
                default:
                    throw new InvalidArgument("Unsupported location strategy " + request.strategy);
            }

            var resp = new List<FindElementResponse>();
            foreach (var item in rs)
            {
                var id = RegisterElement(item);
                resp.Add(new FindElementResponse(id));
            }
            return resp;
        }

        private List<IUIAutomationElement> FindElementsWithXpath(string xpath, IUIAutomationElement topLevelWindow, bool stopAtFirst)
        {
            return serviceProvider.GetPageSourceService().ResolveXpath(topLevelWindow, xpath, stopAtFirst);
        }

        private List<IUIAutomationElement> FindElementsWithPropertyNameAndValue(string propertyName, string value, IUIAutomationElement topLevelWindow, bool stopAtFirst)
        {
            return serviceProvider.GetPageSourceService().FindElementByProperty(topLevelWindow, propertyName, value, stopAtFirst);
        }

        public override IUIAutomationElement GetElement(string id)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            return GetElement(id, cacheRequest);
        }

        public override IUIAutomationElement GetElement(string id, IUIAutomationCacheRequest cacheRequest)
        {
            cachedElement.TryGetValue(id, out var element);
            if (element == null) throw new StaleElementReference("element might have been removed from screen");
            try
            {
                element = element.BuildUpdatedCache(cacheRequest);

                var treeWalker = automation.CreateTreeWalker(automation.CreateTrueCondition());
                if (treeWalker.GetParentElement(element) == null && automation.CompareElements(element, automation.GetRootElement()) == 0)
                {
                    throw new StaleElementReference("element might have been removed from screen");
                }
                return element;
            }
            catch (StaleElementReference)
            {
                RemoveElement(id);
                throw;
            }
            catch (Exception ex)
            {
                RemoveElement(id);
                throw new StaleElementReference("element is not accessible or might have been removed from screen: " + ex.Message);
            }
        }

        public override FindElementResponse GetActiveElement()
        {
            return GetActiveElement(null);
        }

        public override FindElementResponse GetActiveElement(int? topLevelHdl)
        {
            var cacheRequest = automation.CreateCacheRequest();
            cacheRequest.AddProperty(UIA_PropertyIds.UIA_NativeWindowHandlePropertyId);

            var active = automation.GetFocusedElementBuildCache(cacheRequest);
            if (topLevelHdl == null)
            {
                string id = RegisterElement(active);
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
                string id = RegisterElement(active);
                return new FindElementResponse(id);
            }
            throw new NoSuchElement("No active element on current window");
        }

        public override FindElementResponse FindElementFromParentElement(FindElementRequest request, string parentElementId)
        {
            IUIAutomationElement? startPoint = null;
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
            IUIAutomationElement? startPoint = null;
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
