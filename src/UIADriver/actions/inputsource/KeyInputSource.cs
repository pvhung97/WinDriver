namespace UIADriver.actions.inputsource
{
    public class KeyInputSource : NullInputSource
    {
        public HashSet<char> pressed = [];
        public bool alt = false;
        public bool ctrl = false;
        public bool shift = false;
        public bool meta = false;

        public override string GetSourceType()
        {
            return "key";
        }

        public override List<string> GetSupportedActions()
        {
            return ["pause", "keyDown", "keyUp"];
        }
    }
}
