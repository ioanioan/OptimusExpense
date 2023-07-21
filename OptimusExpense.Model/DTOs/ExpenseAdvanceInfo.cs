using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.Json.Serialization;

namespace OptimusExpense.Model.DTOs
{
    public class ExpenseAdvanceInfo
    {
        public ExpenseAdvance ExpenseAdvance { get; set; }
        public Document Document { get; set; }
        public String ApproveDescription { get; set; }
        public String ApproveType { get; set; }
        public String ExpenseProject { get; set; }
        public String PaymentMethod { get; set; }
        public bool IsCont { get; set; }
        public bool IsSup { get; set; }
        public bool Enabled { get => Document != null && (Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.Generated.GetHashCode() || (IsSup && Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.Validated.GetHashCode()) || (Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode() && IsCont)); }
        public bool EnabledV { get => Document != null && ((Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.Validated.GetHashCode() && IsSup) || (Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode() && IsCont)); }
        public String StatusName { get; set; }       
    }
}
