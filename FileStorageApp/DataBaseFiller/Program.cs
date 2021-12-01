using System;
using Amazon.S3;
using FilesStorage;
using FilesStorage.Interfaces;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.Extensions.Configuration;

namespace DataBaseFiller
{
    public static class Program
    {
        private static readonly HelpInfoProvider HelpInfoProvider = new();
        private static readonly ActionProvider ActionProvider = new();
        private static InfoStorageFactory _infoStorageFactory;
        private static IFilesStorageFactory _filesStorageFactory;

        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();


        public static void Main()
        {
            CreateConfig();
            Console.WriteLine("Should clean up data base? If yes enter any symbol or press enter to skip");
            var cleanUp = Console.ReadLine();
            if(cleanUp!.Length != 0)
                DataBaseCleaner.CleanUp(_infoStorageFactory, _filesStorageFactory).ConfigureAwait(true);
            var helpString = HelpInfoProvider.GetHelpInfo();
            Console.WriteLine(helpString);
            while (true)
            {
                var command = Console.ReadLine();
                var action = ActionProvider.ChooseAction(command!);
                if (action != null)
                    action.DoAction(_infoStorageFactory);
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
            _infoStorageFactory = new InfoStorageFactory(config);
            
            var s3Config = new AmazonS3Config
            {
                ServiceURL = Config["S3serviceUrl"],
                ForcePathStyle = true
            };
            var amazonConfig = new S3FilesStorageOptions(Config["S3accessKey"], Config["S3secretKey"],
                Config["S3bucketName"], s3Config, S3CannedACL.PublicReadWrite,
                TimeSpan.FromHours(1));
            _filesStorageFactory = new S3FilesStorageFactory(amazonConfig);
        }
    }
}