using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using FakeItEasy;
using FileStorageAPI.RightsFilters;
using FileStorageAPI.Tests.AuthForTests;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        protected BaseShould(FakeAuthUser fakeAuthUser = null)
        {
            _fakeRightsFilter = A.Fake<IRightsFilter>();
            A.CallTo(() => _fakeRightsFilter.CheckRights(A<ActionExecutingContext>.Ignored, A<IEnumerable<int>>.Ignored)).Returns(true);
            _applicationFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Debug");
                    builder.UseConfiguration(Config);
                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton(_fakeRightsFilter);
                        services.AddSingleton(fakeAuthUser ?? new FakeAuthUser());
                        services.AddFakeAuthentication();
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