using System.Security.Claims;
using System.Threading.Tasks;
using FileStorageAPI.Converters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FileStorageAPI.Controllers
{
    /// <summary>
    /// Авторизация через GitLab
    /// </summary>
    [ApiController]
    [Route("auth/gitlab")]
    [SwaggerTag("Авторизация с помощью GitLab")]
    public class AuthenticationController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult ExternalLogin()
        {
            const string provider = "GitLab";
            const string returnUrl = "auth/gitlab/confirm";
            string redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Authentication", new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }
        [Route("confirm")]
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? remoteError)
        {
            if (remoteError != null)
                return Content("Произошла ошибка у GitLab");
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return Content("Почему-то пользователь пустой");
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                var props = new AuthenticationProperties();
                props.StoreTokens(info.AuthenticationTokens);

                await _signInManager.SignInAsync(user, props, info.LoginProvider);

                return LocalRedirect("");
            }
            var id = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return Content($"Успешно авторизован, id пользователя: {id}");
        }
    }
}