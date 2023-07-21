using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class FilterInfo
    {
        public DateTime? Date { get; set; }
        public DateTime? DateEnd { get; set; }
        public int Id { get; set; }
        public String Value { get; set; }
        public String UserId { get; set; }
        public int NrRows { get; set; } //0 - all
    }
}
