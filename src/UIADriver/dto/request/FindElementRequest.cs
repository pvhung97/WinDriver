using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using UIA3Driver.exception;

namespace UIA3Driver.dto.request
{
    public class FindElementRequest
    {
        [JsonPropertyName("using")]
        public string strategy;
        public string value;

        public FindElementRequest(string strategy, string value)
        {
            this.strategy = strategy;
            this.value = value;
        }

        public static FindElementRequest Validate(JsonObject data)
        {
            data.TryGetPropertyValue("using", out var strat);
            if (strat == null || strat.GetValueKind() != JsonValueKind.String) throw new InvalidArgument("Location strategy must be a string");
            data.TryGetPropertyValue("value", out var v);
            if (v == null || v.GetValueKind() != JsonValueKind.String) throw new InvalidArgument("Selector must be a string");
            return new FindElementRequest(strat.ToString(), v.ToString());
        }

    }
}
