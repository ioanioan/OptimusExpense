using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class Employee: IEntityBase
    {
        public string EntityId { get => "EmployeeId"; set { } }
        public int EmployeeId { get; set; }
        public int PartnerPointId { get; set; }
        public String Phone { get; set; }
        public String Email { get; set; }    
        public DateTime? HireDate { get; set; }
        public int? SuperiorEmployeeId { get; set; }
        public int? AccountingEmployeeId { get; set; }
        public String Marca { get; set; }
        public int? CostCenterId { get; set; }
        public int? StatusId { get; set; }
        public String SectionCode { get; set; }
        public int? NormaAngajat { get; set; }
    }
}
