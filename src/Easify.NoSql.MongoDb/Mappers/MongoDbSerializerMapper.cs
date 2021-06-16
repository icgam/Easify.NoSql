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
using MongoDB.Bson.Serialization;

namespace Easify.NoSql.MongoDb.Mappers
{
    public static class MongoDbSerializerMapper
    {
        private static readonly object SyncObject = new object();

        public static void TryRegisterSerializer<T>(IBsonSerializer<T> bsonSerializer)
        {
            if (bsonSerializer == null) throw new ArgumentNullException(nameof(bsonSerializer));
            lock (SyncObject)
            {
                BsonSerializer.RegisterSerializer(typeof(T), bsonSerializer);
            }
        }
        public static void TryRegisterSerializer<T>(IBsonSerializer bsonSerializer)
        {
            if (bsonSerializer == null) throw new ArgumentNullException(nameof(bsonSerializer));

            lock (SyncObject)
            {
                BsonSerializer.RegisterSerializer(typeof(T), bsonSerializer);
            }
        }
    }
}