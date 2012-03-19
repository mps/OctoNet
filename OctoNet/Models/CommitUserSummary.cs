using System;
using Newtonsoft.Json;

namespace OctoNet.Models
{
    [JsonObject]
    public class CommitUserSummary
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }
    }
}