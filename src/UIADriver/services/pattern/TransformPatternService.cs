namespace UIADriver.services.pattern
{
    public abstract class TransformPatternService<T, U> : PatternService<T, U>
    {
        protected TransformPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract void Move(string elementId, double x, double y);
        public abstract void Resize(string elementId, double width, double height);
        public abstract void Rotate(string elementId, double degrees);
    }
}
