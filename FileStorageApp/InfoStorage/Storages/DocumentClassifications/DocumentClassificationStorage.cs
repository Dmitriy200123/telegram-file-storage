using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApp.Data.InfoStorage.Storages.DocumentClassifications
{
    internal class DocumentClassificationStorage : BaseStorage<DocumentClassification>,
        IDocumentClassificationStorage
    {
        private DbSet<DocumentClassificationWord> ClassificationWords { get; set; }

        public DocumentClassificationStorage(IDataBaseConfig dataBaseConfig) : base(dataBaseConfig)
        {
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
        ) => AddOptionsInQuery(DbSet, includeClassificationWords, skip, take)
            .Where(classification => classification.Name.ToLower().Contains(query.ToLower()))
            .ToListAsync();

        private static IQueryable<DocumentClassification> AddOptionsInQuery(
            IQueryable<DocumentClassification> query,
            bool useInclude = false,
            int? skip = null,
            int? take = null
        )
        {
            if (useInclude)
                query = query.Include(x => x.ClassificationWords);

            if (skip != null)
                query = query.Skip(skip.Value);

            if (take != null)
                query = query.Take(take.Value);

            return query;
        }

        public async Task<bool> AddWordAsync(Guid classificationId, DocumentClassificationWord classificationWord)
        {
            var isAlreadyExist = ClassificationWords
                .Where(word => word.ClassificationId == classificationId)
                .Any(word => word.Value == classificationWord.Value);

            if (isAlreadyExist)
                throw new AlreadyExistException(
                    $"{nameof(DocumentClassificationWord)} with value {classificationWord.Value} already exist"
                );

            classificationWord.ClassificationId = classificationId;

            await ClassificationWords.AddAsync(classificationWord);

            return await TrySaveChangesAsync();
        }

        public async Task<bool> DeleteWordAsync(Guid classificationId, Guid wordId)
        {
            var classificationWord = ClassificationWords
                .FirstOrDefault(word => word.ClassificationId == classificationId && word.Id == wordId);

            if (classificationWord == null)
                return false;

            ClassificationWords.Remove(classificationWord);

            return await TrySaveChangesAsync();
        }

        public Task<int> GetCountByQueryAsync(string query) => DbSet
            .Where(classification => classification.Name.ToLower().Contains(query.ToLower()))
            .CountAsync();

        public Task<List<DocumentClassificationWord>> GetWordsByIdAsync(Guid classificationId) => ClassificationWords
            .Where(word => word.ClassificationId == classificationId)
            .ToListAsync();

        public Task<List<DocumentClassification>> GetAllAsync() => DbSet.ToListAsync();
    }
}