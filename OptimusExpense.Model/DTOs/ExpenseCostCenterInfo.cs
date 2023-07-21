using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class ExpenseCostCenterInfo
    {
        public ExpenseCostCenter ExpenseCostCenter { get; set; }
        public String CboxName { get { return ExpenseCostCenter.Name + "(" + ExpenseCostCenter.Code + ")"; } }
    }
}
