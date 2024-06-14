using System.Text.Json.Nodes;
using UIADriver.exception;

namespace UIADriver.dto.request
{
    public class ResizeRequest
    {
        public double width;
        public double height;

        public static ResizeRequest Validate(JsonObject data)
        {
            var request = new ResizeRequest();
            data.TryGetPropertyValue("width", out var wJson);
            try
            {
                request.width = wJson.GetValue<double>();
            }
            catch
            {
                throw new InvalidArgument("width value must be a number");
            }

            data.TryGetPropertyValue("height", out var hJson);
            try
            {
                request.height = hJson.GetValue<double>();
            }
            catch
            {
                throw new InvalidArgument("height value must be a number");
            }
            return request;
        }
    }
}
