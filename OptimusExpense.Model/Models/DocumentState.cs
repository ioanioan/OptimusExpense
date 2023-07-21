using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class DocumentState: IEntityBase
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }

        public int DocumentStateId { get; set; }

        public String UserId { get; set; }
        public DateTime TransitionMoment { get; set; }
        public string EntityId { get => "Id"; set { } }
    }
}
