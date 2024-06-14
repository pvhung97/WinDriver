using System.Text.Json.Nodes;
using UIADriver.exception;

namespace UIADriver.dto.request
{
    public class MoveRequest
    {
        public double x;
        public double y;

        public static MoveRequest Validate(JsonObject data)
        {
            var request = new MoveRequest();
            data.TryGetPropertyValue("x", out var xJson);

            try
            {
                request.x = xJson.GetValue<double>();
            }
            catch
            {
                throw new InvalidArgument("x value must be a number");
            }
            

            data.TryGetPropertyValue("y", out var yJson);
            try
            {
                request.y = yJson.GetValue<double>();
            }
            catch
            {
                throw new InvalidArgument("y value must be a number");
            }

            return request;
        }
    }
}
