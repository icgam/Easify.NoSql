// This software is part of the Easify.Ef Library
// Copyright (C) 2018 Intermediate Capital Group
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 


using System;
using Easify.Http;
using Easify.NoSql.MongoDb;
using Easify.NoSql.MongoDb.ApplicationInsights;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Easify.NoSql.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbSupportWithTelemetry(this IServiceCollection services,
            IConfiguration configuration,
            Action<MongoDbConfiguratorOptions> configure = null)
        {
            return services.AddMongoDb(configuration, m =>
            {
                m.EnableTelemetry = true;
                configure?.Invoke(m);
            });
        }

        public static IServiceCollection AddMongoDbSupport(this IServiceCollection services,
            IConfiguration configuration,
            Action<MongoDbConfiguratorOptions> configure = null)
        {
            return services.AddMongoDb(configuration, configure);
        }

        private static IServiceCollection AddMongoDb(this IServiceCollection services, 
            IConfiguration configuration,
            Action<MongoDbConfiguratorOptions> configure = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<MongoDbOptions>(configuration.GetSection(nameof(MongoDbOptions)));
            
            var configuratorOptions = new MongoDbConfiguratorOptions();
            configure?.Invoke(configuratorOptions);
            
            services.TryAddSingleton<IDbConfigurator>(new MongoDbConfigurator(configuratorOptions));
            services.TryAddSingleton(sp =>
            {
                var optionAccessor = sp.GetRequiredService<IOptions<MongoDbOptions>>();
                var logger = sp.GetRequiredService<ILogger<MongoDbRepositoryFactory>>();

                if (!configuratorOptions.EnableTelemetry)
                    return new MongoDbRepositoryFactory(optionAccessor, logger);

                var requestContext = sp.GetRequiredService<IRequestContext>();
                var telemetryClient = sp.GetRequiredService<TelemetryClient>();
                return new MongoDbRepositoryFactory(optionAccessor, logger,
                    (s, db) => s.AttachTelemetry(telemetryClient, new TelemetryContext(db, requestContext.CorrelationId)));
            });

            services.TryAddSingleton<IDocumentRepositoryFactory>(m => m.GetRequiredService<MongoDbRepositoryFactory>());
            services.TryAddSingleton<IMongoDbRepositoryFactory>(m => m.GetRequiredService<MongoDbRepositoryFactory>());

            return services;
        }

        public static IServiceCollection AddDocumentRepository<T>(this IServiceCollection services, string collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            services.TryAddTransient(m =>
            {
                var factory = m.GetRequiredService<IDocumentRepositoryFactory>();
                var options = m.GetRequiredService<IOptions<MongoDbOptions>>();

                return factory.GetRepository<T>(options.Value.PrimaryDatabaseName, collection);
            });

            return services;
        }

        public static IServiceCollection AddMongoDbRepository<T>(this IServiceCollection services, string collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            services.TryAddTransient(m =>
            {
                var factory = m.GetRequiredService<IMongoDbRepositoryFactory>();
                var options = m.GetRequiredService<IOptions<MongoDbOptions>>();

                return factory.GetMongoDbRepository<T>(options.Value.PrimaryDatabaseName, collection);
            });

            return services;
        }

        public static IServiceCollection AddDocumentRepository<T>(this IServiceCollection services, string databaseName,
            string collection)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (databaseName == null) throw new ArgumentNullException(nameof(databaseName));
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            services.TryAddTransient(m =>
            {
                var factory = m.GetRequiredService<IDocumentRepositoryFactory>();
                return factory.GetRepository<T>(databaseName, collection);
            });

            return services;
        }

        public static IServiceCollection AddMongoDbRepository<T>(this IServiceCollection services, string databaseName,
            string collection)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (databaseName == null) throw new ArgumentNullException(nameof(databaseName));
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            services.TryAddTransient(m =>
            {
                var factory = m.GetRequiredService<IMongoDbRepositoryFactory>();
                return factory.GetMongoDbRepository<T>(databaseName, collection);
            });

            return services;
        }

        public static IServiceCollection AddDocumentRepository<T>(this IServiceCollection services)
        {
            services.TryAddTransient(m =>
            {
                var factory = m.GetRequiredService<IDocumentRepositoryFactory>();
                var options = m.GetRequiredService<IOptions<MongoDbOptions>>();

                return factory.GetRepository<T>(options.Value.PrimaryDatabaseName);
            });

            return services;
        }

        public static IServiceCollection AddMongoDbRepository<T>(this IServiceCollection services)
        {
            services.TryAddTransient(m =>
            {
                var factory = m.GetRequiredService<IMongoDbRepositoryFactory>();
                var options = m.GetRequiredService<IOptions<MongoDbOptions>>();

                return factory.GetMongoDbRepository<T>(options.Value.PrimaryDatabaseName);
            });

            return services;
        }
    }
}