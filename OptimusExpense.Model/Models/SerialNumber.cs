using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class SerialNumber : IEntityBase
    {
        public string EntityId { get => "SerialNumberId"; set { } }

        public int SerialNumberId { get; set; }

        public String Series { get; set; }

        public int Number { get; set; }

        public int DocumentTypeId { get; set; }

        public int? PartnerId { get; set; }

    }
}
