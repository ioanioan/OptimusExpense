using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class PartnerPoint: IEntityBase
    {
        public string EntityId { get => "PartnerPointId"; set { } }
        public int PartnerPointId { get; set; }
        public int PartnerId { get; set; }
        public String Name { get; set; }
        public int? AddressId { get; set; }
        public bool? CommercialPoint { get; set; }
    }
}
