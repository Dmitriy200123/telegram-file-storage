using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Enums;

namespace FileStorageAPI.Converters
{
    /// <inheritdoc />
    public class DocumentToFileConverter : IDocumentToFileConverter
    {
        /// <inheritdoc />
        public FileSearchParameters ToFileSearchParameters( DocumentSearchParameters parameters)
        {
            return new()
            {
                FileName = parameters.Phrase,
                DateFrom = parameters.DateFrom,
                DateTo = parameters.DateTo,
                SenderIds = parameters.SenderIds,
                ChatIds = parameters.ChatIds,
                Categories = new[]
                {
                    FileType.TextDocument
                }
            };
        }
    }
}