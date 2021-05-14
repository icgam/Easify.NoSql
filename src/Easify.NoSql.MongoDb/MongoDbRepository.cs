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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Easify.Extensions.Specifications;
using Easify.NoSql.ComponentModel;
using MongoDB.Driver;

namespace Easify.NoSql.MongoDb
{
    public class MongoDbRepository<T> : IMongoDbRepository<T>
    {
        public MongoDbRepository(IMongoDatabase mongoDatabase, string mongoCollectionName)
        {
            var name = mongoCollectionName ?? throw new ArgumentNullException(nameof(mongoCollectionName));
            MongoDatabase = mongoDatabase ?? throw new ArgumentNullException(nameof(mongoDatabase));
            MongoCollection = mongoDatabase.GetCollection<T>(name);
        }

        protected IMongoCollection<T> MongoCollection { get; }

        protected IMongoDatabase MongoDatabase { get; }

        public virtual Task InsertAsync(T t)
        {
            return MongoCollection.InsertOneAsync(t);
        }

        public virtual Task InsertManyAsync(T[] ts)
        {
            return MongoCollection.InsertManyAsync(ts);
        }

        public virtual Task<T> GetAsync(Expression<Func<T, bool>> condition)
        {
            return InternalGetAsync(condition, m => m, CancellationToken.None);
        }

        public virtual Task<T> GetAsync(Func<IFindFluent<T, T>, IFindFluent<T, T>> query)
        {
            return InternalGetAsync(m => true, query, CancellationToken.None);
        }

        public virtual Task<T> GetAsync(Specification<T> specification,
            Func<IFindFluent<T, T>, IFindFluent<T, T>> query)
        {
            return InternalGetAsync(specification?.ToExpression(), query, CancellationToken.None);
        }

        public virtual Task<T> GetAsync(Specification<T> specification)
        {
            return InternalGetAsync(specification?.ToExpression(), m => m, CancellationToken.None);
        }

        public virtual Task<T> GetAsync(Expression<Func<T, bool>> condition,
            Func<IFindFluent<T, T>, IFindFluent<T, T>> query)
        {
            return InternalGetAsync(condition, query, CancellationToken.None);
        }

        public virtual Task<T> GetAsync(Expression<Func<T, bool>> condition,
            Func<IFindFluent<T, T>, IFindFluent<T, T>> query,
            CancellationToken cancellationToken)
        {
            return InternalGetAsync(condition, query, cancellationToken);
        }

        [Obsolete("Use function with IFindFluent interface")]
        public virtual Task<T> GetAsync(Expression<Func<T, bool>> condition, Action<ISort<T>> sortAction,
            CancellationToken cancellationToken)
        {
            var sort = new MongoDbSort<T>();
            sortAction(sort);

            return InternalGetAsync(condition, ff => ff.Sort(sort.SortDefinition), cancellationToken);
        }

        [Obsolete("Use function with IFindFluent interface")]
        public virtual Task<T> GetAsync(Expression<Func<T, bool>> condition, Action<ISort<T>> sortAction)
        {
            var sort = new MongoDbSort<T>();

            sortAction?.Invoke(sort);

            return InternalGetAsync(condition, ff => ff.Sort(sort.SortDefinition), CancellationToken.None);
        }

        public virtual Task<List<T>> GetListAsync()
        {
            return InternalGetListAsync(m => true, null, CancellationToken.None);
        }

        public virtual Task<List<T>> GetListAsync(Func<IFindFluent<T, T>, IFindFluent<T, T>> query)
        {
            return InternalGetListAsync(m => true, query, CancellationToken.None);
        }

        public virtual Task<List<T>> GetListAsync(Func<IFindFluent<T, T>, IFindFluent<T, T>> query,
            CancellationToken cancellationToken)
        {
            return InternalGetListAsync(m => true, query, cancellationToken);
        }

        public virtual Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition,
            Func<IFindFluent<T, T>, IFindFluent<T, T>> query, CancellationToken cancellationToken)
        {
            return InternalGetListAsync(condition, query, cancellationToken);
        }

        public virtual Task<List<T>> GetListAsync(Specification<T> specification)
        {
            return InternalGetListAsync(specification?.ToExpression(), null, CancellationToken.None);
        }

        public virtual Task<List<T>> GetListAsync(Specification<T> specification, CancellationToken cancellationToken)
        {
            return InternalGetListAsync(specification?.ToExpression(), null, cancellationToken);
        }

        public virtual Task<List<T>> GetListAsync(Specification<T> specification,
            Func<IFindFluent<T, T>, IFindFluent<T, T>> query, CancellationToken cancellationToken)
        {
            return InternalGetListAsync(specification?.ToExpression(), query, cancellationToken);
        }

