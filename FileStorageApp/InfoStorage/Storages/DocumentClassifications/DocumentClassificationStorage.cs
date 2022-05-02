using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Contracts;
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

        public Task<bool> AddAsync(Classification classification)
        {
            return AddAsync(classification.ToDocumentClassification());
        }

        public async Task<Classification?> FindByIdAsync(Guid id, bool includeClassificationWords = false)
        {
            var queryable = DbSet.AsQueryable();

            if (includeClassificationWords)
                queryable = queryable.Include(classification => classification.ClassificationWords);

            var documentClassification = await queryable.FirstOrDefaultAsync(classification => classification.Id == id);

            return documentClassification?.ToClassification();
        }

        public Task<List<Classification>> FindByQueryAsync(
            string? query,
            int skip,
            int take,
            bool includeClassificationWords = false
        )
        {
            var queryable = DbSet
                .AsQueryable()
                .OrderByDescending(classification => classification.CreatedAt);

            return AddOptionsInQuery(queryable, query, includeClassificationWords, skip, take)
                .Select(documentClassification => documentClassification.ToClassification())
                .ToListAsync();
        }

        public async Task<bool> RenameAsync(Guid id, string newName)
        {
            var classification = await GetByIdAsync(id);

            if (classification == null)
                throw new NotFoundException($"{nameof(Classification)} with Id {id} not found");

            var classificationWithNewName = await DbSet
                .Where(documentClassification => documentClassification.Name == newName)
                .FirstOrDefaultAsync();

            if (classificationWithNewName != null)
                throw new AlreadyExistException($"{nameof(Classification)} with Name {newName} already exist");

            classification.Name = newName;

            return await UpdateAsync(classification);
        }

        public async Task<Guid> AddWordAsync(Guid classificationId, ClassificationWord classificationWord)
        {
            var isAlreadyExist = ClassificationWords
                .Where(word => word.ClassificationId == classificationId)
                .Any(word => word.Value == classificationWord.Value);

            if (isAlreadyExist)
                throw new AlreadyExistException(
                    $"{nameof(ClassificationWord)} with value {classificationWord.Value} already exist"
                );

            var classificationExist = await ContainsAsync(classificationId);

            if (!classificationExist)
                throw new NotFoundException($"{nameof(Classification)} with Id {classificationId} not found");

            classificationWord.ClassificationId = classificationId;

            var documentClassificationWord = classificationWord.ToDocumentClassificationWord();
            
            await ClassificationWords.AddAsync(documentClassificationWord);
            await TrySaveChangesAsync();

            return documentClassificationWord.Id;
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

        public async Task<List<ClassificationWord>> GetWordsByIdAsync(Guid classificationId)
        {
            var classification = await GetByIdAsync(classificationId);

            if (classification == null)
                throw new NotFoundException($"{nameof(DocumentClassification)} with Id {classificationId} not found");

            return await ClassificationWords
                .Where(word => word.ClassificationId == classificationId)
                .Select(word => word.ToClassificationWord())
                .ToListAsync();
        }

        public Task<List<Classification>> GetAllAsync() => DbSet
            .Select(documentClassification => documentClassification.ToClassification())
            .ToListAsync();
    }
}