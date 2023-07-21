using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class ExpenseNature: IEntityBase
    {
        public string EntityId { get => "ExpenseNatureId"; set { } }

        public int ExpenseNatureId { get; set; }
        public String Name { get; set; }
        public String ContContabil { get; set; }
        public int? VatId { get; set; }
        public bool Active { get; set; }
    }
}
