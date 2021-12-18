using System;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace TelegramAuth
{
    public class Program
    {
        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json", true, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                true, true)
            .Build();

        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static void Main()
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            // kick off asynchronous stuff 

            Console.WriteLine("hui");
            Console.WriteLine(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            Console.WriteLine(Config["DbHost"]);
            var bot = new AuthBot(Config);
            _quitEvent.WaitOne();
        }
    }
}