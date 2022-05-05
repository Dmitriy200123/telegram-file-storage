using System.Linq;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Contracts
{
    internal static class ClassificationMapper
    {
        public static Classification ToClassification(this DocumentClassification documentClassification)
        {
            var words = documentClassification
                .ClassificationWords
                .Select(word => word.ToClassificationWord())
                .ToList();

            return new Classification
            {
                Id = documentClassification.Id,
                Name = documentClassification.Name,
                ClassificationWords = words
            };
        }
        
        public static ClassificationWord ToClassificationWord(
            this DocumentClassificationWord documentClassificationWord
        ) => new()
        {
            Id = documentClassificationWord.Id,
            Value = documentClassificationWord.Value,
            ClassificationId = documentClassificationWord.ClassificationId
        };

        public static DocumentClassification ToDocumentClassification(this Classification classification)
        {
            var words = classification
                .ClassificationWords
                .Select(word => word.ToDocumentClassificationWord())
                .ToList();

            return new DocumentClassification
            {
                Id = classification.Id,
                Name = classification.Name,
                ClassificationWords = words
            };
        }

        public static DocumentClassificationWord ToDocumentClassificationWord(
            this ClassificationWord classificationWord
        ) => new()
        {
            Id = classificationWord.Id,
            Value = classificationWord.Value,
            ClassificationId = classificationWord.ClassificationId
        };
    }
}