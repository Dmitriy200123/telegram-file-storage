using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace TelegramAuth
{
    public class AuthBot
    {
        private readonly IInfoStorageFactory _infoStorageFactory;
        private const string Url = "https://git.66bit.ru/api/v4/user";
        private readonly HttpClient _httpClient;

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
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            var chatId = update.Message!.Chat.Id;
            var messageText = update.Message.Text;
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
            if (!CommandChecker.IsCommandValid(update, out var token) || token == null)
            {
                await botClient.SendTextMessageAsync(chatId, "Неверная команда, я могу только авторизировать вас",
                    cancellationToken: cancellationToken);
                return;
            }

            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var isAlreadyAdded = await usersStorage.HasTelegramIdAsync(update.Message.From!.Id);
            if (isAlreadyAdded)
            {
                await botClient.SendTextMessageAsync(chatId,
                    "Ваш аккаунт уже привязан, если это были не вы обратитесь к системному администратору",
                    cancellationToken: cancellationToken);
                return;
            }

            var telegramId = update.Message.From.Id;
            var gitLabUserId = GetGitlabIdByToken(token).Result;
            if (!gitLabUserId.HasValue)
            {
                await botClient.SendTextMessageAsync(chatId, "Неверный токен",
                    cancellationToken: cancellationToken);
                return;
            }


            var dataBaseResult = await usersStorage.AddTelegramIdToGitLabUserAsync(gitLabUserId!.Value, telegramId);
            if (!dataBaseResult)
                await botClient.SendTextMessageAsync(chatId, "Не смогли добавить в базу",
                    cancellationToken: cancellationToken);
            else
                await botClient.SendTextMessageAsync(chatId, "Успешно авторизован, пожалуйста, обновите страницу на сайте",
                    cancellationToken: cancellationToken);
        }

        private async Task<long?> GetGitlabIdByToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(Url);
            var responseString = await response.Content.ReadAsStringAsync();
            var gitLabUser = JsonConvert.DeserializeObject<GitLabUser>(responseString);
            return gitLabUser.Id;
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