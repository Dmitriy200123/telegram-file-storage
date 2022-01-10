using Newtonsoft.Json;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Данные о пользователе полученные от GitLab
    /// </summary>
    public class GitLabUser
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ссылка на аватар
        /// </summary>
        [JsonProperty(PropertyName = "avatar_url")]
        public string AvatarUrl { get; set; }
    }
}