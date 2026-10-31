using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api;
using GtMotive.Estimate.Microservice.Infrastructure;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MongoDb;
using Xunit;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure
{
    /// <summary>
    /// Raíz de composición de la aplicación sin el host: casos de uso, presenters y adaptadores de MongoDB reales.
    /// Por defecto arranca un contenedor de MongoDB, así que el único requisito es Docker. Para usar un
    /// MongoDB que ya exista, define la variable de entorno MongoDb__ConnectionString.
    /// </summary>
    public sealed class CompositionRootTestFixture : IDisposable, IAsyncLifetime
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly MongoDbContainer _mongoContainer;
        private readonly string _externalConnectionString;
        private readonly string _databaseName = "functional_" + Guid.NewGuid().ToString("N");

        public CompositionRootTestFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            _externalConnectionString = configuration["MongoDb:ConnectionString"];

            if (string.IsNullOrWhiteSpace(_externalConnectionString))
            {
                _mongoContainer = new MongoDbBuilder("mongo:8.0").Build();
            }

            var services = new ServiceCollection();
            Configuration = configuration;
            ConfigureServices(services);
            services.AddSingleton<IConfiguration>(configuration);
            _serviceProvider = services.BuildServiceProvider();
        }

        public IConfiguration Configuration { get; }

        public async Task InitializeAsync()
        {
            if (_mongoContainer is not null)
            {
                await _mongoContainer.StartAsync();
            }
        }

        public async Task DisposeAsync()
        {
            if (_mongoContainer is not null)
            {
                await _mongoContainer.DisposeAsync();
            }
        }

        public async Task UsingHandlerForRequest<TRequest>(Func<IRequestHandler<TRequest, Unit>, Task> handlerAction)
            where TRequest : IRequest<Unit>
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, Unit>>();

            await handlerAction.Invoke(handler);
        }

        public async Task UsingHandlerForRequestResponse<TRequest, TResponse>(Func<IRequestHandler<TRequest, TResponse>, Task> handlerAction)
            where TRequest : IRequest<TResponse>
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            if (handler == null)
            {
                Debug.Fail("The requested handler has not been registered");
            }

            await handlerAction.Invoke(handler);
        }

        public async Task UsingRepository<TRepository>(Func<TRepository, Task> handlerAction)
        {
            ArgumentNullException.ThrowIfNull(handlerAction);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<TRepository>();

            if (handler == null)
            {
                Debug.Fail("The requested handler has not been registered");
            }

            await handlerAction.Invoke(handler);
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddApiDependencies();
            services.AddLogging();

            // La cadena de conexión se lee de forma diferida, cuando se usa MongoDB por primera vez, que es después de arrancar el contenedor.
            services.Configure<MongoDbSettings>(settings =>
            {
                settings.ConnectionString = _mongoContainer?.GetConnectionString() ?? _externalConnectionString;
                settings.MongoDbDatabaseName = _databaseName;
            });

            services.AddBaseInfrastructure(true)
                .AddMongoDb();
        }
    }
}
