using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Contracts;

namespace FileStorageApp.Data.InfoStorage.Storages.DocumentClassifications
{
    public interface IDocumentClassificationStorage : IDisposable
    {
        Task<bool> AddAsync(Classification classification);

        Task<bool> DeleteAsync(Guid id);

        Task<Classification?> FindByIdAsync(Guid id, bool includeClassificationWords = false);
        
        Task<List<Classification>> FindByQueryAsync(
            string? query,
            int skip,
            int take,
            bool includeClassificationWords = false
        );

        Task<bool> RenameAsync(Guid id, string newName);

        Task<Guid> AddWordAsync(Guid classificationId, ClassificationWord classificationWord);
        
        Task<bool> DeleteWordAsync(Guid wordId);

        Task<int> GetCountByQueryAsync(string? query);
        
        Task<List<ClassificationWord>> GetWordsByIdAsync(Guid classificationId);
        
        public Task<List<Classification>> GetAllAsync();
    }
}