using System;
using FileStorageAPI.Models;
using DataBaseFileInfo = FileStorageApp.Data.InfoStorage.Models.File;

namespace FileStorageAPI.Converters
{
    /// <inheritdoc />
    public class FileInfoConverter : IFileInfoConverter
    {
        private readonly IChatConverter _chatConverter;
        private readonly ISenderConverter _senderConverter;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="FileInfoConverter"/>
        /// </summary>
        /// <param name="chatConverter">Конвертор для преобразования чатов в API-контракты</param>
        /// <param name="senderConverter">Конвертор для преобразования отправителей в API-контракты</param>
        public FileInfoConverter(IChatConverter chatConverter, ISenderConverter senderConverter)
        {
            _chatConverter = chatConverter ?? throw new ArgumentNullException(nameof(chatConverter));
            _senderConverter = senderConverter ?? throw new ArgumentNullException(nameof(senderConverter));
        }

        /// <inheritdoc />
        public FileInfo ConvertFileInfo(DataBaseFileInfo file)
        {
            return new()
            {
                FileId = file.Id,
                FileName = file.Name,
                FileType = file.Type,
                Sender = _senderConverter.ConvertFileSender(file.FileSender),
                UploadDate = file.UploadDate,
                Chat = _chatConverter.ConvertToChatInApi(file.Chat),
            };
        }
    }
}