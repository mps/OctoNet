﻿using System;
using Newtonsoft.Json;

namespace OctoNet.Models
{
    [JsonObject]
    public class User
    {
        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }

        [JsonProperty(PropertyName = "1")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty(PropertyName = "gravatar_id")]
        public string GravatarId { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "company")]
        public string Company { get; set; }

        [JsonProperty(PropertyName = "blog")]
        public string BlogUrl { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "hireable")]
        public bool Hireable { get; set; }

        [JsonProperty(PropertyName = "bio")]
        public string Bio { get; set; }

        [JsonProperty(PropertyName = "public_repos")]
        public int PublicRepos { get; set; }

        [JsonProperty(PropertyName = "public_gists")]
        public int PublicGists { get; set; }

        [JsonProperty(PropertyName = "following")]
        public int Following { get; set; }

        [JsonProperty(PropertyName = "followers")]
        public int Followers { get; set; }

        [JsonProperty(PropertyName = "html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonIgnore]
        public bool IsOrganization
        {
            get
            {
                return Type != null &&
                       Type.ToLower() == "organization";
            }
        }

        [JsonProperty(PropertyName = "total_private_repos")]
        public int TotalPrivateRepos { get; set; }

        [JsonProperty(PropertyName = "owned_private_repos")]
        public int OwnedPrivateRepos { get; set; }

        [JsonProperty(PropertyName = "private_gists")]
        public int PrivateGists { get; set; }

        [JsonProperty(PropertyName = "collaborators")]
        public int Collaborators { get; set; }

        [JsonProperty(PropertyName = "disk_usage")]
        public int DiskUsage { get; set; }

        [JsonProperty(PropertyName = "plan")]
        public Plan Plan { get; set; }
    }
}