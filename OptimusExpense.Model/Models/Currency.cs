using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class Currency : IEntityBase
    {
        public string EntityId { get => "CurrencyId"; set { } }

        public int CurrencyId { get; set; }

        public String Code { get; set; }

        public String Name { get; set; }

        public String Symbol { get; set; }

        public int? CountryId { get; set; }
    }
}
