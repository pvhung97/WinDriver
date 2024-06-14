using System.Text.Json.Nodes;
using UIADriver.exception;

namespace UIADriver.dto.request
{
    public class ScrollByAmountRequest
    {
        public string horizontalAmount;
        public string verticalAmount;

        public static ScrollByAmountRequest Validate(JsonObject data)
        {
            var request = new ScrollByAmountRequest();
            data.TryGetPropertyValue("horizontalAmount", out var hJson);

            try
            {
                request.horizontalAmount = hJson.GetValue<string>();
            }
            catch
            {
                throw new InvalidArgument("horizontalAmount value must be a string");
            }

            data.TryGetPropertyValue("verticalAmount", out var vJson);
            try
            {
                request.verticalAmount = vJson.GetValue<string>();
            }
            catch
            {
                throw new InvalidArgument("verticalAmount value must be a string");
            }

            return request;
        }
    }
}
