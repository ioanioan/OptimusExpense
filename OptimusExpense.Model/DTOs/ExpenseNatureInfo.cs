using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class ExpenseNatureInfo
    {
        public ExpenseNature ExpenseNature { get; set; }
        public String Vat { get; set; }
        public String CboxName { get { return ExpenseNature.Name + " " + Vat.Replace("TVA","") + " ("+ ExpenseNature.ContContabil + ")"; } }
    }
}
