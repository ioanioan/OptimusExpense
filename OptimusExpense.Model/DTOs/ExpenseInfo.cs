using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class ExpenseInfo
    {
        public Expense Expense { get; set; }

        public String Vat { get; set; }
        public String ExpenseNature { get; set; }
        public String ExpenseProject { get; set; }
        public String Provider { get; set; }
        public String PaymentMethod { get; set; }
        public String ExpenseDocumentType { get; set; }
        public String CurrencyName { get; set; }
        public String NumeAngajat { get; set; }
        public DateTime? DataDecont { get; set; }
        public String NumarDecont { get; set; }
        public String StatusDecont { get; set; }

        public DocumentDetail DocumentDetail { get; set; }
    }
}
