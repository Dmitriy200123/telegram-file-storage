using System;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace TelegramAuth
{
    public class Program
    {
        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .Build();

        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main()
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            Console.WriteLine($"Run in {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")} mode");
            var bot = new AuthBot(Config);
            _quitEvent.WaitOne();
        }
    }
}