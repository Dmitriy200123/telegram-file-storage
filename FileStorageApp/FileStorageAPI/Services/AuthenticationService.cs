using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using JwtAuth;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly ITokenRefresher _tokenRefresher;
        private readonly IConfiguration _configuration;
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// Конструктор сервиса.
        /// </summary>
        /// <param name="jwtAuthenticationManager">Менеджер токенов</param>
        /// <param name="tokenRefresher">Обновлятель токена</param>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="infoStorageFactory"></param>
        public AuthenticationService(IJwtAuthenticationManager jwtAuthenticationManager, ITokenRefresher tokenRefresher,
            IConfiguration configuration, IInfoStorageFactory infoStorageFactory)
        {
            _jwtAuthenticationManager = jwtAuthenticationManager ??
                                        throw new ArgumentNullException(nameof(jwtAuthenticationManager));
            _tokenRefresher = tokenRefresher ?? throw new ArgumentNullException(nameof(tokenRefresher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
        }

        /// <inheritdoc />
        public async Task<RequestResult<AuthenticationResponse>> LogIn(string token)
        {
            const string url = "https://git.66bit.ru/api/v4/user";
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = client.GetAsync(url).Result;
            var responseContent = await response.Content.ReadAsStringAsync();
            var gitLabUser = JsonConvert.DeserializeObject<GitLabUser>(responseContent);
            if (gitLabUser.Id == null)
                return RequestResult.BadRequest<AuthenticationResponse>("Invalid token");


            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            AuthenticationResponse jwtToken; 
            var user = await usersStorage.GetByGitLabIdAsync(gitLabUser.Id.Value);
            if (user is null)
            {
                var guid = Guid.NewGuid();
                jwtToken = _jwtAuthenticationManager.Authenticate(guid.ToString());
                user = new User
                {
                    Id = guid,
                    TelegramId = null,
                    GitLabId = gitLabUser.Id.Value,
                    Avatar = gitLabUser.AvatarUrl!,
                    RefreshToken = jwtToken.RefreshToken
                };
                await usersStorage.AddAsync(user);
            }
            else
            {
                jwtToken = _jwtAuthenticationManager.Authenticate(user.Id.ToString());
                await usersStorage.UpdateRefreshToken(user.Id, jwtToken.RefreshToken);
            }

            return RequestResult.Ok(jwtToken);
        }


        /// <inheritdoc />
        public async Task<RequestResult<AuthenticationResponse>> Refresh(RefreshCred refreshCred)
        {
            var token = await _tokenRefresher.Refresh(refreshCred);
            return token is null
                ? RequestResult.Unauthorized<AuthenticationResponse>("No such user")
                : RequestResult.Ok(token);
        }

        /// <inheritdoc />
        public async Task<RequestResult<string>> LogOut(Guid guid)
        {
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            var result = await usersStorage.RemoveRefreshToken(guid);
            return result 
                ? RequestResult.NoContent<string>() 
                : RequestResult.BadRequest<string>("No such user");
        }
    }
}