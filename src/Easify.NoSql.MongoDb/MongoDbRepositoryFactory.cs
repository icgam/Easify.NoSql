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
using Easify.NoSql.MongoDb.Exceptions;
using Easify.NoSql.MongoDb.Mappers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Easify.NoSql.MongoDb
{
    public class MongoDbRepositoryFactory : IDocumentRepositoryFactory, IMongoDbRepositoryFactory
    {
        private readonly ILogger<MongoDbRepositoryFactory> _logger;
        private readonly Action<MongoClientSettings, string> _extendSettings;
        private readonly MongoDbOptions _options;

        public MongoDbRepositoryFactory(
            IOptions<MongoDbOptions> optionAccessor, 
            ILogger<MongoDbRepositoryFactory> logger,
            Action<MongoClientSettings, string> extendSettings = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _extendSettings = extendSettings;

            if (optionAccessor == null) throw new ArgumentNullException(nameof(optionAccessor));
            _options = optionAccessor.Value;
        }

        public IDocumentRepository<T> GetRepository<T>(string database)
        {
            return InternalGetRepository<T>(database, typeof(T).Name);
        }

        public IDocumentRepository<T> GetRepository<T>(string database, string collectionName)
        {
            return InternalGetRepository<T>(database, collectionName);
        }
        
        public IMongoDbRepository<T> GetMongoDbRepository<T>(string database)
        {
            return InternalGetRepository<T>(database, typeof(T).Name);
        }

        public IMongoDbRepository<T> GetMongoDbRepository<T>(string database, string collectionName)
        {
            return InternalGetRepository<T>(database, collectionName);
        }        
        
        private MongoDbRepository<T> InternalGetRepository<T>(string database, string collectionName)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));
            
            if (string.IsNullOrWhiteSpace(_options.ConnectionUrl))
                throw new InvalidConnectionInfoException();

            try
            {
                var clientSettings = CreateClientSettings(database);
                var mongoClient = new MongoClient(clientSettings);
                var mongoDatabase = mongoClient.GetDatabase(database);

                MongoDbClassMapper.RegisterClass<T>(_options.IgnoreExtraPropertiesFromCollection);

                return new MongoDbRepository<T>(mongoDatabase, collectionName);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error in connecting to mongodb {database} with {_options.ConnectionUrl}", e);
                throw new DatabaseConnectionFailedException($"Error in connecting to mongodb {database}", e);
            }
        }



        private MongoClientSettings CreateClientSettings(string database)
        {
            var mongoClientSettings = CreateClientSettingsWithDefaults(database);

            _extendSettings?.Invoke(mongoClientSettings, database);

            return mongoClientSettings;
        }

        private MongoClientSettings CreateClientSettingsWithDefaults(string database)
        {
            var mongoClientSettings = MongoClientSettings
                .FromUrl(new MongoUrl(_options.ConnectionUrl))
                .WithDefaultClusterConfiguration();
            
            if (_options.IsSslEnabled)
                mongoClientSettings.SslSettings = new SslSettings { EnabledSslProtocols = _options.SslProtocol };

            _extendSettings?.Invoke(mongoClientSettings, database);

            return mongoClientSettings;
        }
    }
}