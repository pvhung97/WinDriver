namespace UIADriver.services.pattern
{
    public abstract class DockPatternService<T, U> : PatternService<T, U>
    {
        public DockPatternService(ServiceProvider<T, U> serviceProvider) : base (serviceProvider) { }

        public abstract void SetDockPosition(string elementId, string dockPosition);
        
    }
}
