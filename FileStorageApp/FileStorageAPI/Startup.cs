using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using FilesStorage;
using FilesStorage.Interfaces;
using FileStorageAPI.Converters;
using FileStorageAPI.Data;
using FileStorageAPI.Models;
using FileStorageAPI.Providers;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
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
            services.AddControllers();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("UserDataBase"));

            //подумать, а нужно ли это нам? 
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

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
                })
                .AddGitLab(options =>
                {
                    options.ClientId = Configuration["Authentication:GitLab:ClientId"];
                    options.ClientSecret = Configuration["Authentication:GitLab:ClientSecret"];
                    options.AuthorizationEndpoint = Configuration["Authentication:GitLab:AuthorizationEndpoint"];
                    options.TokenEndpoint = Configuration["Authentication:GitLab:TokenEndpoint"];
                    options.UserInformationEndpoint = Configuration["Authentication:GitLab:UserInformationEndpoint"];
                    options.SaveTokens = true;
                    options.AccessDeniedPath = "/auth/gitlab/unauthorized";
                    options.Events.OnCreatingTicket = ctx =>
                    {
                        var tokens = ctx.Properties.GetTokens() as List<AuthenticationToken>;
                        tokens!.Add(new AuthenticationToken()
                        {
                            Name = "TicketCreated",
                            Value = DateTime.UtcNow.ToString()
                        });
                        ctx.Properties.StoreTokens(tokens);
                        return Task.CompletedTask;
                    };
                });

            services.AddSingleton(Configuration);
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });
            services.ConfigureApplicationCookie(options =>
                {
                    options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden);
                    options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized);
                }
            );
            services.AddCors();
            RegisterProviders(services);
            RegisterAuth(services, tokenKey, key);
            RegisterDtoConverters(services);
            RegisterFileStorage(services);
            RegisterInfoStorage(services);
            RegisterApiServices(services);
            RegisterConverters(services);
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
        }

        private static void RegisterConverters(IServiceCollection services)
        {
            services.AddSingleton<IIntToGuidConverter, IntToGuidConverter>();
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

        private string CreateConnectionString()
        {
            return $"Server={Configuration["DbHost"]};" +
                   $"Username={Configuration["DbUser"]};" +
                   $"Database={Configuration["UsersDbName"]};" +
                   $"Port={Configuration["DbPort"]};" +
                   $"Password={Configuration["DbPassword"]};" +
                   "SSLMode=Prefer";
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
        }

        private static void RegisterProviders(IServiceCollection services)
        {
            services.AddSingleton<IDownloadLinkProvider, DownloadLinkProvider>();
            services.AddSingleton<IFileTypeProvider, FileTypeProvider>();
            services.AddSingleton<IExpressionFileFilterProvider, ExpressionFileFilterProvider>();

        }

        private static void RegisterAuth(IServiceCollection serviceCollection, string tokenKey, byte[] key)
        {
            serviceCollection.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            serviceCollection.AddSingleton<ITokenRefresher>(x => 
                new TokenRefresher(key, x.GetService<IJwtAuthenticationManager>()!));
            serviceCollection.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
            serviceCollection.AddSingleton<IJwtAuthenticationManager>(x => 
                new JwtAuthenticationManager(tokenKey, x.GetService<IRefreshTokenGenerator>()!));
            
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