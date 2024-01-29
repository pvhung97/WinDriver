
namespace UIA3Driver.actions.inputsource
{
    public class WheelInpoutSource : NullInputSource
    {
        public override string GetSourceType()
        {
            return "wheel";
        }

        public override List<string> GetSupportedActions()
        {
            return ["pause", "scroll"];
        }
    }
}
