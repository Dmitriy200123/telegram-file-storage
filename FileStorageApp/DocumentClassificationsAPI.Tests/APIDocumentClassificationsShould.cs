using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using API.Tests;
using DocumentClassificationsAPI.Models;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using DocumentClassification = FileStorageApp.Data.InfoStorage.Contracts.Classification;
using DocumentClassificationWord = FileStorageApp.Data.InfoStorage.Contracts.ClassificationWord;

namespace DocumentClassificationsAPI.Tests
{
    public class APIDocumentClassificationsShould : BaseShould<Startup>
    {
        [TearDown]
        public async Task TearDown()
        {
            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var classifications = await storage.GetAllAsync();

            foreach (var chat in classifications)
                await storage.DeleteAsync(chat.Id);
        }

        [TestCase("abob")]
        [TestCase("cdvb")]
        [TestCase("qwerty")]
        public async Task AddClassification_Ok_WhenWithoutWords(string classificationName)
        {
            var classificationInsert = new ClassificationInsert
            {
                Name = classificationName
            };

            var json = JsonConvert.SerializeObject(classificationInsert);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using var apiClient = CreateHttpClient();
            var response = await apiClient.PostAsync($"/api/documentClassifications", stringContent);

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var classification = (await storage.GetAllAsync()).FirstOrDefault();

            classification.Should().NotBeNull();
            classification.Name.Should().Be(classificationName);
        }

        [TestCase("abob", "word1", "word2")]
        [TestCase("cdvb", "qwe")]
        [TestCase("qwerty", "basdas", "dasdasq")]
        public async Task AddClassification_Ok_WhenHaveWords(string classificationName, params string[] words)
        {
            var classificationInsert = new ClassificationInsert
            {
                Name = classificationName,
                ClassificationWords = words
                    .Select(word => new ClassificationWordInsert { Value = word })
                    .ToList()
            };

            var json = JsonConvert.SerializeObject(classificationInsert);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using var apiClient = CreateHttpClient();
            var response = await apiClient.PostAsync($"/api/documentClassifications", stringContent);

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var classification = (await storage.FindByQueryAsync(classificationName, 0, 1, true))
                .FirstOrDefault();

            classification.Should().NotBeNull();
            classification.Name.Should().Be(classificationName);
            classification.ClassificationWords.Select(word => word.Value).Should().BeEquivalentTo(words);
        }

        [TestCase("abob", "word1", "word2")]
        [TestCase("cdvb", "qwe")]
        [TestCase("qwerty", "basdas", "dasdasq")]
        public async Task FindClassificationById_Ok_WhenIncludeWords(string classificationName, params string[] words)
        {
            var classificationId = await CreateClassification(classificationName, words);

            using var apiClient = CreateHttpClient();
            var response =
                await apiClient.GetAsync(
                    $"/api/documentClassifications/{classificationId}?includeClassificationWords=true");

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var actualClassification = JsonConvert.DeserializeObject<Classification>(content);

            actualClassification.Should().NotBeNull();
            actualClassification.Name.Should().Be(classificationName);
            actualClassification.ClassificationWords.Select(word => word.Value).Should().BeEquivalentTo(words);
        }

        [TestCase("word", "word1", "word2")]
        [TestCase("qw", "qwe", "qwbrty")]
        [TestCase("as", "basdas", "dasdasq")]
        public async Task FindClassificationsByQuery_Ok_WhenSameSubstring(
            string query,
            params string[] classificationNames
        )
        {
            foreach (var classificationName in classificationNames)
                await CreateClassification(classificationName);

            using var apiClient = CreateHttpClient();
            var response = await apiClient.GetAsync($"/api/documentClassifications?query={query}&skip=0&take=10");

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var actualClassifications = JsonConvert.DeserializeObject<Classification[]>(content);

            actualClassifications.Should().HaveCount(classificationNames.Length);
            actualClassifications.Select(classification => classification.Name).Should()
                .BeEquivalentTo(classificationNames);
        }

        [TestCase("word")]
        [TestCase("qwbrty")]
        [TestCase("dasdasq")]
        public async Task DeleteClassification_NoContent(string classificationName)
        {
            var classificationId = await CreateClassification(classificationName);

            using var apiClient = CreateHttpClient();
            var response = await apiClient.DeleteAsync($"/api/documentClassifications/{classificationId}");

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var classification = await storage.FindByIdAsync(classificationId);

            classification.Should().BeNull();
        }

