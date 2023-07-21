using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class Document : IEntityBase
    {
        public string EntityId { get => "DocumentId"; set { } }

        public int DocumentId { get; set; }

        public int DocumentTypeId { get; set; }

        public String Number { get; set; }

        public int? SerialNumberId { get; set; }

        public DateTime? Date { get; set; }

        public DateTime ServerDate { get; set; }

        public int StatusId { get; set; }

        public int? PrintCount { get; set; }

        public String CreatedByUserId { get; set; }
    }
}
