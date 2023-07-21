using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class ExpenseCostCenter: IEntityBase
    {
        public string EntityId { get => "CostCenterId"; set { } }

        public int CostCenterId { get; set; }
        public String Code { get; set; }
        public String Name { get; set; }
        public bool Active { get; set; }
    }
}
