using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Tests.AuthForTests
{
    public class FakeAuthHandler : AuthenticationHandler<FakeAuthOptions>
    {
        private readonly FakeAuthUser _fakeAuthUser;

        public FakeAuthHandler(IOptionsMonitor<FakeAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            FakeAuthUser fakeAuthUser) : base(options, logger, encoder, clock)
        {
            _fakeAuthUser = fakeAuthUser;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(_fakeAuthUser.Claims, FakeAuthConstants.Scheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, FakeAuthConstants.Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}