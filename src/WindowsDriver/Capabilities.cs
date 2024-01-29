using System.Text.Json.Nodes;

namespace WindowsDriver
{
    public struct Capabilities
    {
        public MatchObject capabilities { get; set; }
    }

    public struct MatchObject
    {
        public JsonObject? alwaysMatch { get; set; }
        public JsonArray? firstMatch { get; set; }
    }

}
