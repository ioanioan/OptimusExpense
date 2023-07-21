using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class EF_RaportareXML : IEntityBase
    {
        public string EntityId { get => "IdEFRaportare"; set { } }

        public int IdEFRaportare { get; set; }
    
        public String XMLRaportare { get; set; }
    }
}
