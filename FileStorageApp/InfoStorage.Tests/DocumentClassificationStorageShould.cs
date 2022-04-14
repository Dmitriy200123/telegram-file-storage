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
        public async Task AddDocumentClassificationTable_ShouldAdd(string classificationName, string word)
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
        public async Task DeleteClassification_ShouldDelete(string classificationName)
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
        public async Task AddWordWithoutId_ShouldAdd(string classificationName, string wordValue)
        {
            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, classificationName);

            var word = new DocumentClassificationWord
            {
                Value = wordValue
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var result = await storage.AddWordAsync(classificationId, word);

            result.Should().BeTrue();

            var classification = await storage.FindByIdAsync(classificationId, true);
            var actualWord = classification.ClassificationWords.First();

            actualWord.Value.Should().Be(wordValue);
            actualWord.ClassificationId.Should().Be(classificationId);
        }

        [TestCase("abodovada", "gnome")]
        [TestCase("doaqewv", "abob")]
        [TestCase("document", "lol")]
        public async Task AddWordWithId_ShouldAdd(string classificationName, string wordValue)
        {
            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, classificationName);

            var wordId = Guid.NewGuid();
            var word = new DocumentClassificationWord
            {
                Id = wordId,
                Value = wordValue,
                ClassificationId = classificationId
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var result = await storage.AddWordAsync(classificationId, word);

            result.Should().BeTrue();

            var classification = await storage.FindByIdAsync(classificationId, true);
            var actualWord = classification.ClassificationWords.First();

            actualWord.Value.Should().Be(wordValue);
            actualWord.Id.Should().Be(wordId);
            actualWord.ClassificationId.Should().Be(classificationId);
        }

        [TestCase("abodovada", "gnome")]
        [TestCase("doaqewv", "abob")]
        [TestCase("document", "lol")]
        public async Task DeleteWord_ShouldDelete(string classificationName, string wordValue)
        {
            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, classificationName);

            var wordId = Guid.NewGuid();
            var word = new DocumentClassificationWord { Id = wordId, Value = wordValue };
            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();

            await storage.AddWordAsync(classificationId, word);

            var deleted = await storage.DeleteWordAsync(wordId);
            deleted.Should().BeTrue();

            var classification = await storage.FindByIdAsync(classificationId, true);
            classification.ClassificationWords.Should().BeEmpty();
        }

        [TestCase("a", 1, "aboba")]
        [TestCase("Ab", 3, "aBob", "ABIdo", "AbBb")]
        [TestCase("do", 2, "OdOb", "aqaq", "qWe", "toDO")]
        [TestCase("do", 0, "aqaq", "qWe")]
        public async Task GetCountByQuery_ShouldCorrectCount(
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
        public async Task GetWords_ShouldGetCorrectWords(string classificationName, params string[] wordValues)
        {
            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, classificationName);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();

            foreach (var wordValue in wordValues)
            {
                var word = new DocumentClassificationWord { Value = wordValue };
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
        public async Task RenameClassification_ShouldRenamed(string classificationName, string newClassificationName)
        {
            var classificationId = Guid.NewGuid();
            await AddClassification(classificationId, classificationName);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var classification = await storage.FindByIdAsync(classificationId);

            classification.Name = newClassificationName;
            await storage.UpdateAsync(classification);

            classification = await storage.FindByIdAsync(classificationId);
            classification.Name.Should().Be(newClassificationName);
        }

        private async Task AddClassification(Guid classificationId, string classificationName)
        {
            var classification = new DocumentClassification
            {
                Id = classificationId,
                Name = classificationName
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddAsync(classification);
        }
    }
}