using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class Log : IEntityBase
    {
        public string EntityId { get => "LogId"; set { } }
        public int LogId { get; set; }

        public String UserId { get; set; }

        public String Action { get; set; }
        
        public String Value { get; set; }

        public DateTime Date { get; set; }
    }
}
