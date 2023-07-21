using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class ReportInfo
    {
        public Report Report { get; set; }
        public Dictionary<String,Object> Parameters { get; set; }
    }
}
