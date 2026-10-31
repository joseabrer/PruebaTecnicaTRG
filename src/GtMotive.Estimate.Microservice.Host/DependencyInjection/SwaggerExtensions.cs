using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GtMotive.Estimate.Microservice.Host.Configuration;
using GtMotive.Estimate.Microservice.Host.Infrastructure.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace GtMotive.Estimate.Microservice.Host.DependencyInjection
{
    internal static class SwaggerExtensions
    {
        private static string AssemblyName => Assembly.GetEntryAssembly().GetName().Name;

        private static string AssemblyVersion => Assembly.GetEntryAssembly().GetName().Version.ToString();

        public static IServiceCollection AddSwagger(
            this IServiceCollection services,
            AppSettings settings,
            IConfiguration configuration)
        {
            // Registra el generador de Swagger, definiendo uno o más documentos Swagger
            services.AddSwaggerGen(
                options =>
                {
                    options.CustomSchemaIds(NombreDelEsquema);
                    options.SwaggerDoc($"v{AssemblyVersion}", new OpenApiInfo
                    {
                        Title = $"{AssemblyName} API",
                        Version = $"v{AssemblyVersion}",
                    });

                    if (configuration.GetValue<string>("Swagger:EnableTryIt") == "Yes")
                    {
                        // Define el esquema OAuth2.0 en uso (por ejemplo, Implicit Flow)
                        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Name = "oauth2",
                            Flows = configuration.GetValue<string>("Swagger:AuthFlow") == "AuthorizationCode"
                                ? new OpenApiOAuthFlows
                                {
                                    AuthorizationCode = new OpenApiOAuthFlow
                                    {
                                        AuthorizationUrl = new Uri($"{settings.JwtAuthority}/connect/authorize"),
                                        Scopes = new Dictionary<string, string>
                                        {
                                            ["estimate-public-scope"] = "estimate-api"
                                        },
                                        TokenUrl = new Uri($"{settings.JwtAuthority}/connect/token")
                                    }
                                }
                                : new OpenApiOAuthFlows()
                                {
                                    ClientCredentials = new OpenApiOAuthFlow()
                                    {
                                        Scopes = new Dictionary<string, string>
                                        {
                                            ["estimate-public-scope"] = "estimate-api"
                                        },
                                        TokenUrl = new Uri($"{settings.JwtAuthority}/connect/token")
                                    }
                                }
                        });

                        options.OperationFilter<IdentityServerApiSecurityOperationFilter>();
                    }
                });

            return services;
        }

        public static IApplicationBuilder UseSwaggerInApplication(
            this IApplicationBuilder app,
            PathBase pathBase,
            IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(pathBase);

            // Habilita el middleware que sirve el Swagger generado como un endpoint JSON.
            app.UseSwagger(options =>
            {
                if (!pathBase.IsDefault)
                {
                    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
                    options.RouteTemplate = "swagger/{documentName}/swagger.json";
                    options.PreSerializeFilters.Add((document, request) =>
                    {
                        document.Servers =
                        [
                            new OpenApiServer
                            {
                                Url = $"{request.Scheme}://{request.Host.Value}{pathBase.CurrentWithoutTrailingSlash}"
                            }

                        ];
                    });
                }
            });

            // Habilita el middleware que sirve swagger-ui (HTML, JS, CSS, etc.), indicando el endpoint JSON de Swagger.
            var url = pathBase.IsDefault
                ? $"/swagger/v{AssemblyVersion}/swagger.json"
                : $"{pathBase.CurrentWithoutTrailingSlash}/swagger/v{AssemblyVersion}/swagger.json";

            app.UseSwaggerUI(
                options =>
                {
                    options.SwaggerEndpoint(url, $"{AssemblyName} API V{AssemblyVersion}");

                    if (configuration.GetValue<string>("Swagger:EnableTryIt") == "No")
                    {
                        options.SupportedSubmitMethods();
                    }

                    options.OAuthClientId("client-gtestimate-swagger");
                    options.OAuthClientSecret("gtmotive");
                    options.OAuthScopeSeparator(" ");
                });

            return app;
        }

        private static string NombreDelEsquema(Type tipo)
        {
            if (!tipo.IsGenericType)
            {
                return tipo.Name;
            }

            // RespuestaApi<List<VehiculoRespuesta>> se publica como RespuestaApiDeListDeVehiculoRespuesta.
            var nombre = tipo.Name[..tipo.Name.IndexOf('`', StringComparison.Ordinal)];
            var argumentos = tipo.GetGenericArguments().Select(NombreDelEsquema);

            return $"{nombre}De{string.Join("Y", argumentos)}";
        }
    }
}
