using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class pck_OrderViewInfo
    {
        public string OrderNumber { get; set; }

        public List<pck_CartViewInfo> ListCarts { get; set; }

        public List<pck_TaskViewInfo> ListTasks { get; set; }
    }
}
