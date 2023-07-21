using Newtonsoft.Json;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    
    public class EF_RaportareInfo
    {
      
        public EF_Raportare EF_Raportare { get; set; }

        [IgnoreDataMember]
        public String Xml { get; set; }

     
        public String User { get; set; }
    }
}
