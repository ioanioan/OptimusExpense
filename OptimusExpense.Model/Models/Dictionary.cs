using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class Dictionary
    {
        public int DictionaryId { get; set; }
        public String Name { get; set; }
        public String Code { get; set; }
        public bool ValueEditable { get; set; }
    }
}
