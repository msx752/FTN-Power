using FTNPower.Core.ApplicationService;
using FTNPower.Data.Migrations;
using FTNPower.Static.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FTNPower.Static.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public BotContext Context { get; private set; }
        private Dictionary<Type, object> _repositories;
        private bool _disposed;

        public UnitOfWork(BotContext context)
        {
            _repositories = new Dictionary<Type, object>(30);
            Context = context;
        }
        public int Commit()
        {
            return Context.SaveChanges();
        }

        public IEFRepository<TEntity> Db<TEntity>() where TEntity : class
        {
            if (_repositories.Keys.Contains(typeof(TEntity)))
                return _repositories[typeof(TEntity)] as IEFRepository<TEntity>;
            _repositories.Add(typeof(TEntity), new EFRepository<TEntity>(Context));
            return (IEFRepository<TEntity>)_repositories[typeof(TEntity)];
        }

        public int Query(string sql, params object[] parameters)
        {
            return Context.Database.ExecuteSqlRaw(sql, parameters);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                }
                this._disposed = true;
            }
        }
        public List<T> Query<T>(string rawSql, params SqlParameter[] parameters)
        {
            var conn = this.Context.Database.GetDbConnection();
            List<T> res = new List<T>();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = rawSql;
                command.Parameters.Clear();
                if (parameters != null)
                {
                    foreach (var item in parameters)
                    {
                        var p = command.CreateParameter();
                        p.ParameterName = item.ParameterName;
                        p.Value = item.Value;
                        p.DbType = item.DbType;
                        command.Parameters.Add(p);
                    }
                }
                var wasOpen = false;
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                    wasOpen = true;
                }
                using (var r = command.ExecuteReader())
                {
                    while (r.Read())
                    {
                        T t = Activator.CreateInstance<T>();
                        for (int inc = 0; inc < r.FieldCount; inc++)
                        {
                            Type type = t.GetType();
                            string
                                 pname = r.GetName(inc);
                            PropertyInfo
                                prop = type.GetProperty(pname);

                            prop.SetValue(t, r.GetValue(inc), null);
                        }
                        res.Add(t);
                    }
                }
                if (wasOpen)
                    conn.Close();
            }
            return res;
        }
    }
}
