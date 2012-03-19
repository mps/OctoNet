using Newtonsoft.Json;

namespace OctoNet.Models
{
    [JsonObject]
    public class Link
    {
        [JsonProperty(PropertyName = "href")]
        public string HRef { get; set; }
    }
}