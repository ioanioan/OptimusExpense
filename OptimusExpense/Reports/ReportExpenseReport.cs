using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusExpense.Reports
{
    public   interface ReportExpenseReport
    {
        public String ExpenseReportId { get; set; }
        public String Description { get; set; }
		public String NrSerieDecont { get; set; }
		public String DataDecont { get; set; }
		public String ExpenseId { get; set; }
		public String Angajat { get; set; }
		public String FunctieAngajat { get; set; }
		public String NaturaCheltuiala { get; set; }
		public String SumaCheltuiala { get; set; }
		public String Moneda { get; set; }

		public String MonedaTotal { get; set; }
		
		public String TotalDecont { get; set; }
		public String Aprobator { get; set; }
		public String Contabil { get; set; }
		public String DataCreare { get; set; }
		public String DataAprobare { get; set; }
		public String DataAprobareContabil { get; set; }
		public String StatusDecont { get; set; }
		public String Partener { get; set; }
		public String TipCheltuiala { get; set; }
		public String DataCheltuiala { get; set; }
		public String NrCheltuiala { get; set; }
	}
}
