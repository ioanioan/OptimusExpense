using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Infrastructure.Extensions;
using OptimusExpense.Infrastucture.Extensions;
using OptimusExpense.Infrastucture.Utils;
using OptimusExpense.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace OptimusExpense.Data.Abstract
{

  

    public abstract class EntityBaseRepositoryA<T,U> : IEntityBaseRepository<T>
              where T : class, IEntityBase, new()
             where U : DbContext
    {

        U entityContext;
        public EntityBaseRepositoryA(U _context)
        {
            this.entityContext = _context;
        }

        public U GetContext() 
        {
            return this.entityContext;
        }

        public void SetContext(DbContext context)
        {
            this.entityContext =(U) context;
        }

        public SqlBulkCopyOptions SqlBulkCopyOptions { get; set; }

        protected virtual T AddEntity(U entityContext, T entity)
        {
            var ent = GetDbSet(entityContext).Add(entity).Entity;
            entity.GetType().GetProperty(entity.EntityId.Split(',')[0]).SetValue(entity, ent.GetType().GetProperty(entity.EntityId.Split(',')[0]).GetValue(ent, null));
            return entity;
        }

        protected virtual T UpdateEntity(U entityContext, T entity)
        {
            // var id = entity.GetType().GetProperty(entity.EntityId).GetValue(entity, null);

            String[] keys = entity.EntityId.Split(',');
            Object[] values = new Object[keys.Length];
            for (var i = 0; i < keys.Length; i++)
            {
                values[i] = entity.GetType().GetProperty(keys[i]).GetValue(entity, null);
                if (values[i] == null && entity.GetType().GetProperty(keys[i]).PropertyType == typeof(string))
                {
                    values[i] = "";
                }
            }
            return GetEntity(entityContext, values);
        }

        protected virtual IQueryable<T> GetEntities(U entityContext)
        {
            return GetDbSet(entityContext);
        }

        protected virtual T GetEntity(U entityContext, Object[] values)
        {
            T t = new T();

            String[] keys = t.EntityId.Split(',');
            StringBuilder queryB = new StringBuilder();
            queryB.Append("( ");
            for (var i = 0; i < keys.Length; i++)
            {
                if (i > 0)
                {
                    queryB.Append(" && ");
                }
                queryB.Append(keys[i] + "==@" + i);
            }

            queryB.Append(" )");


            return GetDbSet(entityContext).Where(queryB.ToString(), values).FirstOrDefault();
        }

        protected virtual DbSet<T> GetDbSet(U entityContext)
        {
            return entityContext.Set<T>();
        }

        public virtual T Add(T entity)
        {
      
                T addedEntity = AddEntity(entityContext, entity);
                entityContext.SaveChanges();
                return addedEntity;
           
        }

        public virtual T Save(T entity)
        {
            var id = "" + entity.GetType().GetProperty(entity.EntityId.Split(',')[0]).GetValue(entity, null);
            if (id == "0")
            {
                return Add(entity);
            }
            else
            {
                return Update(entity);
            }
        }



        public virtual void Remove(T entity)
        {
           
            
                entityContext.Entry<T>(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
             
        }



        public virtual T Update(T entity)
        {
            T existingEntity = null;


            existingEntity = UpdateEntity(entityContext, entity);
            if (existingEntity != null)
            {
                SimpleMapper.PropertyMap(entity, existingEntity);

                entityContext.SaveChanges();
            }


            if (existingEntity == null)
            {
                existingEntity = Add(entity);
            }
            return existingEntity;
        }

        public virtual IQueryable<T> Get()
        {
            T t = new T();
            
            return GetEntities(entityContext);
        }



        public virtual void SaveBulk(IEnumerable<T> enumerable)
        {
            SaveBulk<T>(enumerable, null, null);
        }
        private static String GetColumnsTableString(String numeTabela)
        {
            return "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + numeTabela + "'";

        }

        class cCOLUMN_NAME
        {
            public String COLUMN_NAME { get; set; }
        }


        public virtual void SaveBulk(DataTable dt, String numeTabela)
        {
            U _context = entityContext;

            T tt = new T();
            if (numeTabela == null)
                numeTabela = typeof(T).Name;

            String[] orders = _context.Database.ExecuteSqlRawExt<cCOLUMN_NAME>(GetColumnsTableString(numeTabela)).Select(p => p.COLUMN_NAME).ToArray();
            var conn = _context.Database.GetDbConnection() as SqlConnection;
            var dat = DateTime.Now;
            if (conn.State == System.Data.ConnectionState.Closed)
                conn.Open();
            var d1 = DateTime.Now - dat;
            try
            {
                if (orders != null)
                {
                    for (int i = dt.Columns.Count - 1; i >= 0; i--)
                    {
                        if (!orders.Contains(dt.Columns[i].ColumnName))
                        {
                            dt.Columns.RemoveAt(i);
                        }
                    }
                }
                dt.SetColumnsOrder(orders);




                var d2 = DateTime.Now - dat;


                SaveBulk(dt, numeTabela, conn, this.SqlBulkCopyOptions);


            }
            finally
            {
                conn.Close();
            }

        }

        private void SaveBulk<E>(IEnumerable<E> enumerable, String key, String numeTabela) where E : class, new()
        {
            if (numeTabela == null)
                numeTabela = typeof(T).Name;

            var dt = enumerable.ToDataTable();


            SaveBulk(dt, numeTabela);


        }

        public virtual void SaveBulkEnumerableUpdate(IEnumerable<T> enumerable)
        {
            T t = new T();
            var dt = enumerable.ToDataTable();
            SaveBulkEnumerableUpdate(dt, typeof(T).Name, t.EntityId.Split(','));
        }


        public virtual void SaveBulkEnumerableUpdate(DataTable dt, string numeTabela, String[] key, ExecuteTransaction exec = null, SQlQueryMerge merge = null)
        {
            try
            {
                U _context = entityContext;

                T tt = Activator.CreateInstance<T>();
                if (numeTabela == null)
                    numeTabela = typeof(T).Name;
                string[] orders = _context.Database.ExecuteSqlRawExt<cCOLUMN_NAME>(GetColumnsTableString(numeTabela)).Select(p => p.COLUMN_NAME).ToArray();
                var conn = _context.Database.GetDbConnection() as SqlConnection;
                var dat = DateTime.Now;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                var d1 = DateTime.Now - dat;
                try
                {
                    if (merge != null && merge.Columns != null)
                    {
                        for (int i = dt.Columns.Count - 1; i >= 0; i--)
                        {
                            if (!merge.Columns.Contains(dt.Columns[i].ColumnName))
                            {
                                dt.Columns.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = dt.Columns.Count - 1; i >= 0; i--)
                        {
                            if (!orders.Contains(dt.Columns[i].ColumnName))
                            {
                                dt.Columns.RemoveAt(i);
                            }
                        }
                    }

                    var numeTemp = "#" + numeTabela;//"TestT";
                    dt.SetColumnsOrder(orders);
                    string query = "";

                    var com = conn.CreateCommand();
                    var listC = new List<String>();
                    foreach (DataColumn cc in dt.Columns)
                    {
                        listC.Add(cc.ColumnName);
                    }
                    query = "select top 0 " + (merge != null && merge.Columns != null ? merge.Columns.Select(p => p.ToString()).Aggregate((current, next) => current + "," + next) : String.Join(",", listC)) + "  into " + numeTemp + " from " + numeTabela + " WITH(NOLOCK) ";


                    com.CommandText = query;
                    com.ExecuteNonQuery();


                    var d2 = DateTime.Now - dat;

                    SaveBulk(dt, numeTemp, conn);

                    var columnJoinString = key.Select(p => "D." + p.ToString() + "=S." + p).Aggregate((current, next) => current + " AND " + next);

                    var stringUpdate = (merge != null && merge.Update != null ? merge.Update : listC.Select(p => "D." + p.ToString() + "=S." + p).Aggregate((current, next) => current + ", " + next));




                    query = " MERGE " + numeTabela + " AS D " +
                          "USING  " + numeTemp + " S " +
                          "ON " + columnJoinString +
                          " WHEN MATCHED  THEN " +
                          "UPDATE SET " + stringUpdate +

                      (merge != null && merge.Columns != null ? "" : (" WHEN NOT MATCHED BY TARGET THEN INSERT(" + listC.Select(p => p.ToString()).Aggregate((current, next) => current + "," + next) + ") " +
                          "VALUES(" + listC.Select(p => "S." + p.ToString()).Aggregate((current, next) => current + "," + next) + ") ")) +
                          (merge != null && merge.Delete != null ? " WHEN NOT MATCHED BY SOURCE THEN " + merge.Delete : "") +
                          " ; " +
                         "drop table " + numeTemp + " ";
                    com.CommandText = query;
                    com.ExecuteNonQuery();
                    //var d4 = DateTime.Now - dat;



                }
                finally
                {
                    conn.Close();
                }
                //  tx.Commit();
            }

            catch (Exception)
            {
                throw;
            }
        }

        private static void SaveBulk(DataTable dt, String numeTabela, SqlConnection con, SqlBulkCopyOptions op = SqlBulkCopyOptions.Default)
        {
            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(con, op, null))
            {
                bulkcopy.BulkCopyTimeout = 1200;
                bulkcopy.DestinationTableName = numeTabela;
                bulkcopy.BatchSize = dt.Rows.Count;
                bulkcopy.WriteToServer(dt);
                bulkcopy.Close();
            }
        }

        public virtual void DeleteBulk<E>(T entity) where E : class, new()
        {
         

                String[] keys = entity.EntityId.Split(',');
                String[] values = new String[keys.Length];
                String name = typeof(E).Name;


                StringBuilder strB = new StringBuilder();

                for (var i = 0; i < keys.Length; i++)
                {
                    values[i] = "" + entity.GetType().GetProperty(keys[i]).GetValue(entity, null);

                    if (i > 0)
                    {
                        strB.Append(" AND ");
                    }
                    strB.Append(keys[i] + "=" + values[i] + "");
                }



                entityContext.Database.ExecuteSqlRaw("delete from " + name + " where " + strB.ToString());
            

        }

        public virtual void DeleteBulk(String tabela, String[] keys, Object[] values)
        {


            StringBuilder strB = new StringBuilder();

            for (var i = 0; i < keys.Length; i++)
            {

                if (i > 0)
                {
                    strB.Append(" AND ");
                }
                strB.Append(keys[i] + "=" + values[i] + "");
            }

            entityContext.Database.ExecuteSqlRaw("delete from " + tabela + " where " + strB.ToString());


        }


        public virtual void Remove(int id)
        {
            Remove(new object[] { id });
        }


        public virtual void Remove(Object[] keys)
        {
        
                T entity = GetEntity(entityContext, keys);
                entityContext.Entry<T>(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
             
        }

        public virtual T Get(Object[] keys)
        {


            
                return GetEntity(entityContext, keys);
        }


        public virtual T Get(int id)
        {
            return Get(new object[] { id });
        }

        public virtual void SaveBulk<E>(IEnumerable<E> list, int[] values) where E : class, new()
        {
            T t = new T();
            var keys = t.EntityId.Split(',');
            for (var i = 0; i < keys.Length; i++)
            {
                PropertyInfo prop = typeof(T).GetProperty(keys[i]);
                prop.SetValue(t, values[i], null);
            }


            DeleteBulk<E>(t);

            SaveBulk<E>(list, t.EntityId, typeof(E).Name);
        }


        public virtual void SaveBulk<E>(IEnumerable<E> list, int id) where E : class, new()
        {


            SaveBulk(list, new int[] { id });


        }
    }
}
