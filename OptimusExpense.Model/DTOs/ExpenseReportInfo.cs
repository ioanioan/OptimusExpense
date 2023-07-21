using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.Json.Serialization;

namespace OptimusExpense.Model.DTOs
{
    public class ExpenseReportInfo
    {
        public ExpenseReport ExpenseReport { get; set; }
        public Document Document { get; set; }

        public String ApproveDescription { get; set; }
        public String NumeAngajat { get; set; }
        public String PlAngajat { get; set; }

        public String ApproveType { get; set; }

        [JsonIgnore]
        public IEnumerable<DTOs.ExpenseInfo> _ListExpense { get; set; }


        public bool IsCont { get; set; }

        public bool IsSup { get; set; }

        public String UserIdView { get; set; }

        public bool Enabled { get => Document != null && ((Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.Generated.GetHashCode()&&UserIdView==Document.CreatedByUserId) || Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.CanceledCont.GetHashCode() || Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.CanceledSup.GetHashCode() || (IsSup && Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.Validated.GetHashCode()) || (Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode() && IsCont)); }

        public bool EnabledV { get => Document != null && ((Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.Validated.GetHashCode() && IsSup) || (Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode() && IsCont)); }



        public IEnumerable<ExpenseInfo> SelectedExpense
        {
            get => _ListExpense;
            set { _ListExpense = value; }
        }

        public String StatusName { get; set; }
        public String ObsStatus { get; set; }
        public Decimal SumaDecont { get { return this.SelectedExpense == null ? 0 : this.SelectedExpense.Sum(p => p.Expense.Price); } }
    }
}
