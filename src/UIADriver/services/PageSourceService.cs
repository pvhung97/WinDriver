using System.Xml.Linq;

namespace UIADriver.services
{
    public abstract class PageSourceService<T, U>
    {
        protected SessionCapabilities capabilities;
        protected ServiceProvider<T, U> serviceProvider;

        public PageSourceService(SessionCapabilities capabilities, ServiceProvider<T, U> serviceProvider)
        {
            this.capabilities = capabilities;

            this.serviceProvider = serviceProvider;
        }

        public abstract PageSource BuildPageSource(T startElement);
        public abstract List<T> FindElementByProperty(T topLevelWindow, string propertyName, string? propertyValue, bool stopAtFirst);
        public abstract List<T> ResolveXpath(T topLevelWindow, string xpath, bool stopAtFirst);

        public class PageSource
        {
            public XDocument pageSource;
            public Dictionary<XElement, T> mapping;

            public PageSource(XDocument pageSource, Dictionary<XElement, T> mapping)
            {
                this.pageSource = pageSource;
                this.mapping = mapping;
            }
        }
    }
}
