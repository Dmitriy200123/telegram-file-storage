using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Models;

namespace FileStorageApp.Data.InfoStorage.Storages.DocumentClassifications
{
    public interface IDocumentClassificationStorage : IDisposable, IInfoStorage<DocumentClassification>
    {
        Task<DocumentClassification> FindByIdAsync(Guid id, bool includeClassificationWords = false);
        
        Task<List<DocumentClassification>> FindByQueryAsync(
            string query,
            int skip,
            int take,
            bool includeClassificationWords = false
        );

        Task<bool> AddWordAsync(Guid classificationId, DocumentClassificationWord classificationWord);
        
        Task<bool> DeleteWordAsync(Guid classificationId, Guid wordId);

        Task<int> GetCountByQueryAsync(string query);
        
        Task<List<DocumentClassificationWord>> GetWordsByIdAsync(Guid classificationId);
        
        public Task<List<DocumentClassification>> GetAllAsync();
    }
}