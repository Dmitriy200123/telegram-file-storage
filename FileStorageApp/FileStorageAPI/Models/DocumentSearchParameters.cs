using System;
using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Параметры поиска документа
    /// </summary>
    public class DocumentSearchParameters
    {
        /// <summary>
        /// Фраза для поиска в имени или содержимом документа
        /// </summary>
        public string? Phrase { get; set; }

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
        
        // TODO: Добавить поле классификации
    }
}