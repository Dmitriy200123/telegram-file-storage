using System.ComponentModel.DataAnnotations;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Обновление данных о файле.
    /// </summary>
    public class UpdateFile
    {
        /// <summary>
        /// Имя файла.
        /// </summary>
        [Required]
        public string FileName { get; set; }
    }
}