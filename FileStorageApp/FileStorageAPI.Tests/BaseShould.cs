using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FakeItEasy;
using FileStorageAPI.Providers;
using FileStorageAPI.Tests.AuthForTests;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RightServices;

namespace FileStorageAPI.Tests
{
    public abstract class BaseShould
    {
        protected static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        protected readonly IInfoStorageFactory _infoStorageFactory;

        private readonly WebApplicationFactory<Startup> _applicationFactory;
        private readonly IRightsFilter _fakeRightsFilter;
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;
        private readonly ISenderFromTokenProvider _senderFromTokenProvider;
        private readonly IAccessesByUserIdProvider _accessesByUserIdProvider;
        private const string SenderId = "00000000-0000-0000-0000-000000000001";

        protected BaseShould(FakeAuthUser fakeAuthUser = null)
        {
            var sender = new FileSender
            {
                Id = Guid.Parse(SenderId),
                TelegramId = -1,
                TelegramUserName = "Загрузчик с сайта",
                FullName = "Загрузчик с сайта",
            };
            _userIdFromTokenProvider = A.Fake<IUserIdFromTokenProvider>();
            _accessesByUserIdProvider = A.Fake<IAccessesByUserIdProvider>();
            A.CallTo(() => _userIdFromTokenProvider.GetUserIdFromToken(A<HttpRequest>.Ignored, A<byte[]>.Ignored))
                .Returns(Guid.Parse(SenderId));
            A.CallTo(() => _accessesByUserIdProvider.GetAccessesByUserIdAsync(A<Guid>.Ignored)).Returns(new[] { Access.ViewAnyFiles });
            _fakeRightsFilter = A.Fake<IRightsFilter>();
            A.CallTo(() => _fakeRightsFilter.CheckRightsAsync(A<ActionExecutingContext>.Ignored, A<int[]>.Ignored))
                .Returns(true);
            _senderFromTokenProvider = A.Fake<ISenderFromTokenProvider>();
            A.CallTo(() => _senderFromTokenProvider.GetSenderFromToken(A<HttpRequest>.Ignored))
                .Returns(Task.FromResult(sender));
            _applicationFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Development");
                    builder.UseConfiguration(Config);
                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton(_fakeRightsFilter);
                        services.AddSingleton(fakeAuthUser ?? new FakeAuthUser());
                        services.AddFakeAuthentication();
                        services.AddSingleton(_userIdFromTokenProvider);
                        services.AddSingleton(_senderFromTokenProvider);
                        services.AddSingleton(_accessesByUserIdProvider);
                    });
                });

            var dbConfig = new DataBaseConfig($"Server={Config["DbHost"]};" +
                                              $"Username={Config["DbUser"]};" +
                                              $"Database={Config["DbName"]};" +
                                              $"Port={Config["DbPort"]};" +
                                              $"Password={Config["DbPassword"]};" +
                                              "SSLMode=Prefer");
            _infoStorageFactory = new InfoStorageFactory(dbConfig);
        }

        protected HttpClient CreateHttpClient()
        {
            var client = _applicationFactory.CreateClient();
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}