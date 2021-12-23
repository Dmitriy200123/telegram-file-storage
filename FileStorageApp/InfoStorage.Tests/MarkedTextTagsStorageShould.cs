using System;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace InfoStorage.Tests
{
    public class MarkedTextTagsStorageShould
    {
        private readonly IInfoStorageFactory _infoStorageFactory;

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        public MarkedTextTagsStorageShould()
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
            using var markedTextTagsStorage = _infoStorageFactory.CreateMarkedTextTagsStorage();

            var element = markedTextTagsStorage.FirstOrDefault();
            while (element is not null)
            {
                await markedTextTagsStorage.DeleteAsync(element.Id);
                element = markedTextTagsStorage.FirstOrDefault();
            }
        }

        [Test]
        public async Task FirstOrDefault_MarkedTextTags_WhenContainsElement()
        {
            using var markedTextTagsStorage = _infoStorageFactory.CreateMarkedTextTagsStorage();
            var element = new MarkedTextTags {Id = Guid.NewGuid(), TitleTag = "<title>", DescriptionTag = "<description>"};
            await markedTextTagsStorage.AddAsync(element);

            var act = markedTextTagsStorage.FirstOrDefault();

            act.Should().BeEquivalentTo(element);
        }

        [Test]
        public void FirstOrDefault_Default_WhenDoesNotContainElement()
        {
            using var markedTextTagsStorage = _infoStorageFactory.CreateMarkedTextTagsStorage();

            var act = markedTextTagsStorage.FirstOrDefault();

            act.Should().BeNull();
        }
    }
}