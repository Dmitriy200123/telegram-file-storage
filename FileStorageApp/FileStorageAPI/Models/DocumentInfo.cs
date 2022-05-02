using System;

namespace FileStorageAPI.Models
{
    /// <summary>
    /// Информация о документах
    /// </summary>
    public class DocumentInfo
    {
        /// <summary>
        /// Идентификатор документа.
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Имя документа
        /// </summary>
        public string DocumentName { get; set; }

        /// <summary>
        /// Отправитель документа
        /// </summary>
        public Sender Sender { get; set; }

        /// <summary>
        /// Дата загрузки
        /// </summary>
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// Чат из которого был отправлен документ
        /// </summary>
        public Chat Chat { get; set; }
        
        /// <summary>
        /// Прикрепленная классификация
        /// </summary>
        public ClassificationInfo? Classification { get; set; }
    }
}