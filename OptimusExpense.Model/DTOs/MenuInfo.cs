using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class MenuInfo
    {
        public UserAction Menu { get; set; }

        public IEnumerable<UserAction> SubMenus { get; set; }
    }
}
