using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class pck_OrderLog:IEntityBase
    {
        public string EntityId { get => "LogId"; set { } }
        public int LogId { get; set; }
        public DateTime InternalTime { get; set; }
        public String UserId { get; set; }
        public String OrderNumber { get; set; }
        public String TaskCode { get; set; }
        public String TaskName { get; set; }
        public int CarriageNumber { get; set; }
    }
}
