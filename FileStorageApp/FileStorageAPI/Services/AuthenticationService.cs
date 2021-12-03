using System;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FileStorageAPI.Services
{
    /// <summary>
    /// Сервис отвечающий за аутентификацию
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIntToGuidConverter _intToGuidConverter;
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signInManager">Менеджер входа</param>
        /// <param name="userManager">Менеджер пользователей</param>
        /// <param name="intToGuidConverter">Конвертер, который превращает int в Guid</param>
        /// <param name="infoStorageFactory">Фабрика для работы с базой данных</param>
        public AuthenticationService(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, IIntToGuidConverter intToGuidConverter,
            IInfoStorageFactory infoStorageFactory)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _intToGuidConverter = intToGuidConverter;
            _infoStorageFactory = infoStorageFactory;
        }

        /// <summary>
        /// Метод, который отвечает за аутентификацию пользователя и добавление его в менеджера пользователей
        /// </summary>
        /// <param name="remoteError">Возможная ошибка от GitLab</param>
        /// <returns></returns>
        public async Task<RequestResult<AuthenticationProperties>> LogIn(string? remoteError)
        {
            if (remoteError != null)
                return RequestResult.BadRequest<AuthenticationProperties>("Произошла ошибка у GitLab");
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RequestResult.BadRequest<AuthenticationProperties>("Почему-то пользователь пустой");
            var providerKey = _intToGuidConverter.Convert(int.Parse(info.ProviderKey)).ToString();
            await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, providerKey, true);
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, providerKey);
            if (user == null)
            {
                using var sendersStorage = _infoStorageFactory.CreateFileSenderStorage();
                var sender = await sendersStorage.GetByIdAsync(Guid.Parse(providerKey));
                user = new ApplicationUser(providerKey, sender?.TelegramId);
                var userInfo = new UserLoginInfo(info.LoginProvider, providerKey, info.ProviderDisplayName);
                await _userManager.CreateAsync(user);
                await _userManager.AddLoginAsync(user, userInfo);
            }

            var props = new AuthenticationProperties();
            props.StoreTokens(info.AuthenticationTokens);

            await _signInManager.SignInAsync(user, props, info.LoginProvider);
            return RequestResult.Ok(props);
        }
    }
}