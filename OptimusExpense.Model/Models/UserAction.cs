using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class UserAction : IEntityBase
    {
        public string EntityId { get => "UserActionId"; set { } }

        public int UserActionId { get; set; }

        public String Code { get; set; }

        public String Component { get; set; }

        public String Name { get; set; }

        public int ParentUserActionId { get; set; }

        public String Description { get; set; }

        public String Action { get; set; }

        public int? Type { get; set; }

        public int? DisplayOrder { get; set; }

        public String Icon { get; set; }
    }
}
