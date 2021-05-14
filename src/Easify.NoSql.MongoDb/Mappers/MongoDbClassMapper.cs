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


using MongoDB.Bson.Serialization;

namespace Easify.NoSql.MongoDb.Mappers
{
    public static class MongoDbClassMapper
    {
        private static readonly object SyncObject = new object();

        public static void RegisterClass<T>(bool ignoreExtraProperties = true)
        {
            lock (SyncObject)
            {
                if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
                {
                    BsonClassMap.RegisterClassMap<T>(m =>
                    {
                        m.AutoMap();
                        m.SetIgnoreExtraElements(ignoreExtraProperties);
                    });
                }
            }
        }
    }
}