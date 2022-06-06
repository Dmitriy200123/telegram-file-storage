using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using FileStorageApp.Data.InfoStorage.Storages.Rights;
using FileStorageApp.Data.InfoStorage.Storages.Users;
using JwtAuth;
using Microsoft.Extensions.Configuration;

namespace TokenGenerator
{
    public static class Program
    {
        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        public static async Task Main(string[] args)
        {
            var arguments = GetKeyFromArgs(args);
            var storage = new InfoStorageFactory(CreateDataBaseConfig());
            var refreshTokenGenerator = new RefreshTokenGenerator();
            var userStorage = storage.CreateUsersStorage();
            var rightsStorage = storage.CreateRightsStorage();
            var newUser = await CreateNewUserAsync(userStorage, rightsStorage, arguments);
            
            var claimName = new Claim(ClaimTypes.Name, newUser.ToString());
            var tokenGenerator = new JwtAuthenticationManager(Configuration["TokenKey"], refreshTokenGenerator, storage);
            var auth = tokenGenerator.Authenticate(newUser.ToString(), DateTime.Now.AddYears(10), claimName).GetAwaiter().GetResult();
            Console.WriteLine($"Token: {auth.JwtToken}");
        }

        private static string GetKeyFromArgs(string[] args)
        {
            if (args.Length != 2)
                throw new ArgumentException("Invalid arguments length");
            if(args[0] != "-u")
                throw new ArgumentException("No userName in arguments");
            return args[1];
        }
        
        private static DataBaseConfig CreateDataBaseConfig()
        {
            var connectionString = $"Server={Configuration["DbHost"]};" +
                                   $"Username={Configuration["DbUser"]};" +
                                   $"Database={Configuration["UsersDbName"]};" +
                                   $"Port={Configuration["DbPort"]};" +
                                   $"Password={Configuration["DbPassword"]};" +
                                   "SSLMode=Prefer";
            return new DataBaseConfig(connectionString);
        }

        private static async Task<Guid> CreateNewUserAsync(IUsersStorage usersStorage, IRightsStorage rightsStorage, string name)
        {
            var guid = Guid.NewGuid();
            var user = new User
            {
                Id = guid,
                TelegramId = -1,
                GitLabId = -1,
                RefreshToken = "",
                Avatar = "https://static.detmir.st/media_out/383/198/3198383/1500/0.jpg?1572577221104",
                Name = name
            };
            await usersStorage.AddAsync(user);
            await AddAccessOptionsToUser(guid, rightsStorage);
            return guid;
        }

        private static async Task AddAccessOptionsToUser(Guid userId, IRightsStorage rightsStorage)
        {
            var accesses = Enum
                .GetValues(typeof(Access))
                .Cast<int>()
                .Where(x => x != 0);
            foreach (var elem in accesses)
            {
                var rightModel = new Right
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Access = elem,
                };
                await rightsStorage.AddAsync(rightModel);
            }
        }
    }
}