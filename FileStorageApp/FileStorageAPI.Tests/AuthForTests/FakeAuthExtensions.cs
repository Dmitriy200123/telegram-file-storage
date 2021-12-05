using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorageAPI.Tests.AuthForTests
{
    public static class FakeAuthExtensions
    {
        public static AuthenticationBuilder AddFakeAuthentication(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(FakeAuthConstants.Scheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services.AddAuthentication(FakeAuthConstants.Scheme)
                .AddScheme<FakeAuthOptions, FakeAuthHandler>(
                    FakeAuthConstants.Scheme, _ => { });
        }
    }
}