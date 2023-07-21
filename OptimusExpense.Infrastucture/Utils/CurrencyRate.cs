using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;
using System.Linq;
using System.Globalization;

namespace OptimusExpense.Infrastucture.Utils
{
    public class CurrencyRate
    {
        public static decimal? Rate(String code)
        {
            decimal? rr = null;
            var s = code.ToLower();
            if (s == "ron")
            {
                return 1;
            }
            var listRates = ListRates();
            var  r=listRates.FirstOrDefault(p => p.Code == s) ;
            if (r != null)
            {
                rr = r.Rate;
                if (r.Paritate != null)
                {
                    rr /= r.Paritate;
                }
            }

            return rr;

        }


        public static List<DictionaryRate> ListRates()
        {
            List<DictionaryRate> newListaReturnat = new List<DictionaryRate>();
            WebClient client = new WebClient();
            String htmlCode = client.DownloadString("https://bnr.ro/nbrfxrates.xml");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(htmlCode);

            //var xnList = doc.DocumentElement.SelectNodes("Rate");

            var xnList = doc.GetElementsByTagName("Rate");//.AsQueryable();

            DictionaryRate curs;
            foreach (XmlNode xn in xnList)
            {
                curs = new DictionaryRate();
                curs.Code = xn.Attributes["currency"].Value.ToLower();// xn.Name;
                curs.Rate = Convert.ToDecimal(xn.InnerText, CultureInfo.InvariantCulture);
                if (xn.Attributes["multiplier"] != null)
                {
                    curs.Paritate = Convert.ToInt32(xn.Attributes["multiplier"].Value);
                }
                curs.Data = Convert.ToDateTime(xn.ParentNode.Attributes["date"].Value);
                newListaReturnat.Add(curs);
            }
            return newListaReturnat;
        }
    }

    [Serializable]
    public class DictionaryRate  
    {
        public string Code { get; set; }
        public decimal Rate { get; set; }
        public int? Paritate { get; set; }
        public DateTime Data { get; set; }

    }
}
