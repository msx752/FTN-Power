using FTNPower.Core.ApplicationService;
using FTNPower.Data.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FTNPower.Static.Services
{
    public class EFRepository<T> : IEFRepository<T>, IDisposable where T : class
    {
        private BotContext _context;
        private DbSet<T> _dbset;
        private bool disposedValue;

        public EFRepository(BotContext context)
        {
            _context = context;
            _dbset = context.Set<T>();
        }

        public EntityEntry<T> Add(T entity)
        {
            return _dbset.Add(entity);
        }

        public void Delete(T entity)
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
        }

        public void Update(T entity)
        {
            var entry = _context.Entry(entity);
            _dbset.Attach(entity);
            entry.State = EntityState.Modified;
        }

        public T GetById(object id)
        {
            return _dbset.Find(id);
        }

        public IQueryable<T> All()
        {
            return _dbset.AsQueryable<T>();
        }
        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return All().Where(predicate).AsEnumerable<T>();
        }

        public T Search(params object[] keyValues)
        {
            return _dbset.Find(keyValues);
        }
        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. This method default no-tracking query.
        /// </summary>
        /// <param name="selector">The selector for projection.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. Default to <c>true</c>.</param>
        /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition specified by <paramref name="predicate"/>.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        public T Single(Expression<Func<T, T>> selector,
                                    Expression<Func<T, bool>> predicate = null,
                                    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                    Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                                    bool disableTracking = true)
        {
            IQueryable<T> query = _dbset.AsQueryable<T>();
            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return orderBy(query).Select(selector).FirstOrDefault();

            else
                return query.Select(selector).FirstOrDefault();
        }

        public void Add(params T[] entities)
        {
            _dbset.AddRange(entities);
        }

        public void Add(IEnumerable<T> entities)
        {
            _dbset.AddRange(entities);
        }


        public void Delete(params T[] entities)
        {
            foreach (var item in entities)
            {
                Delete(item);
            }
        }

        public void Delete(IEnumerable<T> entities)
        {
            foreach (var item in entities)
            {
                Delete(item);
            }
        }

        public void Update(params T[] entities)
        {
            foreach (var item in entities)
            {
                Update(item);
            }
        }

        public void Update(IEnumerable<T> entities)
        {
            foreach (var item in entities)
            {
                Update(item);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context?.Dispose();
                    _dbset = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
