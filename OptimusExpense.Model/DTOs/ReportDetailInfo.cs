using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class ReportDetailInfo
    {
        public ReportDetail ReportDetail { get; set; }

        public ReportDetailCombo ReportDetailCombo { get; set; }

        public IEnumerable<dynamic> ReportDetailComboResult { get; set; }
    }
}
