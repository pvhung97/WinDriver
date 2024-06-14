using UIADriver.dto.response;

namespace UIADriver.services
{
    public abstract class ElementAttributeService<T>
    {
        public abstract string? GetAttributeString(T element, string attribute);
        public abstract string? GetAttributeString(T element, string attribute, bool updateCache);
        public abstract object? GetAttributeObject(T element, string attribute);
        public abstract string GetElementTagName(T element);
        public abstract string GetElementText(T element);
        public abstract bool IsElementEnabled(T element);
        public abstract bool IsElementSelected(T element);
        public abstract bool IsElementDisplayed(T element);
        public abstract RectResponse GetElementRect(T element, Point rootPoint);
    }
}
