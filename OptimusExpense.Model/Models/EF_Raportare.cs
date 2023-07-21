using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public  class EF_Raportare: IEntityBase
    {
        public string EntityId { get => "IdEFRaportare"; set { } }

        public int IdEFRaportare { get; set; }
        public int IdFactura { get; set; }


        public String NumarFactura { get; set; }

        public DateTime? DataFactura { get; set; }
        public DateTime? DataRaportare { get; set; }
        public DateTime? DataRaspuns { get; set; }
        public long? CodRaspuns { get; set; }
        public String MesajRaspuns { get; set; }    
        public long? IdTranzactieRaspuns { get; set; }    

        public bool EsteActiva { get; set; }
        public String UserId { get; set; }
        public DateTime ImportedTime { get; set; }



    }
}
