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
using System.Linq.Expressions;
using Easify.NoSql.ComponentModel;
using MongoDB.Driver;

namespace Easify.NoSql.MongoDb
{
    public class MongoDbSort<T> : ISortItBy<T>, IThenSortItBy<T>
    {
        public SortDefinition<T> SortDefinition { get; private set; }

        public IThenSortItBy<T> OrderBy(string field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            SortDefinition = Builders<T>.Sort.Ascending(field);

            return this;
        }

        public IThenSortItBy<T> OrderByDescending(string field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            SortDefinition = Builders<T>.Sort.Descending(field);

            return this;
        }

        public IThenSortItBy<T> OrderBy(Expression<Func<T, object>> property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            SortDefinition = Builders<T>.Sort.Ascending(property);

            return this;
        }

        public IThenSortItBy<T> OrderByDescending(Expression<Func<T, object>> property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            SortDefinition = Builders<T>.Sort.Descending(property);

            return this;
        }

        public IThenSortItBy<T> ThenBy(string field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            SortDefinition = SortDefinition.Ascending(field);

            return this;
        }

        public IThenSortItBy<T> ThenByDescending(string field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            SortDefinition = SortDefinition.Ascending(field);

            return this;
        }

        public IThenSortItBy<T> ThenBy(Expression<Func<T, object>> property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            SortDefinition = SortDefinition.Ascending(property);

            return this;
        }

        public IThenSortItBy<T> ThenByDescending(Expression<Func<T, object>> property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            SortDefinition = SortDefinition.Ascending(property);

            return this;
        }
    }
}