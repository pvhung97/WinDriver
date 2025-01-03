namespace UIADriver.services.pattern
{
    public abstract class ScrollPatternService<T, U> : PatternService<T, U>
    {
        protected ScrollPatternService(ServiceProvider<T, U> serviceProvider) : base(serviceProvider) { }

        public abstract void Scroll(string elementId, string horizontalAmount, string verticalAmount);
        public abstract void SetScrollPercent(string elementId, double horizontalPercent, double verticalPercent);
    }
}
