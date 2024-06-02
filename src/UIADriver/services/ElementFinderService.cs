using UIADriver.dto.request;
using UIADriver.dto.response;

namespace UIADriver.services
{
    public abstract class ElementFinderService<T, U>
    {
        protected Dictionary<string, T> cachedElement;
        protected Dictionary<string, string> cachedRuntimeId;

        protected PageSourceService<T> pageSourceService;
        protected ElementAttributeService<T> attrService;

        public ElementFinderService(PageSourceService<T> pageSourceService, ElementAttributeService<T> attrService)
        {
            this.cachedElement = [];
            this.cachedRuntimeId = [];

            this.pageSourceService = pageSourceService;
            this.attrService = attrService;
        }

        public void ResetCache()
        {
            cachedElement.Clear();
            cachedRuntimeId.Clear();
        }

        public string RegisterElement(T element)
        {
            string? runtimeId = attrService.GetAttributeString(element, "RuntimeId");
            if (runtimeId == null || runtimeId.Length == 0)
            {
                string newId = Guid.NewGuid().ToString();
                cachedElement.Add(newId, element);
                return newId;
            }

            cachedRuntimeId.TryGetValue(runtimeId, out var elementId);
            if (elementId != null) return elementId;

            string id = Guid.NewGuid().ToString();
            cachedElement.Add(id, element);
            cachedRuntimeId[runtimeId] = id;
            return id;
        }

        protected void RemoveElement(string elementId)
        {
            cachedElement.Remove(elementId);
            var toRemove = cachedRuntimeId.Where(entry => entry.Value.Equals(elementId)).Select(entry => entry.Key).ToList();
            foreach (var item in toRemove)
            {
                cachedRuntimeId.Remove(item);
            }
        }

        public abstract T GetElement(string id);
        public abstract T GetElement(string id, U cacheRequest);
        public abstract FindElementResponse FindElement(FindElementRequest request, T topLevelWindow);
        public abstract List<FindElementResponse> FindElements(FindElementRequest request, T topLevelWindow);
        public abstract FindElementResponse FindElementFromParentElement(FindElementRequest request, string parentElementId);
        public abstract List<FindElementResponse> FindElementsFromParentElement(FindElementRequest request, string parentElementId);
        public abstract FindElementResponse GetActiveElement();
        public abstract FindElementResponse GetActiveElement(int? topLevelHdl);
    }
}
