using System;
using Microsoft.Extensions.Configuration;

namespace TelegramAuth
{
    public class Program
    {
        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        public static void Main()
        {
            var bot = new AuthBot(Config);
            Console.ReadKey();
        }
    }
}