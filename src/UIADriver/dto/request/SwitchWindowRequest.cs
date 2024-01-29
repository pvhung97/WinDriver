using System.Text.Json.Nodes;
using UIA3Driver.exception;

namespace UIA3Driver.dto.request
{
    public class SwitchWindowRequest
    {
        public string handle;

        public SwitchWindowRequest(string handle) 
        {
            this.handle = handle;
        }

        public static SwitchWindowRequest Validate(JsonObject data)
        {
            data.TryGetPropertyValue("handle", out var hJson);
            if (hJson != null) 
            {
                try
                {
                    string hdl = hJson.AsValue().GetValue<string>();
                    if (hdl != null) return new SwitchWindowRequest(hdl);
                }
                catch { }
            }
            throw new InvalidArgument("handle value must be a string");
        }
    }
}
