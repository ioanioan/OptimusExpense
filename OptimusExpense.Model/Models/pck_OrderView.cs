using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public  class pck_OrderView
    {
        public String OrderNumber { get; set; }
        public String SectionCode { get; set; }
        public String TaskCode { get; set; }
        public double? TaskOrder { get; set; }
        public double? OrderStatus { get; set; }
    }
}
