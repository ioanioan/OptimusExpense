using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.Models
{
    public class AspNetRoles
    {
        public String Id { get; set; }

        public String Name { get; set; }
        public String NormalizedName { get; set; }

        public String ConcurrencyStamp { get; set; }
    }
}
