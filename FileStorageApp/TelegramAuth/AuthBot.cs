using System;
using System.Threading;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace TelegramAuth
{
    public class AuthBot
    {
        private readonly IInfoStorageFactory _infoStorageFactory;

        public AuthBot(IConfiguration config)
        {
            var botToken = config["BotToken"];
            var botClient = new TelegramBotClient(botToken);
            var receiverOptions = new ReceiverOptions();
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions);
            var dbConfig = new DataBaseConfig($"Server={config["DbHost"]};" +
                                              $"Username={config["DbUser"]};" +
                                              $"Database={config["DbName"]};" +
                                              $"Port={config["DbPort"]};" +
                                              $"Password={config["DbPassword"]};" +
                                              "SSLMode=Prefer");
            _infoStorageFactory = new InfoStorageFactory(dbConfig);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            var chatId = update.Message!.Chat.Id;
            var messageText = update.Message.Text;
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
            if (!CommandChecker.IsCommandValid(update, out var guid))
            {
                await botClient.SendTextMessageAsync(chatId, "Неверная команда, я могу только авторизировать вас",
                    cancellationToken: cancellationToken);
                return;
            }

            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var isAlreadyAdded = await usersStorage.HasTelegramId(update.Message.From.Id);
            if (isAlreadyAdded)
            {
                await botClient.SendTextMessageAsync(chatId,
                    "Ваш аккаунт уже привязан, если это были не вы обратитесь к системному администратору",
                    cancellationToken: cancellationToken);
                return;
            }

            var result = await usersStorage.AddTelegramIdToUser(guid, update.Message.From.Id);
            if (!result)
            {
                await botClient.SendTextMessageAsync(chatId, "В процессе авторизации что-то пошло не так",
                    cancellationToken: cancellationToken);
                return;
            }

            await botClient.SendTextMessageAsync(chatId, "Успешная авторизация",
                cancellationToken: cancellationToken);
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}