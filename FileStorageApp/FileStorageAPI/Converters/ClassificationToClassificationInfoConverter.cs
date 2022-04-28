using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageAPI.Converters
{
    /// <inheritdoc />
    public class ClassificationToClassificationInfoConverter
        : IClassificationToClassificationInfoConverter
    {
        /// <inheritdoc />
        public ClassificationInfo? ConvertToClassificationInfo(DocumentClassification? classification) =>
            classification == null
                ? null
                : new ClassificationInfo
                {
                    Id = classification.Id,
                    Name = classification.Name
                };
    }
}