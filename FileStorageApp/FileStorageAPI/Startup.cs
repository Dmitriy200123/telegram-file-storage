using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using FilesStorage;
using FilesStorage.Interfaces;
using FileStorageAPI.Converters;
using FileStorageAPI.Providers;
using FileStorageAPI.RightsFilters;
using FileStorageAPI.Services;
using FileStorageApp.Data.InfoStorage.Config;
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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using IAuthenticationService = FileStorageAPI.Services.IAuthenticationService;
using AuthenticationService = FileStorageAPI.Services.AuthenticationService;

namespace FileStorageAPI
{
#pragma warning disable CS1591
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var tokenKey = Configuration["TokenKey"];
            var key = Encoding.ASCII.GetBytes(tokenKey);
            Settings.SetUpSettings(Configuration, key, CreateDataBaseConfig());
            services.AddControllers();

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
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            services.AddSingleton(Configuration);
            services.ConfigureApplicationCookie(options =>
                {
                    options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden);
                    options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized);
                }
            );
            services.AddCors();

            services.AddSingleton<IRightsFilter, RightsFilter>();
            RegisterProviders(services);
            RegisterAuth(services, tokenKey, key);
            RegisterDtoConverters(services);
            RegisterFileStorage(services);
            RegisterInfoStorage(services);
            RegisterApiServices(services);
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "FileStorageAPI", Version = "v1"});
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
                var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileStorageAPI.xml");
                options.IncludeXmlComments(xmlPath);
            });
        }

        private DataBaseConfig CreateDataBaseConfig()
        {
            var connectionString = $"Server={Configuration["DbHost"]};" +
                                   $"Username={Configuration["DbUser"]};" +
                                   $"Database={Configuration["UsersDbName"]};" +
                                   $"Port={Configuration["DbPort"]};" +
                                   $"Password={Configuration["DbPassword"]};" +
                                   "SSLMode=Prefer";
            return new DataBaseConfig(connectionString);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Docker"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileStorageAPI v1"));
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void RegisterDtoConverters(IServiceCollection services)
        {
            services.AddSingleton<IChatConverter, ChatConverter>();
            services.AddSingleton<ISenderConverter, SenderConverter>();
            services.AddSingleton<IFileInfoConverter, FileInfoConverter>();
            services.AddSingleton<IUserConverter, UserConverter>();
        }

        private static void RegisterInfoStorage(IServiceCollection services)
        {
            services.AddSingleton<IDataBaseConfig>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var connectionString = $"Server={config["DbHost"]};" +
                                       $"Username={config["DbUser"]};" +
                                       $"Database={config["DbName"]};" +
                                       $"Port={config["DbPort"]};" +
                                       $"Password={config["DbPassword"]};" +
                                       "SSLMode=Prefer";
                return new DataBaseConfig(connectionString);
            });
            services.AddSingleton<IInfoStorageFactory, InfoStorageFactory>();
        }

        private static void RegisterFileStorage(IServiceCollection services)
        {
            services.AddSingleton<IS3FilesStorageOptions>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var configS3 = new AmazonS3Config {ServiceURL = config["S3serviceUrl"], ForcePathStyle = true};
                return new S3FilesStorageOptions(config["S3accessKey"], config["S3secretKey"],
                    config["S3bucketName"], configS3, S3CannedACL.PublicReadWrite,
                    TimeSpan.FromHours(1));
            });
            services.AddSingleton<IFilesStorageFactory, S3FilesStorageFactory>();
        }

        private static void RegisterApiServices(IServiceCollection services)
        {
            services.TryAddScoped<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IChatService, ChatService>();
            services.AddSingleton<ISenderService, SenderService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IUserInfoService, UserInfoService>();
            services.AddSingleton<IRightsService, RightsService>();
        }

        private static void RegisterProviders(IServiceCollection services)
        {
            services.AddSingleton<IDownloadLinkProvider, DownloadLinkProvider>();
            services.AddSingleton<IFileTypeProvider, FileTypeProvider>();
            services.AddSingleton<IExpressionFileFilterProvider, ExpressionFileFilterProvider>();
            services.AddSingleton<ISenderFormTokenProvider, SenderFormTokenProvider>();
        }

        private static void RegisterAuth(IServiceCollection serviceCollection, string tokenKey, byte[] key)
        {
            serviceCollection.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            serviceCollection.AddSingleton<ITokenRefresher>(x =>
                new TokenRefresher(key, x.GetService<IJwtAuthenticationManager>()!,
                    x.GetService<IInfoStorageFactory>()!));
            serviceCollection.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
            serviceCollection.AddSingleton<IJwtAuthenticationManager>(x =>
                new JwtAuthenticationManager(tokenKey, x.GetService<IRefreshTokenGenerator>()!,
                    x.GetService<IInfoStorageFactory>()!));
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
#pragma warning restore CS1591
}