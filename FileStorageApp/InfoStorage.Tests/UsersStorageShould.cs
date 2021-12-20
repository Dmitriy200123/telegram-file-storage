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
    public class UsersStorageShould
    {
        private readonly IInfoStorageFactory _infoStorageFactory;

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        public UsersStorageShould()
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
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            foreach (var elem in await usersStorage.GetAllAsync())
                await usersStorage.DeleteAsync(elem.Id);
        }

        [Test]
        public async Task GetAll_CorrectData_WhenCalled()
        {
            var userId = Guid.NewGuid();
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var expected = new User
            {
                Id = userId,
                TelegramId = null,
                GitLabId = 1,
                Name = "",
                Avatar = "",
                RefreshToken = "",
            };
            await usersStorage.AddAsync(expected);

            var actual = await usersStorage.GetByIdAsync(userId);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}