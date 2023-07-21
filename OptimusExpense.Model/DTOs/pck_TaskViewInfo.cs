using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class pck_TaskViewInfo
    {
        public pck_TaskView pck_TaskView { get; set; }
        public pck_OrderView pck_OrderView { get; set; }

        public int Status { get; set; }
    }
}
