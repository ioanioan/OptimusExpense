using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class Expense : IEntityBase
    {
        public string EntityId { get => "ExpenseId"; set { } }

        public int ExpenseId { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public int CurrencyId{ get; set; }
        public int? VatId { get; set; }
        public String Description { get; set; }

        public int? ExpenseNatureId { get; set; }

        public int? ExpenseDocumentTypeId { get; set; }

        public String DocumentNumber { get; set; }

        public int? ProviderId { get; set; }

        public int? ExpenseProjectId { get; set; }

        public int? PaymentMethodId { get; set; }

        public String ExtraInfo { get; set; }

        public String UserId { get; set; }

        public String FilePath { get; set; }

        public int? ParentExpenseId { get; set; }

        public String FiscalCode { get; set; }

        public decimal? CurrencyRate { get; set; }

    }
}
