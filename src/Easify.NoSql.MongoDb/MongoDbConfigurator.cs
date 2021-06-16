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
using Easify.NoSql.MongoDb.Mappers;
using Easify.NoSql.MongoDb.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Easify.NoSql.MongoDb
{
    public class MongoDbConfigurator : IDbConfigurator
    {
        private readonly MongoDbConfiguratorOptions _configuratorOptions;
        private static readonly object SyncObject = new object();

        public MongoDbConfigurator(MongoDbConfiguratorOptions configuratorOptions)
        {
            _configuratorOptions = configuratorOptions ?? throw new ArgumentNullException(nameof(configuratorOptions));
            
            Configure();
        }

        public void Configure()
        {
            lock (SyncObject)
            {
                ConfigureSerializers();
                ConfigureConventions();
            }
        }

        private static void ConfigureSerializers()
        {
            MongoDbSerializerMapper.TryRegisterSerializer(new SqlCompatibleDateTimeSerializer());
            MongoDbSerializerMapper.TryRegisterSerializer<ObjectId>(new StringSerializer(BsonType.ObjectId));
        }
        
        private void ConfigureConventions()
        {
            var conventionPack = new ConventionPack();
            
            if (_configuratorOptions.RepresentEnumAsString)
                conventionPack.Add(new EnumRepresentationConvention(BsonType.String));
            
            if (_configuratorOptions.IgnoreExtraElements)
                conventionPack.Add(new IgnoreExtraElementsConvention(true));
            
            // TODO: It should be configurable from outside 
            ConventionRegistry.Register("CustomConvention", conventionPack, t =>  true);
        }
    }
}