using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BunLab.API.Core.Swagger
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                  description.GroupName,
                    new Info()
                    {
                        Title = $"BunLab API v{description.ApiVersion.MajorVersion}",
                        Version = $"v{description.ApiVersion.MajorVersion}",
                        Description = "Web API for <a href='https://lab.bun.dev'>BunLab</a>",
                        Contact = new Contact
                        {
                            Name = "bun",
                            Email = "bun@nzc.me",
                            Url = "https://bun.dev"
                        }
                    });
            }
        }
    }
}
