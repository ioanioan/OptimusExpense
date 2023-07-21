using OptimusExpense.Data.Abstract;
using OptimusExpense.Infrastucture.Extensions;
using OptimusExpense.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
   public  class ItemRepository
    {


        //public OptimusExpenseContext c;

        //public ItemRepository(OptimusExpenseContext c)
        //{
        //   this. c = c;
        //}
        //public List<ItemStockInfo> SearchItemStock(ItemStockFilter filter)
        //{

            
        //    var list = new Microsoft.Data.SqlClient.SqlParameter[] {
        //       new Microsoft.Data.SqlClient.SqlParameter("@itemName", filter.ItemName),
        //        new Microsoft.Data.SqlClient.SqlParameter("@districtName", filter.District),
        //          new Microsoft.Data.SqlClient.SqlParameter("@cityName", filter.City),
        //           new Microsoft.Data.SqlClient.SqlParameter("@adress", filter.Address)

        //    };

        //    var result = c.Database.ExecuteSqlRawExt<ItemStockInfo>("exec spSearchItem @itemName,@districtName,@cityName,@adress ", list).ToList();
        //    return result;
        //}
    }
}
