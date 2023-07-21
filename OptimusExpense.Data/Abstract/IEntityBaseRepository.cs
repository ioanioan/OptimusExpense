using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OptimusExpense.Model;
namespace OptimusExpense.Data.Abstract
{
    public interface IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        T Add(T entity);
        void SetContext(DbContext context);

        void Remove(T entity);
        void SaveBulkEnumerableUpdate(DataTable dt, string numeTabela, String[] key, ExecuteTransaction exec = null, SQlQueryMerge merge = null);

        void Remove(int id);

        void Remove(Object[] keys);

        T Update(T entity);
        T Save(T entity);

        void SaveBulk(IEnumerable<T> list);

        IQueryable<T> Get();

        T Get(int id);

        T Get(Object[] keys);
        void SaveBulk<E>(IEnumerable<E> list, int id) where E : class, new();

        void SaveBulk<E>(IEnumerable<E> list, int[] keys) where E : class, new();

        void SaveBulkEnumerableUpdate(IEnumerable<T> enumerable);

        void DeleteBulk<E>(T entity) where E : class, new();

        void DeleteBulk(String tabela, String[] keys, Object[] values);

        void SaveBulk(DataTable dt, String numeTabela);

        SqlBulkCopyOptions SqlBulkCopyOptions
        {
            get; set;
        }
    }


    public class SQlQueryMerge
    {
        public String Update { get; set; }
        public String Delete { get; set; }

        public String[] Columns { get; set; }
    }

    public delegate void ExecuteTransaction(DbConnection con);
}
