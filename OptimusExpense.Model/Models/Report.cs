using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class Report : IEntityBase
    {
        public string EntityId { get => "ReportId"; set { } }

        public int ReportId { get; set; }

        public String Code { get; set; }

        public String Name { get; set; }

        public String StoredProcedureName { get; set; }


    }
}
