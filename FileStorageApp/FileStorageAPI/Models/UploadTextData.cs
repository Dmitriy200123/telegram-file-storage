using System.ComponentModel.DataAnnotations;

namespace FileStorageAPI.Models
{
    public class UploadTextData
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
    }
}