        public virtual Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition)
        {
            return InternalGetListAsync(condition, null, CancellationToken.None);
        }

        [Obsolete("Use function with IFindFluent interface")]
        public virtual Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition, Action<ISort<T>> sortAction)
        {
            return GetListAsync(condition, sortAction, CancellationToken.None);
        }

        [Obsolete("Use function with IFindFluent interface")]
        public virtual Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition, Action<ISort<T>> sortAction,
            CancellationToken cancellationToken)
        {
            var sort = new MongoDbSort<T>();

            sortAction?.Invoke(sort);

            return InternalGetListAsync(condition, ff => ff.Sort(sort.SortDefinition), cancellationToken);
        }

        public virtual Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken)
        {
            return InternalGetListAsync(condition, null, cancellationToken);
        }

        public virtual async Task<T> UpdateAsync(Expression<Func<T, bool>> condition, T t, bool isUpsert = true)
        {
            return await InternalUpdateAsync(condition, t, isUpsert, CancellationToken.None);
        }

        public virtual async Task DeleteOneAsync(Expression<Func<T, bool>> condition)
        {
            await InternalDeleteOneAsync(condition, CancellationToken.None);
        }

        public virtual async Task DeleteManyAsync(Expression<Func<T, bool>> condition)
        {
            await InternalDeleteManyAsync(condition, CancellationToken.None);
        }

        public virtual Task<long> GetCountAsync(Expression<Func<T, bool>> condition = null)
        {
            return InternalGetCountAsync(condition);
        }

        public virtual async Task<T> UpdateAsync(Expression<Func<T, bool>> condition, T t, bool isUpsert,
            CancellationToken cancellationToken)
        {
            return await InternalUpdateAsync(condition, t, isUpsert, cancellationToken);
        }

        public virtual async Task<T> UpdateAsync(Specification<T> specification, T t, bool isUpsert,
            CancellationToken cancellationToken)
        {
            return await InternalUpdateAsync(specification?.ToExpression(), t, isUpsert, cancellationToken);
        }

        public virtual async Task<T> UpdateAsync(Specification<T> specification, T t, bool isUpsert = true)
        {
            return await InternalUpdateAsync(specification?.ToExpression(), t, isUpsert, CancellationToken.None);
        }

        public virtual async Task DeleteOneAsync(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken)
        {
            await InternalDeleteOneAsync(condition, cancellationToken);
        }

        public virtual async Task DeleteOneAsync(Specification<T> specification, CancellationToken cancellationToken)
        {
            await InternalDeleteOneAsync(specification?.ToExpression(), cancellationToken);
        }

        public virtual async Task DeleteOneAsync(Specification<T> specification)
        {
            await InternalDeleteOneAsync(specification?.ToExpression(), CancellationToken.None);
        }

        public virtual async Task DeleteManyAsync(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken)
        {
            await InternalDeleteManyAsync(condition, cancellationToken);
        }

        public virtual async Task DeleteManyAsync(Specification<T> specification, CancellationToken cancellationToken)
        {
            await InternalDeleteManyAsync(specification?.ToExpression(), cancellationToken);
        }

        public virtual async Task DeleteManyAsync(Specification<T> specification)
        {
            await InternalDeleteManyAsync(specification?.ToExpression(), CancellationToken.None);
        }

        public virtual Task<long> GetCountAsync(Specification<T> specification)
        {
            return InternalGetCountAsync(specification?.ToExpression());
        }

        private Task<T> InternalGetAsync(Expression<Func<T, bool>> condition,
            Func<IFindFluent<T, T>, IFindFluent<T, T>> extend, CancellationToken cancellationToken)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            var find = MongoCollection.Find(condition);

            if (extend != null)
                find = extend(find);

            return find.FirstOrDefaultAsync(cancellationToken);
        }

        private Task<List<T>> InternalGetListAsync(Expression<Func<T, bool>> condition,
            Func<IFindFluent<T, T>, IFindFluent<T, T>> extend, CancellationToken cancellationToken)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            var find = MongoCollection.Find(condition);

            if (extend != null)
                find = extend(find);

            return find.ToListAsync(cancellationToken);
        }

        private Task<T> InternalUpdateAsync(Expression<Func<T, bool>> condition, T t, bool isUpsert,
            CancellationToken cancellationToken)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (t == null) throw new ArgumentNullException(nameof(t));

            return MongoCollection.FindOneAndReplaceAsync(condition, t,
                new FindOneAndReplaceOptions<T, T> { IsUpsert = isUpsert }, cancellationToken);
        }

        private async Task InternalDeleteOneAsync(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            await MongoCollection.DeleteOneAsync(condition, cancellationToken);
        }

        private async Task InternalDeleteManyAsync(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));

            await MongoCollection.DeleteManyAsync(condition, cancellationToken);
        }

        private Task<long> InternalGetCountAsync(Expression<Func<T, bool>> condition)
        {
            if (condition == null)
                condition = T => true;

            return MongoCollection.Find(condition).CountDocumentsAsync();
        }
    }
}