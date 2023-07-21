using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class PropertyEntityValue: IEntityBase
    {
        public int PropertyEntityValueId { get; set; }

        public int PropertyId { get; set; }

        public int Entity_Id { get; set; }

        public String Value { get; set; }
        public string EntityId { get => "PropertyEntityValueId"; set { }   }
    }
}
