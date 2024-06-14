using System.Text.Json.Nodes;
using UIADriver.exception;

namespace UIADriver.dto.request
{
    public class GetGridItemRequest
    {
        public int row;
        public int column;

        public static GetGridItemRequest Validate(JsonObject data)
        {
            var request = new GetGridItemRequest();
            data.TryGetPropertyValue("row", out var xJson);

            try
            {
                request.row = xJson.GetValue<int>();
            }
            catch
            {
                throw new InvalidArgument("row value must be an integer");
            }

            data.TryGetPropertyValue("column", out var yJson);
            try
            {
                request.column = yJson.GetValue<int>();
            }
            catch
            {
                throw new InvalidArgument("column value must be an integer");
            }

            return request;
        }
    }
}
