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
    public class RightsStorageShould
    {
        private readonly IInfoStorageFactory _infoStorageFactory;

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        public RightsStorageShould()
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
            using var rightsStorage = _infoStorageFactory.CreateRightsStorage();
            foreach (var userRight in await rightsStorage.GetAllAsync())
                await rightsStorage.DeleteAsync(userRight.Id);
        }

        [Test]
        public async Task GetAll_CorrectData_WhenCalled()
        {
            var userId = Guid.NewGuid();
            await AddUserToDataBase(userId);
            var right = new Right
            {
                UserId = userId,
                Access = 0,
            };
            var expected = new[] {0};
            using var rightStorage = _infoStorageFactory.CreateRightsStorage();
            await rightStorage.AddAsync(right);

            var actual = await rightStorage.GetUserRightsAsync(userId);

            actual.Should().BeEquivalentTo(expected);
        }

        private async Task AddUserToDataBase(Guid userId)
        {
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var user = new User
            {
                Id = userId,
                TelegramId = null,
                GitLabId = 1,
                Name = "",
                Avatar = "",
                RefreshToken = "",
            };
            await usersStorage.AddAsync(user);
        }
    }
}