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

namespace Easify.NoSql
{
    public interface IDocumentRepository<T>
    {
        Task InsertAsync(T t);
        Task InsertManyAsync(T[] ts);

        Task<T> GetAsync(Expression<Func<T, bool>> condition);
        Task<T> GetAsync(Specification<T> specification);

        Task<T> GetAsync(Expression<Func<T, bool>> condition, Action<ISortItBy<T>> sortAction,
            CancellationToken cancellationToken);

        Task<T> GetAsync(Expression<Func<T, bool>> condition, Action<ISortItBy<T>> sortAction);

        Task<List<T>> GetListAsync();
        Task<List<T>> GetListAsync(Specification<T> specification);
        Task<List<T>> GetListAsync(Specification<T> specification, CancellationToken cancellationToken);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition, Action<ISortItBy<T>> sortAction);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition, Action<ISortItBy<T>> sortAction,
            CancellationToken cancellationToken);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken);

        Task<T> UpdateAsync(Expression<Func<T, bool>> condition, T t, bool isUpsert = true);
        Task<T> UpdateAsync(Expression<Func<T, bool>> condition, T t, bool isUpsert,
            CancellationToken cancellationToken);
        Task<T> UpdateAsync(Specification<T> specification, T t, bool isUpsert,
            CancellationToken cancellationToken);
        Task<T> UpdateAsync(Specification<T> specification, T t, bool isUpsert = true);

        Task<long> GetCountAsync(Expression<Func<T, bool>> condition = null);
        Task<long> GetCountAsync(Specification<T> specification);

        Task DeleteOneAsync(Expression<Func<T, bool>> condition);
        Task DeleteOneAsync(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken);
        Task DeleteOneAsync(Specification<T> specification, CancellationToken cancellationToken);
        Task DeleteOneAsync(Specification<T> specification);

        Task DeleteManyAsync(Expression<Func<T, bool>> condition);
        Task DeleteManyAsync(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken);
        Task DeleteManyAsync(Specification<T> specification, CancellationToken cancellationToken);
        Task DeleteManyAsync(Specification<T> specification);
    }
}