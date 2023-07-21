using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class DictionaryDetail: IEntityBase
    {
        public string EntityId { get => "DictionaryDetailId"; set { } }
        public int DictionaryDetailId { get; set; }
        public int DictionaryId { get; set; }
        public String Name { get; set; }
        public String Code { get; set; }
        public int? DisplayOrder { get; set; }
        public String Value { get; set; }
        public bool Active { get; set; }
    }
}
