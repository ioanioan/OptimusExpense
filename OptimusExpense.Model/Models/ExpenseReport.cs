using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class ExpenseReport : IEntityBase
    {
        public string EntityId { get => "ExpenseReportId"; set { } }

        public int ExpenseReportId { get; set; }
        public String Description { get; set; }
    }
}
