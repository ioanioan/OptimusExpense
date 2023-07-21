using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Model.DTOs
{
    public class PartnerInfo
    {
        public Partner Partner { get; set; }
        public IEnumerable<PartnerPoint> PartnerPoints { get; set; }
        public String PartnerStatus { get; set; }
        public String PartnerType { get; set; }
        public String Company { get; set; }
        public DateTime? LastLogin { get; set; }
        public String PartnerNameCUI { get { return Partner.Name + "(" + Partner.FiscalCode + ")"; } }
    }
}
