using System;
using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Информация о файлах
    /// </summary>
    public class File
    {
        /// <summary>
        /// Идентификатор файла.
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Тип файла
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Отправитель файла
        /// </summary>
        public Sender Sender { get; set; }

        /// <summary>
        /// Дата загрузки
        /// </summary>
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// Чат из которого был отправлен файл
        /// </summary>
        public Chat Chat { get; set; }

        /// <summary>
        /// Ссылка на скачивание файла
        /// </summary>
        public string DownloadLink { get; set; }
    }
}