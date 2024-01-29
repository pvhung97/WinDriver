using System.Text.Json.Serialization;

namespace UIA3Driver.dto.response
{
    public class FindElementResponse
    {
        [JsonPropertyName("element-6066-11e4-a52e-4f735466cecf")]
        public string id { get; set; }

        public FindElementResponse(string id)
        {
            this.id = id;
        }
    }
}
