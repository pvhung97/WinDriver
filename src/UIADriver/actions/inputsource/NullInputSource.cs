namespace UIADriver.actions.inputsource
{
    public class NullInputSource : InputSource
    {
        public override string GetSourceType()
        {
            return "none";
        }

        public override List<string> GetSupportedActions()
        {
            return ["pause"];
        }
    }
}
