using System.Threading.Tasks;
using FileStorageAPI.Providers;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RightServices;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// Контроллер отвечающий за работу с телеграмом 
    /// </summary>
    [ApiController]
    [Route("auth/telegram")]
    [SwaggerTag("Авторизация с помощью Telegram")]
    public class TelegramController : ControllerBase
    {
        private readonly IUserIdFromTokenProvider _userIdFromTokenProvider;
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="TelegramController"/>.
        /// </summary>
        /// <param name="userIdFromTokenProvider">Id пользователя из токена</param>
        /// <param name="infoStorageFactory">Фабрика для работы с базой данных</param>
        public TelegramController(IUserIdFromTokenProvider userIdFromTokenProvider, IInfoStorageFactory infoStorageFactory)
        {
            _userIdFromTokenProvider = userIdFromTokenProvider;
            _infoStorageFactory = infoStorageFactory;
        }
        /// <summary>
        /// Удаление telegramId у пользователя
        /// </summary>
        [Route("logout")]
        [HttpPost]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Такого пользователя нет в базе")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Успешно удалили telegramId")]
        public async Task<IActionResult> LogOut()
        {
            var id = _userIdFromTokenProvider.GetUserIdFromToken(Request, Settings.Key);
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var result = await usersStorage.RemoveTelegramIdAsync(id);
            return result ? 
                NoContent() : 
                BadRequest("No such user in database");
        }
    }
}