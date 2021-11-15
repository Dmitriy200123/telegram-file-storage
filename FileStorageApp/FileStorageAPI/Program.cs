using System;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FileStorageAPI
{
#pragma warning disable CS1591
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var service = (IInfoStorageFactory)host.Services.GetService(typeof(IInfoStorageFactory))!;
            var storage = service.CreateFileSenderStorage();
            if (!storage.ContainsAsync(Guid.Parse("00000000-0000-0000-0000-000000000003")).Result)
            {//todo временный костыль, пока не научусь добавлять отправителей при заходе через телегу
                var res = storage.AddAsync(new FileSender
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    TelegramId = 1,
                    TelegramUserName = "temp value",
                    FullName = "Пользователь загрузивший файл с сайта",
                }).Result;
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
#pragma warning restore CS1591
}