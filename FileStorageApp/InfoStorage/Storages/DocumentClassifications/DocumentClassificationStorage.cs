using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages.DocumentClassifications
{
    internal class DocumentClassificationStorage : BaseStorage<DocumentClassification>,
        IDocumentClassificationStorage
    {
        public DocumentClassificationStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
            Database.EnsureCreated();
        }

        public Task<DocumentClassification> FindByIdAsync(Guid id, bool includeClassificationWords = false)
        {
            var queryable = DbSet.AsQueryable();

            if (includeClassificationWords)
                queryable = queryable.Include(classification => classification.ClassificationWords);

            return queryable.FirstOrDefaultAsync(classification => classification.Id == id);
        }

        public Task<List<DocumentClassification>> FindByQueryAsync(
            string query,
            int skip,
            int take,
            bool includeClassificationWords = false
        )
        {
            var queryable = DbSet.AsQueryable();

            if (includeClassificationWords)
                queryable = queryable.Include(classification => classification.ClassificationWords);

            return queryable
                .Skip(skip)
                .Take(take)
                .Where(classification => classification.Name.ToLower().Contains(query.ToLower()))
                .ToListAsync();
        }

        public async Task<bool> AddWordAsync(Guid id, DocumentClassificationWord classificationWord)
        {
            var classification = await GetByIdAsync(id);

            if (classification.ClassificationWords.Any(word => word.Value == classificationWord.Value))
                throw new ArgumentException(
                    $"{nameof(DocumentClassificationWord)} with value {classificationWord.Value} already exist"
                );

            classificationWord.Classification = classification;
            classification.ClassificationWords.Add(classificationWord);

            return await UpdateAsync(classification);
        }

        public async Task<bool> DeleteWordAsync(Guid id, Guid wordId)
        {
            var classification = await GetByIdAsync(id);
            var classificationWord = classification.ClassificationWords.FirstOrDefault(word => word.Id == wordId);

            if (classificationWord == null)
                return false;
            
            classification.ClassificationWords.Remove(classificationWord);
                
            return await UpdateAsync(classification);
        }

        public Task<int> GetCountByQueryAsync(string query)
        {
            return DbSet
                .Where(classification => classification.Name.Contains(query))
                .CountAsync();
        }

        public async Task<List<DocumentClassificationWord>> GetWordsByIdAsync(Guid id)
        {
            var classification = await DbSet
                .Include(classification => classification.ClassificationWords)
                .FirstOrDefaultAsync(classification => classification.Id == id);

            return classification.ClassificationWords.ToList();
        }

        public Task<List<DocumentClassification>> GetAllAsync()
        {
            return DbSet.ToListAsync();
        }
    }
}