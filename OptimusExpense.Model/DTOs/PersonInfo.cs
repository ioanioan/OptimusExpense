using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class PersonInfo
    {
        public Person Person { get; set; }
        public String PartnerName { get; set; }
        public String PositionName { get; set; }
        public String FullName { get { return Person.FirstName + " " + Person.LastName; }}
    }
}
