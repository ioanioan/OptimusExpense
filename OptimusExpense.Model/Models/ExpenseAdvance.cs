using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class ExpenseAdvance : IEntityBase
    {
        public string EntityId { get => "ExpenseAdvanceId"; set { } }

        public int ExpenseAdvanceId { get; set; }
        public String Description { get; set; }
        public Decimal Amount { get; set; }
        public int? ExpenseProjectId { get; set; }
        public int? PaymentMethodId { get; set; }
        public String ExtraInfo { get; set; }
    }
}
