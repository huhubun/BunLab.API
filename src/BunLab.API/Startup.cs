using BunLab.API.Core.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BunLab.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });

            services
                .AddMvc(options =>
                {
                    // 移除不需要的 Formatter
                    // 让 AddSwaggerGen 生成 content type 时，不要有 text/plain 和 text/json
                    options.OutputFormatters.RemoveType<StringOutputFormatter>();

                    var jsonOutputFormatter = options.OutputFormatters.First(m => m.GetType() == typeof(JsonOutputFormatter));
                    (jsonOutputFormatter as JsonOutputFormatter).SupportedMediaTypes.Remove("text/json");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddApiVersioning();

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(options =>
            {
                // swagger doc 由 ConfigureSwaggerOptions 配置，所以这里不需要配置了

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.EnableAnnotations();
                options.OperationFilter<SwaggerDefaultValues>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app.UseMvc();

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "/docs/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "docs";
                options.DocumentTitle = "BunLab API";
                options.IndexStream = () =>
                {
                    // index.html 模板来自 https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/src/Swashbuckle.AspNetCore.SwaggerUI/index.html
                    return GetType().GetTypeInfo().Assembly.GetManifestResourceStream("BunLab.API.Content.SwaggerUI.index.html");
                };

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        // /docs/v1/swagger.json
                        $"/docs/{description.GroupName}/swagger.json",
                        // BunLab API v1
                        $"BunLab API {description.GroupName}");
                }
            });
        }
    }
}
