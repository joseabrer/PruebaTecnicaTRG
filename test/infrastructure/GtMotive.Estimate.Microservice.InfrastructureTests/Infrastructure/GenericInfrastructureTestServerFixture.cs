using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure
{
    public sealed class GenericInfrastructureTestServerFixture : IDisposable
    {
        public GenericInfrastructureTestServerFixture()
        {
            Server = new TestServer(CreateHostBuilder());
        }

        public TestServer Server { get; }

        /// <summary>
        /// Crea otro servidor con la misma configuración del host, donde los servicios indicados sustituyen a los reales.
        /// Así un test cambia los casos de uso por dobles de prueba, de modo que solo se ejercita el host.
        /// </summary>
        /// <param name="configureTestServices">Registros que sustituyen a los de la aplicación.</param>
        /// <returns>Un servidor nuevo. Quien llama debe liberarlo.</returns>
        public static TestServer CreateServer(Action<IServiceCollection> configureTestServices)
        {
            ArgumentNullException.ThrowIfNull(configureTestServices);

            return new TestServer(CreateHostBuilder().ConfigureTestServices(configureTestServices));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Server?.Dispose();
        }

        private static IWebHostBuilder CreateHostBuilder()
        {
            return new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseEnvironment("IntegrationTest")
                .UseDefaultServiceProvider(options => { options.ValidateScopes = true; })
                .ConfigureAppConfiguration((context, builder) => { builder.AddEnvironmentVariables(); })
                .UseStartup<Startup>();
        }
    }
}
