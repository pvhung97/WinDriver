namespace UIADriver.actions.inputsource
{
    public abstract class InputSource
    {
        public abstract string GetSourceType();
        public abstract List<string> GetSupportedActions();
    }
}
