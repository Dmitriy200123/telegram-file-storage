using System;
using System.IO;
using DocumentsIndex.Config;
using DocumentsIndex.Factories;
using DocumentsIndex.Pipelines;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SearchDocumentsAPI.Services.DocumentsIndex;
using SearchDocumentsAPI.Services.DocumentsSearch;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace SearchDocumentsAPI
{
#pragma warning disable CS1591
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ConfigureSwagger(services);
            ConfigureDependencies(services);
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

        private static void ConfigureDependencies(IServiceCollection services)
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


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
#pragma warning restore CS1591