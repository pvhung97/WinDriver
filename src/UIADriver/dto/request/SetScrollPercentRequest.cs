using System.Text.Json.Nodes;
using UIADriver.exception;

namespace UIADriver.dto.request
{
    public class SetScrollPercentRequest
    {
        public double horizontalPercent;
        public double verticalPercent;

        public static SetScrollPercentRequest Validate(JsonObject data)
        {
            var request = new SetScrollPercentRequest();
            data.TryGetPropertyValue("horizontalPercent", out var hJson);

            try
            {
                request.horizontalPercent = hJson.GetValue<double>();
            }
            catch
            {
                throw new InvalidArgument("horizontalPercent value must be a number");
            }

            data.TryGetPropertyValue("verticalPercent", out var vJson);
            try
            {
                request.verticalPercent = vJson.GetValue<double>();
            }
            catch
            {
                throw new InvalidArgument("verticalPercent value must be a number");
            }

            return request;
        }
    }
}
