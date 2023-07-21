using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class Partner: IEntityBase
    {
        public string EntityId { get => "PartnerId"; set { } }
        public int PartnerId { get; set; }
        public String Code { get; set; }
        public String Name { get; set; }
        public String FiscalCode { get; set; }
        public String RegComCode { get; set; }
        public int PartnerTypeId { get; set; }
        public int CompanyId { get; set; }
        public int StatusId { get; set; }
    }
}
