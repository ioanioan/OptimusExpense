using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class ReportDetail: IEntityBase
    {
        public string EntityId { get => "ReportDetailId"; set { } }

        public int ReportDetailId { get; set; }

        public int ReportId { get; set; }

        public String Name { get; set; }

        public String Type { get; set; }

        public String ParameterName { get; set; }

        public String ParameterType { get; set; }

    }
}
