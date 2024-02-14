using System.Text.Json.Nodes;
using UIADriver.exception;

namespace UIADriver.dto.request
{
    public class SetRectRequest
    {
        public int? x;
        public int? y;
        public int? width;
        public int? height;

        public static SetRectRequest Validate(JsonObject data)
        {
            var request = new SetRectRequest();
            data.TryGetPropertyValue("x", out var xJson);
            if (xJson != null)
            {
                try
                {
                    request.x = xJson.GetValue<int>();
                }
                catch
                {
                    throw new InvalidArgument("x value must be an integer");
                }
            }

            data.TryGetPropertyValue("y", out var yJson);
            if (yJson != null)
            {
                try
                {
                    request.y = yJson.GetValue<int>();
                }
                catch
                {
                    throw new InvalidArgument("y value must be an integer");
                }
            }

            data.TryGetPropertyValue("width", out var wJson);
            if (wJson != null)
            {
                try
                {
                    request.width = wJson.GetValue<int>();
                }
                catch
                {
                    throw new InvalidArgument("width value must be an integer");
                }
            }

            data.TryGetPropertyValue("height", out var hJson);
            if (hJson != null)
            {
                try
                {
                    request.height = hJson.GetValue<int>();
                }
                catch
                {
                    throw new InvalidArgument("height value must be an integer");
                }
            }
            return request;
        }
    }
}
