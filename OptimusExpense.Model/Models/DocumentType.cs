using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class DocumentType
    {
        public int DocumentTypeId { get; set; }

        public String Code { get; set; }

        public String Name { get; set; }

        public String HeaderTable { get; set; }

        public String DetailTable { get; set; }
    }
}
