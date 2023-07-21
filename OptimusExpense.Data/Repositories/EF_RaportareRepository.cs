using OptimusExpense.Data.Abstract;
using OptimusExpense.Infrastucture.Extensions;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OptimusExpense.Data.Repositories
{
    public class EF_RaportareRepository : EntityBaseRepository<EF_Raportare>, IEF_RaportareRepository
    {

         
        public EF_RaportareRepository(OptimusExpenseContext c ) : base(c)
        {
            
        }

        public EF_RaportareInfo SendEFactura(String server,String port,int id)
        {
            var facturaXML= GetContext().EF_RaportareXML.First(p => p.IdEFRaportare == id);
           return SendEFactura(server,port,facturaXML);
        }

        public EF_RaportareInfo SendEFacturi(String server, String port, FilterInfo param)
        {
            EF_RaportareInfo factura = new EF_RaportareInfo();
            var facturaXML = GetRaportari(param).Where(p => p.EF_Raportare.CodRaspuns == null).ToArray();
            foreach (var f in facturaXML)
            {
                factura= SendEFactura(server, port, f.EF_Raportare.IdEFRaportare);
            }

            return factura;
        }

        public EF_RaportareInfo SendEFactura(String server,String port,EF_RaportareXML factura)
        {
            EF_RaportareInfo objI = null;
            try
            {
                var result = SendDataSet(server, port, "1|" + factura.XMLRaportare);
                if (result != null)
                {
                    if (!result.Tables[0].Columns.Contains("index_incarcare"))
                    {
                        throw new Exception("" + result.Tables[1].Rows[0]["errorMessage"]);
                    }
                    var obj = this.Get(factura.IdEFRaportare);
                    objI = new EF_RaportareInfo { };

                    objI.EF_Raportare = obj;

                    obj.CodRaspuns = long.Parse("" + result.Tables[0].Rows[0]["index_incarcare"]);

                    obj.DataRaportare = DateTime.Now;
                    obj.DataRaspuns = DateTime.Now;

                    this.Save(obj);

                }
            }
            catch(Exception ex){
                throw new Exception("Eroare la trimitere numar:" + factura.IdEFRaportare + " " + ex.Message);
            } 
            return objI;
        }

        public EF_RaportareInfo GetStareEFactura(String server, String port, EF_RaportareInfo factura)
        {
            EF_RaportareInfo objI = null;
            var result = SendDataSet(server, port, "2|"+factura.  EF_Raportare.CodRaspuns);
            if (result != null)
            {
                if (!result.Tables[0].Columns.Contains("stare"))
                {
                    throw new Exception(""+result.Tables[1].Rows[0]["errorMessage"]);
                }
                var obj = this.Get(factura.EF_Raportare.IdEFRaportare);
                objI = new EF_RaportareInfo { };

                objI.EF_Raportare = obj;
                obj.MesajRaspuns = ""+result.Tables[0].Rows[0]["stare"];
                obj.IdTranzactieRaspuns = result.Tables[0].Columns.Contains("id_descarcare")?(long?) long.Parse("" + result.Tables[0].Rows[0]["id_descarcare"]):null;


                obj.DataRaspuns = DateTime.Now;

                this.Save(obj);

            }
            return objI;
        }


        public EF_RaportareInfo GetStariEFacturi(String server, String port, FilterInfo param)
        {
            EF_RaportareInfo factura = new EF_RaportareInfo { };
            var facturi = this.GetRaportari(param).Where(o => o.EF_Raportare.CodRaspuns != null && o.EF_Raportare.IdTranzactieRaspuns == null).ToList();
            foreach(var v in facturi)
            {
                factura= GetStareEFactura(server, port, v);
            }

            return factura;
        }





        public byte[] DownLoadEFactura(String server, String port, int id)
        {
            EF_Raportare factura = this.Get(id);
            var result = SendBytes(server, port, "3|" + factura.IdTranzactieRaspuns);
            return result;

        }

        private void SetDataFactura(EF_Raportare e, String xml)
        {
            DataSet ds = new DataSet();
            try
            {
                ds.ReadXml(new StringReader(xml), XmlReadMode.InferSchema);
                e.DataFactura = DateTime.ParseExact("" + ds.Tables["Invoice"].Rows[0]["IssueDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new Exception("Fisierul nu contine campul IssueDate!");
            }
        }

        public EF_Raportare UploadEFactura(String userId,String numarFactura, String xml)
        {
            EF_Raportare e = new EF_Raportare { };
            e.UserId= userId;
            e.NumarFactura = numarFactura;
            e.ImportedTime= DateTime.Now;
            SetDataFactura(e, xml);
            GetContext().EF_Raportare.Add(e);
            GetContext().SaveChanges();
            GetContext().EF_RaportareXML.Add(new EF_RaportareXML { IdEFRaportare = e.IdEFRaportare, XMLRaportare = xml });
            GetContext().SaveChanges();

            return e;
        }

        public void UpdateDataFactura()
        {
            var _context = GetContext();
            var facturiUpdate = (from p in _context.EF_Raportare
                                 join pp in _context.EF_RaportareXML on p.IdEFRaportare equals pp.IdEFRaportare
                                 where p.DataFactura == null
                                 select new EF_RaportareInfo
                                 {
                                     EF_Raportare = p,
                                     Xml = pp.XMLRaportare
                                 }).ToList();
            foreach(var f in facturiUpdate)
            {
                SetDataFactura(f.EF_Raportare, f.Xml);
                this.Save(f.EF_Raportare);
            }
        }

        public EF_RaportareInfo Remove(EF_RaportareInfo entity)
        {
            this.Remove(entity.EF_Raportare);
            this.DeleteBulk<EF_RaportareXML>(entity.EF_Raportare);
            return entity;
        }

        public void UplodNewFacturi(FilterInfo param)
        {
            var list = NewFacturi(param);
            foreach(var i in list)
            {
                UploadEFactura(param.UserId, i.InvoiceNumber, i.InvoiceUBL);
            }
        }

        public IQueryable<EF_RaportareInfo> GetRaportari(FilterInfo param)
        {
            if (param.Date != null)
            {
                param.Date = param.Date.Value.Date;
            }
            var _context=GetContext();
            return (from r in _context.EF_Raportare
                    join u in _context.AspnetUsers on r.UserId equals u.Id
                    join x in _context.EF_RaportareXML on r.IdEFRaportare equals x.IdEFRaportare
                   where ( param.Date==null ||param.Date<=r.DataFactura)
                   && (param.DateEnd == null || param.DateEnd >= r.DataFactura)
                    select new EF_RaportareInfo
                    {
                        EF_Raportare = r,
                        User = u.UserName,
                        Xml = x.XMLRaportare
                    }).OrderByDescending(p => p.EF_Raportare.ImportedTime);
        }

        private String Send(String server,String port, String message)
        {
            var bytes=SendBytes(server,port,message);
            var result = Encoding.ASCII.GetString(bytes).Replace("\0", "");
            return result;
        }

        private byte[] SendBytes(String server, String port, String message)
        {
            byte[] result = new byte[] {};
            try
            {
                //Int32 port = 25566;
                TcpClient client = new TcpClient(server, int.Parse(port));
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);
                if (client.ReceiveBufferSize > 0)
                {
                    var bytes = new byte[client.ReceiveBufferSize];
                    stream.Read(bytes, 0, client.ReceiveBufferSize);
                    result = bytes;

                }
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                throw;
            }
            catch (SocketException e)
            {
                throw;
            }

            return result;
        }


        private DataSet SendDataSet(String server, String port, String message)
        {
            var result=Send(server,port,message);
            DataSet ds = new DataSet();
            try
            {
                var nr = 0;
                while (nr < 10&& result.Contains("Fisier incomplet"))
                {
                   
                        Thread.Sleep(3000);
                        result = Send(server, port, message);
                    nr++;
                }
                ds.ReadXml(new StringReader(result), XmlReadMode.InferSchema);
                return ds;

            }
            catch (Exception ex)
            {
                throw new Exception(result);
            }

            return null;
        }



        private List<FacturaPH> NewFacturi(FilterInfo param)
        {
            var start = DateTime.Now.Date.AddDays(-3);
            var end=DateTime.Now.Date.AddDays(1);
            var nrFact = "";

            //in cazul in care ruleaza cu parametrii suprascriu valorile initiale
            start = param.Date != null ? (DateTime)param.Date : start;
            end = param.DateEnd != null ? (DateTime)param.DateEnd : end;
            nrFact = param.Value != null ? param.Value : null;
            var _context = GetContext();
            var lastFacturi= GetContext().Database.ExecuteSqlRawExt<FacturaPH>("	exec [EFACTURA]. PharmalogData.[dbo].[SP_eFactura] "+ (nrFact!=null&&nrFact!=""?   "@StrictVoucherNumber ='"+ nrFact +"',":"") + " @DataStart='" + start.ToString("yyyyMMdd") + "',@DataStop='" + end.ToString("yyyyMMdd") + "'");
            var facturi = lastFacturi.Select(p => p.InvoiceNumber);
            var facturiExista = _context.EF_Raportare.Where(p => facturi.Contains(p.NumarFactura));
            return (from f in lastFacturi
                   from ff in facturiExista.Where(p=>p.NumarFactura==f.InvoiceNumber).DefaultIfEmpty()
                   where ff==null ||ff.IdEFRaportare==0
                   select f).ToList();
  
        }

        public class FacturaPH
        {
            public String InvoiceNumber { get; set; }

            public String InvoiceUBL { get; set; }
        }
    }
}
