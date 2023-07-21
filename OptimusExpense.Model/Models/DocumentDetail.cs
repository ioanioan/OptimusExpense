using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class DocumentDetail : IEntityBase
    {
        public string EntityId { get => "DocumentDetailId"; set { } }

        public int DocumentDetailId { get; set; }
        public int DocumentId { get; set; }
        public int ItemId { get; set; }

        public decimal? Quantity { get; set; }

        public int? MeasuringUnitId { get; set; }

        public decimal? UnitPrice { get; set; }

        public int? VatId { get; set; }

        public decimal? UnitPriceWithVat { get; set; }

    }
}
