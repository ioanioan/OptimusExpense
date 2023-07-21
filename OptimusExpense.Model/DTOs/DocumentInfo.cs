using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class DocumentInfo
    {
        public OptimusExpense.Model.Models.Document Document { get; set; }

        public dynamic DocumentExt { get; set; }

        public IEnumerable<DocumentDetail> DocumentDetail { get; set; }
    }
}
