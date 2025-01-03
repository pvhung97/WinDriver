namespace UIADriver.services.pattern
{
    public abstract class TransformPattern2Service<T, U> : PatternService<T, U>
    {
        protected TransformPattern2Service(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract void Zoom(string elementId, double value);
        public abstract void ZoomByUnit(string elementId, string zoomUnit);
    }
}
