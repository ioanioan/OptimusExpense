using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class ReportDetailCombo: IEntityBase
    {
        public string EntityId { get => "ReportDetailComboId"; set { } }

        public int ReportDetailComboId { get; set; }

        public int ReportDetailId { get; set; }

        public String Querry { get; set; }

        public String QuerryType { get; set; }

        public String DisplayMember { get; set; }

        public String ValueMember { get; set; }
    }
}
