using Newtonsoft.Json;

namespace OctoNet.Models
{
    [JsonObject]
    public class Branch
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "commit")]
        public Commit Commit { get; set; }
    }
}