using Newtonsoft.Json;

namespace OctoNet.Models.Dto
{
    [JsonObject]
    public class IssueDto
    {
        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("body")]
        public string body { get; set; }

        [JsonProperty("assignee")]
        public string assignee { get; set; }

        [JsonProperty("milestone")]
        public string milestone { get; set; }

        [JsonProperty("labels")]
        public string[] labels { get; set; }
    }
}