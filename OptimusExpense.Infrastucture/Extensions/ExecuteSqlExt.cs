using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OptimusExpense.Infrastucture.Extensions
{
    public static class ExecuteSqlExt
    {
        /// <summary>
        /// Execute raw SQL query with query parameters
        /// </summary>
        /// <typeparam name="T">the return type</typeparam>
        /// <param name="db">the database context database, usually _context.Database</param>
        /// <param name="query">the query string</param>
        /// <param name="map">the map to map the result to the object of type T</param>
        /// <param name="queryParameters">the collection of query parameters, if any</param>
        /// <returns></returns>
        public static List<T> ExecuteSqlRawExt<T>(this DatabaseFacade db, string query, IEnumerable<Object> queryParameters = null) where T:  new ()
        {
            var lstColumns = typeof(T).GetProperties();
            using (var command = db.GetDbConnection().CreateCommand())
            {
                if ((queryParameters?.Any() ?? false))
                    command.Parameters.AddRange(queryParameters.ToArray());

                command.CommandText = query;
                command.CommandType = CommandType.Text;
                 
                db.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    var lst = new List<T>();

                    while (reader.Read())
                    {
                       // entities.Add(map(result));

                        var newObject = new T();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            PropertyInfo prop = lstColumns.FirstOrDefault(a => a.Name.ToLower().Equals(name.ToLower()));
                            if (prop == null)
                            {
                                continue;
                            }
                            var val = reader.IsDBNull(i) ? null : reader[i];
                            prop.SetValue(newObject, val, null);
                        }
                        lst.Add(newObject);
                    }

                    return lst;
                }
            }

        }

        public static DataSet ExecuteSqlDataTable(this DatabaseFacade db, string query, IEnumerable<Object> queryParameters = null) 
        {
            DataSet ds = new DataSet();
            using (var con = db.GetDbConnection())
            {
               
                using (var command = con.CreateCommand())
                {
                    if ((queryParameters?.Any() ?? false))
                        command.Parameters.AddRange(queryParameters.ToArray());

                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                     
                    db.OpenConnection();
                    using (DbDataAdapter a = DbProviderFactories.GetFactory(con).CreateDataAdapter())
                    {
                        a.SelectCommand = command;
                        
                        a.Fill(ds);
                       
                    }
                     
                }
            }
            return ds;

        }

    }
}
