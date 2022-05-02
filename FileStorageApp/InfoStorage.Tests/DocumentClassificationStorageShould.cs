using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Contracts;
using FileStorageApp.Data.InfoStorage.Factories;
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
        public async Task AddAsync_Added(string classificationName, string word)
        {
            var classification = new Classification
            {
                Id = Guid.NewGuid(),
                Name = classificationName
            };

            var words = new List<ClassificationWord>
            {
                new()
                {
                    Id = Guid.NewGuid(),
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
        public async Task AddAsync_Added_WhenClassificationWithoutWords(
            string classificationName,
            string word
        )
        {
            var classificationId = Guid.NewGuid();
            var classification = new Classification { Id = classificationId, Name = classificationName };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddAsync(classification);

            using var storage1 = _infoStorageFactory.CreateDocumentClassificationStorage();
            var actualClassification = await storage1.FindByIdAsync(classificationId);

            actualClassification.Should().BeEquivalentTo(classification);
        }

        [TestCase("Test classification", "joke", "aboba")]
        [TestCase("Aboa", "model", "logus")]
        [TestCase("Document", "model")]
        public async Task AddAsync_Added_WhenClassificationWithWords(
            string classificationName,
            params string[] words
        )
        {
            var classificationId = Guid.NewGuid();
            var classification = new Classification
            {
                Id = classificationId,
                Name = classificationName
            };

            var documentsWords = words
                .Select(word => new ClassificationWord
                    {
                        Id = Guid.NewGuid(),
                        ClassificationId = classificationId,
                        Value = word
                    }
                )
                .ToList();

            classification.ClassificationWords = documentsWords;

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddAsync(classification);

            using var storage1 = _infoStorageFactory.CreateDocumentClassificationStorage();
            var actualClassification = await storage1.FindByIdAsync(classificationId, true);
            actualClassification.Should().NotBeNull();
            
            var actualWords = actualClassification.ClassificationWords;

            actualWords
                .Should()
                .BeEquivalentTo(documentsWords);
        }

        [TestCase("Document", "oCu")]
        [TestCase("Valley", "val")]
        [TestCase("abodovada", "BodoVada")]
        public async Task FindByQueryAsync_Found_WhenCorrectQuery(
            string classificationName,
            string query
        )
        {
            var classification = new Classification
            {
                Id = Guid.NewGuid(),
                Name = classificationName
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddAsync(classification);

            await AddClassification(Guid.NewGuid(), "random");
            await AddClassification(Guid.NewGuid(), "random1");

            using var storage1 = _infoStorageFactory.CreateDocumentClassificationStorage();
            var actualClassifications = await storage1.FindByQueryAsync(query, 0, 10);

            actualClassifications.Should().HaveCount(1);
            actualClassifications.FirstOrDefault().Should().BeEquivalentTo(classification);
        }

        [TestCase("abodovada")]
        [TestCase("doaqewv")]
        [TestCase("document")]
        public async Task DeleteAsync_Deleted_WhenCorrectId(string classificationName)
        {
            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, classificationName);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var deleted = await storage.DeleteAsync(classificationId);

            deleted.Should().BeTrue();

            var classification = await storage.GetAllAsync();

            classification.Should().BeEmpty();
        }

        [TestCase("abodovada", "gnome")]
        [TestCase("doaqewv", "abob")]
        [TestCase("document", "lol")]
        public async Task AddWordAsync_Added_WhenCorrectId(string classificationName, string wordValue)
        {
            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, classificationName);

            var word = new ClassificationWord
            {
                Value = wordValue
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var result = await storage.AddWordAsync(classificationId, word);

            result.Should().NotBe(Guid.Empty);

            var classification = await storage.FindByIdAsync(classificationId, true);
            classification.Should().NotBeNull();
            
            var actualWord = classification.ClassificationWords.FirstOrDefault();

            actualWord.Should().NotBeNull();
            actualWord.Value.Should().Be(wordValue);
            actualWord.ClassificationId.Should().Be(classificationId);
        }

        [TestCase("abodovada", "gnome")]
        [TestCase("doaqewv", "abob")]
        [TestCase("document", "lol")]
        public async Task AddWordAsync_Added_WhenHaveId(string classificationName, string wordValue)
        {
            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, classificationName);

            var wordId = Guid.NewGuid();
            var word = new ClassificationWord
            {
                Id = wordId,
                Value = wordValue,
                ClassificationId = classificationId
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var result = await storage.AddWordAsync(classificationId, word);

            result.Should().NotBe(Guid.Empty);

            var classification = await storage.FindByIdAsync(classificationId, true);
            classification.Should().NotBeNull();
            
            var actualWord = classification.ClassificationWords.FirstOrDefault();

            actualWord.Should().NotBeNull();
            actualWord.Value.Should().Be(wordValue);
            actualWord.Id.Should().Be(wordId);
            actualWord.ClassificationId.Should().Be(classificationId);
        }

        [TestCase("abodovada", "gnome")]
        [TestCase("doaqewv", "abob")]
        [TestCase("document", "lol")]
        public async Task DeleteWordAsync_Deleted_WhenCorrectWordId(string classificationName, string wordValue)
        {
            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, classificationName);

            var wordId = Guid.NewGuid();
            var word = new ClassificationWord { Id = wordId, Value = wordValue };
            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();

            await storage.AddWordAsync(classificationId, word);

            var deleted = await storage.DeleteWordAsync(wordId);
            deleted.Should().BeTrue();

            var classification = await storage.FindByIdAsync(classificationId, true);
            
            classification.Should().NotBeNull();
            classification.ClassificationWords.Should().BeEmpty();
        }

        [TestCase("a", 1, "aboba")]
        [TestCase("Ab", 3, "aBob", "ABIdo", "AbBb")]
        [TestCase("do", 2, "OdOb", "aqaq", "qWe", "toDO")]
        [TestCase("do", 0, "aqaq", "qWe")]
        public async Task GetCountByQueryAsync_Count(
            string query,
            int expected,
            params string[] classificationNames
        )
        {
            foreach (var name in classificationNames)
                await AddClassification(Guid.NewGuid(), name);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var count = await storage.GetCountByQueryAsync(query);

            count.Should().Be(expected);
        }

        [TestCase("abodovada", "gnome", "abob")]
        [TestCase("doaqewv", "abob", "moqw", "fasc")]
        [TestCase("document", "lol")]
        public async Task GetWordsByIdAsync_Words_WhenCorrectClassificationId(string classificationName, params string[] wordValues)
        {
            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, classificationName);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();

            foreach (var wordValue in wordValues)
            {
                var word = new ClassificationWord { Value = wordValue };
                await storage.AddWordAsync(classificationId, word);
            }

            var actualWords = await storage.GetWordsByIdAsync(classificationId);

            actualWords.Should().HaveCount(wordValues.Length);
            actualWords.Select(word => word.Value).Should().BeEquivalentTo(wordValues);

            var classificationIds = actualWords.Select(word => word.ClassificationId);

            foreach (var actualId in classificationIds)
                actualId.Should().Be(classificationId);
        }
        
        [TestCase("document", "lol")]
        [TestCase("aboba", "dobw")]
        [TestCase("qwer", "holive")]
        public async Task RenameAsync_Renamed_WhenCorrectNewName(string classificationName, string newClassificationName)
        {
            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, classificationName);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var renamed = await storage.RenameAsync(classificationId, newClassificationName);

            renamed.Should().BeTrue();

            using var storage1 = _infoStorageFactory.CreateDocumentClassificationStorage();
            var classification = await storage1.FindByIdAsync(classificationId);

            classification.Should().NotBeNull();
            classification.Name.Should().Be(newClassificationName);
        }

        private async Task AddClassification(Guid classificationId, string classificationName)
        {
            var classification = new Classification
            {
                Id = classificationId,
                Name = classificationName
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddAsync(classification);
        }
    }
}