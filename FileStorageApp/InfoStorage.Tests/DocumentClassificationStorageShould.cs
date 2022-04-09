using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace InfoStorage.Tests
{
    public class DocumentClassificationStorage
    {
        private IInfoStorageFactory _infoStorageFactory;

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        [SetUp]
        public void SetUp()
        {
            var config = new DataBaseConfig($"Server={Config["DbHost"]};" +
                                            $"Username={Config["DbUser"]};" +
                                            $"Database={Config["DbName"]};" +
                                            $"Port={Config["DbPort"]};" +
                                            $"Password={Config["DbPassword"]};" +
                                            "SSLMode=Prefer");
            _infoStorageFactory = new InfoStorageFactory(config);
        }

        [TearDown]
        public async Task TearDown()
        {
            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();

            foreach (var element in await storage.GetAllAsync())
                await storage.DeleteAsync(element.Id);
        }

        [TestCase("Test classification", "joke")]
        [TestCase("Aboa", "model")]
        public async Task CreateDocumentClassificationTable_ShouldInitialize(string classificationName, string word)
        {
            var classification = new DocumentClassification
            {
                Id = Guid.NewGuid(),
                Name = classificationName
            };

            var words = new List<DocumentClassificationWord>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Classification = classification,
                    Value = word
                }
            };

            classification.ClassificationWords = words;

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var result = await storage.AddAsync(classification);

            result.Should().BeTrue();
        }

        [TestCase("Test classification", "joke")]
        [TestCase("Aboa", "model")]
        public async Task AddClassification_ShouldCorrectAddClassification(
            string classificationName,
            string word
        )
        {
            var classificationId = Guid.NewGuid();
            var classification = new DocumentClassification { Id = classificationId, Name = classificationName };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddAsync(classification);

            using var storage1 = _infoStorageFactory.CreateDocumentClassificationStorage();
            var actualClassification = await storage1.FindByIdAsync(classificationId);

            actualClassification.Should().BeEquivalentTo(classification);
        }

        [TestCase("Test classification", "joke", "aboba")]
        [TestCase("Aboa", "model", "logus")]
        [TestCase("Document", "model")]
        public async Task AddClassificationWithWords_ShouldCorrectAddWords(
            string classificationName,
            params string[] words
        )
        {
            var classificationId = Guid.NewGuid();
            var classification = new DocumentClassification
            {
                Id = classificationId,
                Name = classificationName
            };

            var documentsWords = words
                .Select(word => new DocumentClassificationWord
                    {
                        Id = Guid.NewGuid(),
                        Classification = classification,
                        Value = word
                    }
                )
                .ToList();

            classification.ClassificationWords = documentsWords;

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddAsync(classification);

            using var storage1 = _infoStorageFactory.CreateDocumentClassificationStorage();
            var actualClassification = await storage1.FindByIdAsync(classificationId, true);
            var actualWords = actualClassification.ClassificationWords;
            
           actualWords
                .Should()
                .BeEquivalentTo(
                    documentsWords,
                    options => options.Excluding(x => x.Classification)
                );

            foreach (var actualWord in actualWords)
            {
                actualWord.Classification.Should().NotBeNull();
            }
        }
        
        [TestCase("Document", "oCu")]
        [TestCase("Valley", "val")]
        [TestCase("abodovada", "BodoVada")]
        public async Task FindClassification_ShouldFindClassification(
            string classificationName,
            string query
        )
        {
            var classification = new DocumentClassification
            {
                Id = Guid.NewGuid(),
                Name = classificationName
            };
            
            var randomClassification = new DocumentClassification
            {
                Id = Guid.NewGuid(),
                Name = "random"
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddAsync(classification);
            await storage.AddAsync(randomClassification);

            using var storage1 = _infoStorageFactory.CreateDocumentClassificationStorage();
            var actualClassifications = await storage1.FindByQueryAsync(query, 0, 10);

            actualClassifications.Should().HaveCount(1);
            actualClassifications.FirstOrDefault().Should().BeEquivalentTo(classification);
        }
        
        [TestCase("abodovada")]
        [TestCase("doaqewv")]
        [TestCase("document")]
        public async Task DeleteClassification_ShouldDelete(
            string classificationName
        )
        {
            var classificationId = Guid.NewGuid();
            var classification = new DocumentClassification
            {
                Id = classificationId,
                Name = classificationName
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddAsync(classification);

            using var storage1 = _infoStorageFactory.CreateDocumentClassificationStorage();
            var deleted = await storage1.DeleteAsync(classificationId);

            deleted.Should().BeTrue();
        }
    }
}