
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTNPower.Core.ApplicationService
{
    public interface IUnitOfWork
    {
        IEFRepository<TEntity> Db<TEntity>() where TEntity : class;

        void Dispose();
        int Commit();
        int Query(string sql, params object[] parameters);
        List<T> Query<T>(string rawSql, params SqlParameter[] parameters);
    }
}
