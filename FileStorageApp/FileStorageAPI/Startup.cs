using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using AspNet.Security.OAuth.GitLab.Tests.Data;
using FilesStorage;
using FilesStorage.Interfaces;
using FileStorageAPI.Converters;
using FileStorageAPI.Providers;
using FileStorageAPI.Services;
using FileStorageApp.Data.InfoStorage.Config;
using FileStorageApp.Data.InfoStorage.Factories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddControllers();
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
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("InMemoryDb"));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            /*services.ConfigureSwaggerGen(options =>
            {
                var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileStorageAPI.xml");
                options.IncludeXmlComments(xmlPath);
            });*/
            services.AddAuthentication().AddGitLab(options =>
            {
                // Provide the GitLab Client ID
                options.ClientId = Configuration["Authentication:GitLab:ClientId"];
                // Provide the GitLab Secret
                options.ClientSecret = Configuration["Authentication:GitLab:ClientSecret"];
                options.AuthorizationEndpoint = "https://git.66bit.ru/oauth/authorize";
                options.TokenEndpoint = "https://git.66bit.ru/oauth/token";
                options.UserInformationEndpoint = "https://git.66bit.ru/api/v4/user";
                options.SaveTokens = true;
                options.Events.OnCreatingTicket = ctx =>
                {
                    var tokens = ctx.Properties.GetTokens() as List<AuthenticationToken>;
                    tokens.Add(new AuthenticationToken()
                    {
                        Name = "TicketCreated",
                        Value = DateTime.UtcNow.ToString()
                    });
                    ctx.Properties.StoreTokens(tokens);
                    return Task.CompletedTask;
                };
            });
            services.AddSingleton(Configuration);
            services.AddCors();
            RegisterProviders(services);
            RegisterDtoConverters(services);
            RegisterFileStorage(services);
            RegisterInfoStorage(services);
            RegisterApiServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()||env.IsEnvironment("Docker"))
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
            //app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void RegisterDtoConverters(IServiceCollection services)
        {
            services.AddSingleton<IChatConverter, ChatConverter>();
            services.AddSingleton<ISenderConverter, SenderConverter>();
            services.AddSingleton<IFileInfoConverter, FileInfoConverter>();
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
    }
#pragma warning restore CS1591
}