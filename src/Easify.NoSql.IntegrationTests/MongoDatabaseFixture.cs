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
using System.Collections.Generic;
using Easify.NoSql.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;

namespace Easify.NoSql.IntegrationTests
{
    public class MongoDatabaseFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }
        public MongoDbRunner MongoRunner { get; private set; }

        public MongoDatabaseFixture()
        {
            MongoRunner = MongoDbRunner.Start();
            
            var mongoConfiguration = new Dictionary<string, string>
            {
                {"MongoDbOptions:ConnectionUrl", MongoRunner.ConnectionString},
            };
            
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddInMemoryCollection(mongoConfiguration).Build();

            var services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.AddMongoDbSupport(configuration);

            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            MongoRunner?.Dispose();
            MongoRunner = null;
        }
    }
}