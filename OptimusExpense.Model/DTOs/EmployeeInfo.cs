using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class EmployeeInfo
    {
        public Employee Employee { get; set; }
        public String PartnerPointName { get; set; }
        public String SuperiorName { get; set; }
        public String AccountingName { get; set; }
        public String EmployeeName { get; set; }
        public String EmailAprobator { get; set; }
        public String EmailContabil { get; set; }
        public String StatusName { get; set; }
        public String CostCenterName { get; set; }
        public String SectionName { get; set; }
    }
}
