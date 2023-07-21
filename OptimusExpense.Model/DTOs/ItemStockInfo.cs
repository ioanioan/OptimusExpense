using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class ItemStockInfo
    {
        public  int Qtty { get; set; }

        public String ItemName { get; set; }

        public String Pharmacy { get; set; }

        public String StockType { get; set; }

        public String Stree { get; set; }

        public int? StreeNumber { get; set; }

        public String District { get; set; }

        public String City { get; set; }

        public decimal Price { get; set; } 

        public String Phone { get; set; }
    }


    public class ItemStockFilter
    {
        public String ItemName { get; set; }

        public String Address { get; set; }

        public String District { get; set; }

        public String City { get; set; }
    }
}
