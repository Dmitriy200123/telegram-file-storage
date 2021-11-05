using Microsoft.AspNetCore.Http;

namespace FileStorageAPI.Models
{
    public class UploadFile
    {
        public string SenderName { get; set; }
        public IFormFile File { get; set; }
    }
}