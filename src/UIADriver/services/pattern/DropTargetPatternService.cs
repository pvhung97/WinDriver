namespace UIADriver.services.pattern
{
    public abstract class DropTargetPatternService<T, U> : PatternService<T, U>
    {
        protected DropTargetPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract List<string> GetDropTargetEffects(string elementId);
    }
}
