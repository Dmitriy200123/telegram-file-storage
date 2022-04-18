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

        public async Task<DocumentClassification?> FindByIdAsync(Guid id, bool includeClassificationWords = false)
        {
            var queryable = DbSet.AsQueryable();

            if (includeClassificationWords)
                queryable = queryable.Include(classification => classification.ClassificationWords);

            return await queryable.FirstOrDefaultAsync(classification => classification.Id == id);
        }

        public Task<List<DocumentClassification>> FindByQueryAsync(
            string? query,
            int skip,
            int take,
            bool includeClassificationWords = false
        ) => AddOptionsInQuery(DbSet, query, includeClassificationWords, skip, take).ToListAsync();

        public async Task<bool> RenameAsync(Guid id, string newName)
        {
            var classification = await FindByIdAsync(id);

            if (classification == null)
                throw new NotFoundException($"{nameof(DocumentClassification)} with Id {id} not found");

            var classificationWithNewName = DbSet
                .Where(documentClassification => documentClassification.Name == newName)
                .FirstOrDefaultAsync();

            if (classificationWithNewName != null)
                throw new AlreadyExistException($"{nameof(DocumentClassification)} with Name {newName} already exist");

            classification.Name = newName;

            return await UpdateAsync(classification);
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

        public async Task<bool> DeleteWordAsync(Guid wordId)
        {
            var classificationWord = await ClassificationWords.FirstOrDefaultAsync(word => word.Id == wordId);

            if (classificationWord == null)
                throw new NotFoundException($"Not found {nameof(DocumentClassificationWord)} with Id {wordId}");

            ClassificationWords.Remove(classificationWord);

            return await TrySaveChangesAsync();
        }

        public Task<int> GetCountByQueryAsync(string? query) => AddOptionsInQuery(DbSet, query).CountAsync();

        private static IQueryable<DocumentClassification> AddOptionsInQuery(
            IQueryable<DocumentClassification> queryable,
            string? query = null,
            bool useInclude = false,
            int? skip = null,
            int? take = null
        )
        {
            if (query != null)
                queryable = queryable.Where(classification => classification.Name.ToLower().Contains(query.ToLower()));

            if (useInclude)
                queryable = queryable.Include(x => x.ClassificationWords);

            if (skip != null)
                queryable = queryable.Skip(skip.Value);

            if (take != null)
                queryable = queryable.Take(take.Value);

            return queryable;
        }

        public Task<List<DocumentClassificationWord>> GetWordsByIdAsync(Guid classificationId) => ClassificationWords
            .Where(word => word.ClassificationId == classificationId)
            .ToListAsync();

        public Task<List<DocumentClassification>> GetAllAsync() => DbSet.ToListAsync();
    }
}