using System;
using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageAPI
{
    /// <summary>
    /// Параметры поиска файлов
    /// </summary>
    public class FileSearchParameters
    {
        /// <summary>
        /// Имя файла.
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Период даты "От" загрузки файла.
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Период даты "До" загрузки.
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Идентификаторы отправителей.
        /// </summary>
        public Guid[]? SenderIds { get; set; }

        /// <summary>
        /// Идентификаторы чатов.
        /// </summary>
        public Guid[]? ChatIds { get; set; }

        /// <summary>
        /// Категории файлов.
        /// </summary>
        public FileType[]? Categories { get; set; }
    }
}