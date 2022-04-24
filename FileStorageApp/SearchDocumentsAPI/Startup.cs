using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DocumentsIndex.Config;
using DocumentsIndex.Factories;
using DocumentsIndex.Pipelines;
using FileStorageApp.Data.InfoStorage.Factories;
using JwtAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SearchDocumentsAPI.Services.DocumentsIndex;
using SearchDocumentsAPI.Services.DocumentsSearch;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace SearchDocumentsAPI
{
#pragma warning disable CS1591
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            var mainCatalog = Directory.GetParent(env.ContentRootPath)?.FullName;
            var builder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath($"{mainCatalog}/Config")
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            var tokenKey = Configuration["TokenKey"];
            var key = Encoding.ASCII.GetBytes(tokenKey);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = false,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        RequireExpirationTime = true,
                    };
                });
            services.ConfigureApplicationCookie(options =>
                {
                    options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden);
                    options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized);
                }
            );
            services.AddCors();

            ConfigureSwagger(services);
            ConfigureDependencies(services);
            
            ServicePointManager.ServerCertificateValidationCallback +=
                (_, _, _, _) => true; //Там че-то сложно с сертами на localhost пришел тыкнуть это
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "SearchDocumentsAPI", Version = "v1"});
                c.AddEnumsWithValuesFixFilters(services, o =>
                {
                    o.ApplySchemaFilter = true;
                    o.XEnumNamesAlias = "x-enum-varnames";
                    o.XEnumDescriptionsAlias = "x-enum-descriptions";
                    o.ApplyParameterFilter = true;
                    o.ApplyDocumentFilter = true;
                    o.IncludeDescriptions = true;
                    o.IncludeXEnumRemarks = true;
                    o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;
                });
            });
            services.ConfigureSwaggerGen(options =>
            {
                var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SearchDocumentsAPI.xml");
                options.IncludeXmlComments(xmlPath);
            });
        }

        private void ConfigureDependencies(IServiceCollection services)
        {
            services.AddSingleton<IPipelineCreator, PipelineCreator>();
            services.AddSingleton<IDocumentIndexFactory, DocumentIndexFactory>();
            services.AddSingleton<IDocumentsIndexService, DocumentsIndexService>();
            services.AddSingleton<IDocumentsSearchService, DocumentsSearchService>();
            // TODO: в конфиг
            services.AddSingleton<IElasticConfig>(_ => new ElasticConfig("http://localhost:9200", "testindex"));
            services.AddSingleton(provider =>
            {
                var factory = provider.GetRequiredService<IDocumentIndexFactory>();
                return factory.CreateDocumentIndexStorage();
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Docker"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SearchDocumentsAPI v1"));
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
        
        static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode) =>
            context =>
            {
                if (statusCode != HttpStatusCode.Forbidden && statusCode != HttpStatusCode.Unauthorized)
                    return Task.CompletedTask;
                context.Response.Clear();
                context.Response.StatusCode = (int) statusCode;
                return Task.CompletedTask;
            };
    }
}
#pragma warning restore CS1591