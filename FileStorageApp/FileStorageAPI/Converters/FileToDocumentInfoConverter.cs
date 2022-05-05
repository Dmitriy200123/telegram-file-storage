using System;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    /// <inheritdoc />
    public class FileToDocumentInfoConverter : IFileToDocumentInfoConverter
    {
        private readonly IChatConverter _chatConverter;
        private readonly ISenderConverter _senderConverter;
        private readonly IClassificationToClassificationInfoConverter _classificationConverter;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="FileToDocumentInfoConverter"/>
        /// </summary>
        /// <param name="chatConverter">Конвертер модели чатов</param>
        /// <param name="senderConverter">Конвертер модели отправителей</param>
        /// <param name="classificationConverter">Конвертер модели классификаций</param>
        /// <exception cref="ArgumentNullException">Выбросится, если один из параметров равен null</exception>
        public FileToDocumentInfoConverter(
            IChatConverter chatConverter,
            ISenderConverter senderConverter,
            IClassificationToClassificationInfoConverter classificationConverter
        )
        {
            _chatConverter = chatConverter ?? throw new ArgumentNullException(nameof(chatConverter));
            _senderConverter = senderConverter ?? throw new ArgumentNullException(nameof(senderConverter));
            _classificationConverter = classificationConverter ??
                                       throw new ArgumentNullException(nameof(classificationConverter));
        }

        /// <inheritdoc />
        public DocumentInfo ConvertToDocumentInfo(File file) => new()
        {
            DocumentId = file.Id,
            DocumentName = file.Name,
            Sender = _senderConverter.ConvertFileSender(file.FileSender),
            UploadDate = file.UploadDate,
            Chat = _chatConverter.ConvertToChatInApi(file.Chat),
            Classification = _classificationConverter.ConvertToClassificationInfo(file.Classification)
        };
    }
}