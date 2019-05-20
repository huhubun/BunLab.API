using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

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
            services
                .AddMvc(options =>
                {
                    // 让 AddSwaggerGen 生成 content type 时，不要有 text/plain 和 text/json
                    // 方案2：https://stackoverflow.com/questions/34990291/swashbuckle-swagger-how-to-annotate-content-types/35129474#35129474
                    options.OutputFormatters.RemoveType<StringOutputFormatter>();

                    var jsonOutputFormatter = options.OutputFormatters.First(m => m.GetType() == typeof(JsonOutputFormatter));
                    (jsonOutputFormatter as JsonOutputFormatter).SupportedMediaTypes.Remove("text/json");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "BunLab API",
                    Version = "v1",
                    Description = "Web API for <a href='https://lab.bun.dev'>BunLab</a>",
                    Contact = new Contact
                    {
                        Name = "bun",
                        Email = "bun@nzc.me",
                        Url = "https://bun.dev"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.EnableAnnotations();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "/docs/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/docs/v1/swagger.json", "BunLab API v1");
                options.RoutePrefix = "docs";
                options.DocumentTitle = "BunLab API";
                options.IndexStream = () =>
                {
                    // index.html 模板来自 https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/src/Swashbuckle.AspNetCore.SwaggerUI/index.html
                    return GetType().GetTypeInfo().Assembly.GetManifestResourceStream("BunLab.API.Content.SwaggerUI.index.html");
                };
            });
        }
    }
}
