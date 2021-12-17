using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageApp.Data.InfoStorage.Factories;
using FileStorageApp.Data.InfoStorage.Models;
using JwtAuth;
using Newtonsoft.Json;

namespace FileStorageAPI.Services
{
    /// <inheritdoc />
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;
        private readonly ITokenRefresher _tokenRefresher;
        private readonly IInfoStorageFactory _infoStorageFactory;

        /// <summary>
        /// Конструктор сервиса.
        /// </summary>
        /// <param name="jwtAuthenticationManager">Менеджер токенов</param>
        /// <param name="tokenRefresher">Обновлятель токена</param>
        /// <param name="infoStorageFactory">Фабрика для работы с базой данных</param>
        public AuthenticationService(IJwtAuthenticationManager jwtAuthenticationManager, ITokenRefresher tokenRefresher, IInfoStorageFactory infoStorageFactory)
        {
            _jwtAuthenticationManager = jwtAuthenticationManager ??
                                        throw new ArgumentNullException(nameof(jwtAuthenticationManager));
            _tokenRefresher = tokenRefresher ?? throw new ArgumentNullException(nameof(tokenRefresher));
            _infoStorageFactory = infoStorageFactory ?? throw new ArgumentNullException(nameof(infoStorageFactory));
        }

        /// <inheritdoc />
        public async Task<RequestResult<AuthenticationResponse>> LogIn(string token)
        {
            const string url = "https://git.66bit.ru/api/v4/user";
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            var gitLabUser = JsonConvert.DeserializeObject<GitLabUser>(responseContent);
            if (gitLabUser?.Id == null)
                return RequestResult.BadRequest<AuthenticationResponse>("Invalid token");
            var jwtToken = await CreateToken(gitLabUser);

            return RequestResult.Ok(jwtToken);
        }

        private async Task<AuthenticationResponse> CreateToken(GitLabUser gitLabUser)
        {
            using var usersStorage = _infoStorageFactory.CreateUsersStorage();
            
            var user = await usersStorage.GetByGitLabIdAsync(gitLabUser.Id);
            if (user is null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    TelegramId = null,
                    GitLabId = gitLabUser.Id,
                    Avatar = gitLabUser.AvatarUrl,
                    RefreshToken = "",
                    Name = gitLabUser.Name
                };
                await usersStorage.AddAsync(user);
                using var rightsStorage = _infoStorageFactory.CreateRightsStorage();
                var right = new Right
                {
                    Id = user.Id,
                    AccessType = Accesses.Default
                };
                await rightsStorage.AddAsync(right);
            }

            var userName = user.Id.ToString();
            var claimName = new Claim(ClaimTypes.Name, userName);
            var userRights = user.Rights;
            var accessJson = JsonConvert.SerializeObject(userRights.Select(x => x.Access).ToList());
            var claimAccess = new Claim(ClaimTypes.Role, accessJson);
            var claims = new[] {claimName, claimAccess};
            var jwtToken = await _jwtAuthenticationManager.Authenticate(userName, claims);

            return jwtToken;
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
            var result = await usersStorage.RemoveRefreshTokenAsync(guid);
            return result 
                ? RequestResult.NoContent<string>() 
                : RequestResult.BadRequest<string>("No such user");
        }
    }
}