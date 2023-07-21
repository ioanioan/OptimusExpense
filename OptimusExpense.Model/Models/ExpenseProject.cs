using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class ExpenseProject: IEntityBase
    {
        public string EntityId { get => "ExpenseProjectId"; set { } }

        public int ExpenseProjectId { get; set; }
        public String Name { get; set; }

        public DateTime? ValabilityStartDate { get; set; }

        public DateTime? ValabilityEndDate { get; set; }

        public bool? Active { get; set; }
    }
}
