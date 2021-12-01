using System;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.Extensions.Configuration;

namespace DataBaseFiller
{
    public static class Program
    {
        private static readonly HelpInfoProvider HelpInfoProvider = new();
        private static readonly ActionProvider ActionProvider = new();
        private static InfoStorageFactory infoStorageFactory;

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();


        public static void Main()
        {
            CreateConfig();
            Console.WriteLine("Should clean up data base? If yes enter any symbol or press enter to skip");
            var cleanUp = Console.ReadLine();
            if(cleanUp!.Length != 0)
                DataBaseCleaner.CleanUp(infoStorageFactory).ConfigureAwait(true);
            var helpString = HelpInfoProvider.GetHelpInfo();
            Console.WriteLine(helpString);
            while (true)
            {
                var command = Console.ReadLine();
                var action = ActionProvider.ChooseAction(command!);
                if (action != null)
                    action.DoAction(infoStorageFactory);
                else
                    Console.WriteLine("Unknown command");
            }
        }

        private static void CreateConfig()
        {
            var config = new DataBaseConfig($"Server={Config["DbHost"]};" +
                                            $"Username={Config["DbUser"]};" +
                                            $"Database={Config["DbName"]};" +
                                            $"Port={Config["DbPort"]};" +
                                            $"Password={Config["DbPassword"]};" +
                                            "SSLMode=Prefer");
            infoStorageFactory = new InfoStorageFactory(config);
        }
    }
}