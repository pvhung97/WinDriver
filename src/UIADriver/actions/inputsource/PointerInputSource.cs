using UIADriver.win32native;
using static UIADriver.win32native.Win32Enum;

namespace UIADriver.actions.inputsource
{
    public class PointerInputSource : NullInputSource, IDisposable
    {
        public string subtype = "";
        public int pointerId;
        public HashSet<uint> pressed = [];
        public int x = 0;
        public int y = 0;

        public readonly nint device = 0; 

        public PointerInputSource(string subtype, int pointerId)
        {
            this.subtype = subtype;
            this.pointerId = pointerId;

            switch (subtype)
            {
                case "pen":
                    device = Win32Methods.CreateSyntheticPointerDevice(POINTER_INPUT_TYPE.PT_PEN, 1, POINTER_FEEDBACK_MODE.DEFAULT);
                    break;
                case "touch":
                    device = Win32Methods.CreateSyntheticPointerDevice(POINTER_INPUT_TYPE.PT_TOUCH, 1, POINTER_FEEDBACK_MODE.DEFAULT);
                    break;
            }
        }

        public override string GetSourceType()
        {
            return "pointer";
        }

        public override List<string> GetSupportedActions()
        {
            return ["pause", "pointerDown", "pointerUp", "pointerMove"];
        }

        public void Dispose()
        {
            if (device != 0)
            {
                Win32Methods.DestroySyntheticPointerDevice(device);
            }
        }

    }
}
