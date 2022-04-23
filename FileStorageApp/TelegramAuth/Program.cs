using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace TelegramAuth
{
    public class Program
    {
        private static readonly string? Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        private static IConfigurationRoot Config;
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main()
        {
            
            var mainCatalog = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
            var builder = new ConfigurationBuilder()
                .SetBasePath($"{mainCatalog}/Config")
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Env}.json", true);
            Config = builder.Build();
            
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            Console.WriteLine($"Run in {Env} mode");
            var bot = new AuthBot(Config);
            _quitEvent.WaitOne();
        }
    }
}