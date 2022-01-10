using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace FileStorageAPI.Tests
{
    public class TestWebEnvironment : IDisposable
    {
        private TestServer Server { get; }
        private CookieContainer CookieContainer { get; }
        private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .Build();

        public TestWebEnvironment()
        {
            var builder = new WebHostBuilder()
                .UseConfiguration(Config)
                .UseEnvironment("Debug")
                .UseStartup<Startup>();

            Server = new TestServer(builder);
            CookieContainer = new CookieContainer();
            CookieContainer.Add(new Cookie("CookieName", "CookieValue")
            {
                CommentUri = null,
                Discard = false,
                Domain = "localhost",
                Expired = false,
                Expires = default,
                HttpOnly = false,
                Path = "/",
                Secure = false,
                Version = 0
            });
            CookieContainer.Add(new Cookie(".AspNetCore.Identity.Application", "CfDJ8MId4MCSygRBpSRQldlT5JuIyNpg6F1Bm5llN2km2trz0aGBatc2jKZhrcS4sCSE_3_sDTYWFvgsieOtdiaiDSTZKSv0Vn1mbVmSezZFE5jO5nE20LHbMhry5PMu1Wk00lyB8ubQE2jgW97ajPQhxsXUgaQg80K9lvc9ExUZmvvly1STDuB1s42vFtBx_KIkH_9l7imQDJKXSJDK0Ey1jXjEvcQFDTQLoC-fktnT8zR7ZDomS_rykIzO0vu1EWC_z88Pb7b-75YS7JmjhGDFEoJW1S8q2OhJz0-vRPTjGqUF6qdhaix7GJZblr6lUIFA7t1lSn3mYR-QeLXjBM-vzk3cMqKpNAeOiMei4NuL6MOoCf1gV-TZ2jX5nArb8IHVps25dovqKPW7LI20lS-wYZcbZY8Od3O9K6Fl5RmpaltjGYNYOS9KvXIJJb5u-pB-h20gij-4fKLJ8bc4oWg5Rg4ICG0NlYkRKC4zEubz8FfFsSYhBntYmdVMAeg-PLMyIyCfKXazUCAy1-qSpDOaZeSgWJ_koHrNVuOYTNTrVKWFasC-gL-nFr5OjuorjkbZW93LTyfpHz2IDllPR3Spxtlvxi7A3tQCWVuBeEcmdbma0qXkJMlfnCMhnfhdmlRFVRU8OVD-R_6iDBSqYDjzI3YOrF-8Pe3UFT2-TPfYfvx-sImUIdNs0aTv7l-4QGYDj2amlNSvXMb4EDzqYmdlRWZB5rDnMmUvLSOSRQ7NwcK_RGPvs-N9cLnEBqhddY7ipvoEJpDidCudSVr8pjeCkJrair_WyRSQi6qRDZ1ciGwiuCWAZH1WICcPDIEHiLAm4_6e0CbmZ8AIXJ_blryHMAlkvI1WvQfikzUyhFoCHvhTQkedWGLSwV-ON0wTcbxePZCvknZPEj3cWyoKEwdA6XaVkkKCuEVw_pl7bn_2T4Ns9ZNOOpFfGDvIBW1_fpQWWh0CMcCOrp9zvna1m4JAULC3Atk4THedwa5pQtufAuA-IU7Pd8jXYuYXjqffoet18vFf_63uXOXScVc5u2V8pnx0-xI10FamsSvNR7eIo53MW5dDx2szHvmP_1V5fKAsmkQD-t7PT0o_8i_cvUEQk8UtJ5I351WGrO-bmrqWdvgBVlrsTAH5Gl13FNy3HorrNQ", "/", "localhost"));
        }

        private RequestBuilder BuildRequest(string url)
        {
            var uri = new Uri(Server.BaseAddress, url);
            var builder = Server.CreateRequest(url);

            var cookieHeader = CookieContainer.GetCookieHeader(uri);
            if (!string.IsNullOrWhiteSpace(cookieHeader))
            {
                builder.AddHeader(HeaderNames.Cookie, cookieHeader);
            }

            return builder;
        }

        private void UpdateCookies(string url, HttpResponseMessage response)
        {
            if (response.Headers.Contains(HeaderNames.SetCookie))
            {
                var uri = new Uri(Server.BaseAddress, url);
                var cookies = response.Headers.GetValues(HeaderNames.SetCookie);
                foreach (var cookie in cookies)
                {
                    CookieContainer.SetCookies(uri, cookie);
                }
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var response = await BuildRequest(url).GetAsync();
            UpdateCookies(url, response);
            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            var builder = BuildRequest(url);
            builder.And(request => request.Content = content);
            var response = await builder.PostAsync();
            UpdateCookies(url, response);
            return response;
        }

        public void Dispose()
        {
            Server.Dispose();
        }
    }
}