using Newtonsoft.Json;

namespace FileStorageAPI.Models
{
    public class GitLabUser
    {
        public int? Id { get; set; }
        public string? Name { get; set; }

        [JsonProperty(PropertyName = "avatar_url")]
        public string? AvatarUrl { get; set; }
    }
}