        [TestCase("word", "eqw")]
        [TestCase("qwbrty", "abod")]
        [TestCase("dasdasq", "wegfa")]
        public async Task RenameClassification_Ok(string classificationName, string newName)
        {
            var classificationId = await CreateClassification(classificationName);

            var contentValue = $"\"{newName}\"";
            var content = new StringContent(contentValue, Encoding.UTF8, "application/json");

            using var apiClient = CreateHttpClient();
            var response = await apiClient.PutAsync($"/api/documentClassifications/{classificationId}", content);

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var classification = await storage.FindByIdAsync(classificationId);

            classification.Should().NotBeNull();
            classification.Name.Should().Be(newName);
        }

        [TestCase(2, "word", "word1", "word2")]
        [TestCase(1, "abc", "abcd", "qwe")]
        [TestCase(1, "", "wegfa")]
        [TestCase(2, "", "wegfa", "bdfa")]
        public async Task GetClassificationsCountByQuery_Ok(
            int expectedCount,
            string query,
            params string[] classificationNames
        )
        {
            foreach (var classificationName in classificationNames)
                await CreateClassification(classificationName);

            using var apiClient = CreateHttpClient();
            var response = await apiClient.GetAsync($"/api/documentClassifications/count?query={query}");

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var countString = await response.Content.ReadAsStringAsync();
            var actualCount = int.Parse(countString);

            actualCount.Should().Be(expectedCount);
        }

        [TestCase("abob")]
        [TestCase("cdvb")]
        [TestCase("qwerty")]
        public async Task AddWordToClassification_Ok(string word)
        {
            var classificationId = await CreateClassification("test");
            var wordInsert = new ClassificationWordInsert { Value = word };

            var json = JsonConvert.SerializeObject(wordInsert);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using var apiClient = CreateHttpClient();
            var response =
                await apiClient.PostAsync($"/api/documentClassifications/{classificationId}/words", stringContent);

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var wordId = JsonConvert.DeserializeObject<Guid>(content);
            
            wordId.Should().NotBe(Guid.Empty);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var classificationWord = (await storage.GetWordsByIdAsync(classificationId)).FirstOrDefault();

            classificationWord.Should().NotBeNull();
            classificationWord.Value.Should().Be(word);
        }

        [TestCase("asdasd", "word")]
        [TestCase("qwbrty", "bas")]
        [TestCase("dasdasq", "qwfe")]
        public async Task DeleteWordFromClassification_NoContent(string classificationName, string word)
        {
            var classificationId = await CreateClassification(classificationName);
            var wordId = await CreateClassificationWord(classificationId, word);

            using var apiClient = CreateHttpClient();
            var response = await apiClient.DeleteAsync($"/api/documentClassifications/words/{wordId}");

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            var classificationWords = await storage.GetWordsByIdAsync(classificationId);

            classificationWords.Should().BeEmpty();
        }

        private async Task<Guid> CreateClassificationWord(Guid classificationId, string word)
        {
            var wordId = Guid.NewGuid();
            var classificationWordInsert = new DocumentClassificationWord
            {
                Id = wordId,
                Value = word
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddWordAsync(classificationId, classificationWordInsert);

            return wordId;
        }

        [TestCase("abob", "word1", "word2")]
        [TestCase("cdvb", "qwe")]
        [TestCase("qwerty", "basdas", "dasdasq")]
        public async Task GetWordsByClassificationId_Ok(string classificationName, params string[] words)
        {
            var classificationId = await CreateClassification(classificationName, words);

            using var apiClient = CreateHttpClient();
            var response = await apiClient.GetAsync($"/api/documentClassifications/{classificationId}/words");

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var classificationWords = JsonConvert.DeserializeObject<ClassificationWord[]>(content);

            classificationWords.Should().HaveCount(words.Length);
            classificationWords.Select(word => word.Value).Should().BeEquivalentTo(words);
        }

        private async Task<Guid> CreateClassification(string classificationName, params string[] words)
        {
            var classificationId = Guid.NewGuid();
            var classificationInsert = new DocumentClassification
            {
                Id = classificationId,
                Name = classificationName,
                ClassificationWords = words
                    .Select(word => new DocumentClassificationWord { Value = word })
                    .ToList()
            };

            using var storage = _infoStorageFactory.CreateDocumentClassificationStorage();
            await storage.AddAsync(classificationInsert);

            return classificationId;
        }
    }
}