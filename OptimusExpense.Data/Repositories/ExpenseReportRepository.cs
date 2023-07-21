using OptimusExpense.Data.Abstract;
using OptimusExpense.Infrastucture;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OptimusExpense.Infrastucture.Extensions;
using System.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace OptimusExpense.Data.Repositories
{
    public class ExpenseReportRepository: EntityBaseRepository<ExpenseReport>, IExpenseReportRepository
    {
        IDocumentRepository _repDoc;
        IDocumentDetailRepository _repDocDetail;
        IPropertyEntityValueRepository _propertyEntityValueRepository;
        ISerialNumberRepository _serialNumberRepository;
        OptimusExpenseContext _context;
        IEmailSender _emailSender;
        IEmployeeRepository _repEmp;

        IConfiguration _configuration;

        public ExpenseReportRepository(OptimusExpenseContext c, IDocumentRepository repDoc, IDocumentDetailRepository repDocDetail, IEmailSender emailSender, IEmployeeRepository repEmp,
             IPropertyEntityValueRepository propertyEntityValueRepository,
             ISerialNumberRepository serialNumberRepository,
             IConfiguration configuration) : base(c)
        {
            _repDoc = repDoc;
            _repDocDetail = repDocDetail;
            _emailSender = emailSender;
            _repEmp = repEmp;
            _context = c;
            _propertyEntityValueRepository = propertyEntityValueRepository;
            _serialNumberRepository = serialNumberRepository;
            _configuration = configuration;
        }

        public ExpenseReportInfo Save(ExpenseReportInfo entity)
        {
            entity.Document.DocumentTypeId = DocumentTypeEnum.ExpenseReport.GetHashCode();
            if (entity.Document.Number == null)
            {
                _serialNumberRepository.SetNumberDocument(entity.Document);
            }
            
        
            if ( entity.Document!=null!&&entity.ApproveType!=null&& entity.ApproveType!="")
            {
                int? propType = null;
                var angajat = entity.Document.DocumentId>0?_repEmp.GetEmployeeByDocumentId(entity.Document.DocumentId):_repEmp.GetEmployeeByUserId(entity.Document.CreatedByUserId);
                var email = "";
                var subject = "Detalii decont nr. " + entity.Document.Number + "(" + angajat.EmployeeName + ")";
                var body = "";
                var link = "<a href='"+ _configuration["Url"]+"/expense/ListExpenseReport?id="+entity.Document.Number + "'>aici" + "</a>";
                if (entity.Document.StatusId == DictionaryDetailType.CanceledSup.GetHashCode()|| entity.Document.StatusId == DictionaryDetailType.CanceledCont.GetHashCode())
                {
                    if (entity.ApproveType=="send")
                    {
                        entity.Document.StatusId = DictionaryDetailType.Generated.GetHashCode();
                    }
                    else if (entity.ApproveType == "approve")
                    {
                        if (entity.IsCont)
                        {
                            entity.Document.StatusId = DictionaryDetailType.ApproveCont.GetHashCode();
                        } 
                        else if (entity.IsSup)
                        {
                            entity.Document.StatusId = DictionaryDetailType.Validated.GetHashCode();
                        }
                    }
                }
                switch (entity.ApproveType)
                {
                    case "send":
                        if(entity.Document.StatusId >= DictionaryDetailType.Validated.GetHashCode())
                        {
                            entity.Document.StatusId = DictionaryDetailType.Validated.GetHashCode();
                            email = angajat.EmailAprobator;
                            body += "Draga " + angajat.SuperiorName + "," + "<br/><br/>";
                            body += angajat.EmployeeName + " a transmis o noua cerere de decont." + "<br/><br/>";
                            body += "Va rugam sa mergeti la sectiunea aprobare cerere de decont pentru a vedea mai multe detalii si pentru a aproba decontul click "+ link+" ." + "<br/><br/>";
                            body += "Detalii decont:" + "<br/>";
                            body += "Nr. decont: "+ entity.Document.Number + "<br/>";
                            body += "Descriere decont: " + entity.ExpenseReport.Description + "<br/>";
                            body += "Valoare decont: " + entity.SumaDecont + "<br/>";
                            body += "Status decont: TRIMIS" + "<br/><br/>";
                            body += "Cu stima.";
                        }
                        break;
                    case "approve":
                        entity.Document.StatusId = entity.Document.StatusId > OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode() ? OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode() : OptimusExpense.Infrastucture.DictionaryDetailType.ApproveCont.GetHashCode();
                        email = entity.Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode() ? angajat.EmailContabil : angajat.Employee.Email;
                        propType = entity.Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode() ? PropertyType.DecontObsSup.GetHashCode(): PropertyType.DecontObsCont.GetHashCode();
                        if(entity.Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode())
                        {
                            body += "Draga " + angajat.AccountingName + "," + "<br/><br/>";
                            body += angajat.EmployeeName + " a transmis o noua cerere de decont." + "<br/><br/>";
                            body += "Va rugam sa mergeti la sectiunea aprobare cerere de decont pentru a vedea mai multe detalii si pentru a aproba decontul click "+ link + "." + "<br/><br/>";
                            body += "Detalii decont:" + "<br/>";
                            body += "Nr. decont: " + entity.Document.Number + "<br/>";
                            body += "Descriere decont: " + entity.ExpenseReport.Description + "<br/>";
                            body += "Valoare decont: " + entity.SumaDecont + Environment.NewLine;
                            body += "Status decont: APROBAT" + "(" + entity.ApproveDescription + ")" + "<br/><br/>";
                            body += "Cu stima.";
                        }
                        else
                        {
                            body += "Draga " + angajat.EmployeeName + "," + "<br/><br/>";
                            body += "Decontul dumneavoastra a fost APROBAT, gasiti mai jos detalii legate de decontul pe care l-ati transmis click "+ link + "." + "<br/><br/>";
                            body += "Detalii decont:" + "<br/>";
                            body += "Nr. decont: " + entity.Document.Number + "<br/>";
                            body += "Descriere decont: " + entity.ExpenseReport.Description + "<br/>";
                            body += "Valoare decont: " + entity.SumaDecont + "<br/>";
                            body += "Status decont: APROBAT" + "(" + entity.ApproveDescription + ")" + "<br/><br/>";
                            body += "Cu stima.";
                        }
                        
                        break;
                    case "canceled":
                        entity.Document.StatusId = entity.Document.StatusId > OptimusExpense.Infrastucture.DictionaryDetailType.CanceledSup.GetHashCode() ? OptimusExpense.Infrastucture.DictionaryDetailType.CanceledSup.GetHashCode() : OptimusExpense.Infrastucture.DictionaryDetailType.CanceledCont.GetHashCode();
                        email = entity.Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.CanceledSup.GetHashCode() ? angajat.Employee.Email : angajat.EmailAprobator+";" + angajat.Employee.Email ;
                        propType = entity.Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.CanceledSup.GetHashCode() ? PropertyType.DecontObsSup.GetHashCode() : PropertyType.DecontObsCont.GetHashCode();
                        if (entity.Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.CanceledSup.GetHashCode())
                        {
                            body += "Draga " + angajat.EmployeeName + "," + "<br/><br/>";
                            body += "Decontul dumneavoastra a fost RESPINS, gasiti mai jos detalii legate de decontul pe care l-ati transmis click "+ link + "." + "<br/><br/>";
                            body += "Detalii decont:" + "<br/>";
                            body += "Nr. decont: " + entity.Document.Number + "<br/>";
                            body += "Descriere decont: " + entity.ExpenseReport.Description + "<br/>";
                            body += "Valoare decont: " + entity.SumaDecont + "<br/>";
                            body += "Status decont: RESPINS" + "(" + entity.ApproveDescription + ")" + "<br/><br/>";
                            body += "Cu stima.";
                        }
                        else
                        {
                            body += "Buna ziua, " + "<br/><br/>";
                            body += "Decontul dumneavoastra a fost RESPINS, gasiti mai jos detalii legate de decontul respectiv click "+ link + "." + "<br/><br/>";
                            body += "Detalii decont:" + "<br/>";
                            body += "Nr. decont: " + entity.Document.Number + "<br/>";
                            body += "Descriere decont: " + entity.ExpenseReport.Description + "<br/>";
                            body += "Valoare decont: " + entity.SumaDecont + "<br/>";
                            body += "Status decont: RESPINS" + "(" + entity.ApproveDescription + ")" + "<br/><br/>";
                            body += "Cu stima.";
                        }
                        break;                    
                }

                //trimitere mail in functie de status
                body += "<br/><br/><img src='https://optimus.ro/images/optimus.png' width ='200'/>" + "<br/>";
                body += "<i style='color: grey; font-size: 13px;'>Acesta este un mesaj generat automat - va rugam sa nu raspundeti.<br/>Optimus Expense, un produs <a href='https://Optimus.ro' target='_blank'>www.Optimus.ro</a><br/>Mergeti la profilul dumneavoastra pentru dezabonare.</i>";
                _emailSender.SendEmailAsync(email, subject, body);
                //end trimitere mail

                if (propType != null && entity.ApproveDescription != null&& entity.ApproveDescription!="")
                {
                    _propertyEntityValueRepository.Save(new PropertyEntityValue { Entity_Id=entity.Document.DocumentId, PropertyId=(int)propType, Value=entity.ApproveDescription });
                }
                var dic=_context.DictionaryDetail.FirstOrDefault(p => p.DictionaryDetailId == entity.Document.StatusId);
                entity.StatusName = dic != null ? dic.Name : "";
                entity.ApproveType = null;
                entity.ApproveDescription = null;
            }
            var listExpense = entity.SelectedExpense.Select(p => (int?)p.Expense.ExpenseId).ToArray();
            if (entity.Document.DocumentId > 0 && _context.DocumentDetail != null)
            {
                var deleDet = _context.DocumentDetail.Where(p => entity.Document.DocumentId == p.DocumentId && !listExpense.Contains(p.ItemId));
                foreach (var d in deleDet)
                {
                    _context.Remove(d);
                }
                var listDet=_context.DocumentDetail.Where(p => p.DocumentId == entity.Document.DocumentId);
                foreach(var d in entity.SelectedExpense.Where(p => p.DocumentDetail == null))
                {
                    d.DocumentDetail = listDet.FirstOrDefault(p => p.ItemId == d.Expense.ExpenseId);
                }
            }
            _repDoc.Save(entity.Document);
            foreach(var l in entity.SelectedExpense)
            {
                if (l.DocumentDetail == null)
                {
                    l.DocumentDetail = new DocumentDetail();
                    
                }
                l.DocumentDetail.ItemId = l.Expense.ExpenseId;
                l.DocumentDetail.Quantity = 1;
                l.DocumentDetail.UnitPrice = l.Expense.Price;
                l.DocumentDetail.DocumentId = entity.Document.DocumentId;
                l.DocumentDetail.VatId = l.Expense.VatId;
                _repDocDetail.Save(l.DocumentDetail);
            }
            entity.ExpenseReport.ExpenseReportId = entity.Document.DocumentId;
            
            base.Save(entity.ExpenseReport);
            return entity;
        }

        public ExpenseReportInfo Remove(ExpenseReportInfo entity)
        {

            foreach (var a in entity.SelectedExpense.Select(p => p.DocumentDetail))
            {
                _repDocDetail.Remove(a);
            }
            _repDoc.Remove(entity.Document);
            base.Remove(entity.ExpenseReport);
            return entity;
        }



        public IQueryable<ExpenseReportInfo> GetListExpenseReport(FilterInfo param)
        {
            int[] propertyDecont = new int[] { PropertyType.DecontObsCont.GetHashCode(), PropertyType.DecontObsSup.GetHashCode() };
            var ApproveSup = OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode();
            var Validatet = OptimusExpense.Infrastucture.DictionaryDetailType.Validated.GetHashCode();

            var isAdmin = (from r in _context.AspNetRoles
                           join ur in _context.AspNetUserRoles on r.Id equals ur.RoleId
                           where r.Name == "Admin" && ur.UserId == param.UserId
                           select r).FirstOrDefault() != null;

            var listA = (from aa in _context.AspnetUsers
                         join e in _context.Employee on aa.EmployeeId equals e.SuperiorEmployeeId
                         join u in _context.AspnetUsers on e.EmployeeId equals u.EmployeeId
                         where aa.Id == param.UserId
                         select new { UserId = u.Id });

            var listC = (from aa in _context.AspnetUsers
                         join e in _context.Employee on aa.EmployeeId equals e.AccountingEmployeeId
                         join u in _context.AspnetUsers on e.EmployeeId equals u.EmployeeId
                         where aa.Id == param.UserId
                         select new { UserId = u.Id });

            var listPropertyMax = (from d in _context.Document
                                   join p in _context.PropertyEntityValue on d.DocumentId equals p.Entity_Id
                                   where propertyDecont.Contains(p.PropertyId)
                                   group new { p.Entity_Id, p.PropertyEntityValueId } by new { p.Entity_Id } into g
                                   select new { Entity_Id = g.Key.Entity_Id, MaxId = g.Max(p => p.PropertyEntityValueId) });

            var listProperty = (from l in listPropertyMax
                                join p in _context.PropertyEntityValue on l.MaxId equals p.PropertyEntityValueId
                                select new { Entity_Id = l.Entity_Id, ObsStatus = p.Value });

            var list = (from er in _context.ExpenseReport
                        join d in _context.Document on er.ExpenseReportId equals d.DocumentId
                        join au in _context.AspnetUsers on d.CreatedByUserId equals au.Id
                        join pers in _context.Person on au.EmployeeId equals pers.PersonId
                        join emp in _context.Employee on pers.PersonId equals emp.EmployeeId
                        join part in _context.PartnerPoint on emp.PartnerPointId equals part.PartnerPointId
                        join dd in _context.DictionaryDetail on d.StatusId equals dd.DictionaryDetailId
                        from uA in listA.Where(p => p.UserId == d.CreatedByUserId).DefaultIfEmpty()
                        from uC in listC.Where(p => p.UserId == d.CreatedByUserId).DefaultIfEmpty()
                            //from os in listProperty.Where(p => p.Entity_Id ==   d.DocumentId).DefaultIfEmpty()
                        where ((d.CreatedByUserId == param.UserId || (uA.UserId != null && d.StatusId <= Validatet)
                         || (uC.UserId != null)) //s-a dorit contabilii sa vada toate deconturile asa ca am sters conditia  "&& d.StatusId <= ApproveSup" cum era la aprobator
                          &&
                          (param.Id == 0 || er.ExpenseReportId == param.Id)
                          && (param.Date == null || (param.Date <= d.Date && param.DateEnd >= d.Date))) || isAdmin
                        select new Model.DTOs.ExpenseReportInfo
                        {
                            UserIdView = param.UserId,
                            ExpenseReport = er,
                            Document = d,
                            NumeAngajat = pers.FirstName + " " + pers.LastName,
                            PlAngajat = part.Name,
                            StatusName = dd.Name,
                            //ObsStatus = os.ObsStatus,
                            IsCont = uC.UserId != null,
                            IsSup = uA.UserId != null,
                            //Enabled =d.StatusId==Generated||(d.StatusId==ApproveCont&&uC.UserId!=null),
                            //EnabledV = (d.StatusId == Generated&&uA.UserId!=null) || (d.StatusId == ApproveCont && uC.UserId != null),
                            _ListExpense = (from dd in _context.DocumentDetail
                                            join e in _context.Expense on dd.ItemId equals e.ExpenseId
                                            where dd.DocumentId == d.DocumentId
                                            select new Model.DTOs.ExpenseInfo
                                            {
                                                DocumentDetail = dd,
                                                Expense = e
                                            }).ToList()


                        });

            var result = param.NrRows > 0 ? list.OrderByDescending(p => p.Document.StatusId).ThenByDescending(p => p.Document.Date).Take(param.NrRows) : list.OrderByDescending(p => p.Document.StatusId).ThenByDescending(p => p.Document.Date);
            if (param.Id > 0)
            {
                foreach (var l in list)
                {
                    l._ListExpense = (from dd in _context.DocumentDetail
                                      join e in _context.Expense on dd.ItemId equals e.ExpenseId
                                      where dd.DocumentId == l.Document.DocumentId
                                      select new Model.DTOs.ExpenseInfo
                                      {
                                          DocumentDetail = dd,
                                          Expense = e
                                      }).ToList();
                }

            }
            return result;
        }


        class TMP
        {
            public int ExpenseReportId { get; set; }
            public String Description { get; set; }
        }

        public DataSet GetRaportExpenseReport(FilterInfo filter)
        {
         
            var list = new Microsoft.Data.SqlClient.SqlParameter[] {
              new Microsoft.Data.SqlClient.SqlParameter("@ExpenseReportId", filter.Id),
               new Microsoft.Data.SqlClient.SqlParameter("@UserId", filter.UserId)
            };

            var result = _context.Database.ExecuteSqlDataTable("exec spGetRaportExpenseReport @ExpenseReportId, @UserId", list);
            return result;
        }

        public List<DashboardExpenseReportInfo> GetDashboardExpenseReport(FilterInfo param)
        {
            
            var list = new Microsoft.Data.SqlClient.SqlParameter[] {
               new Microsoft.Data.SqlClient.SqlParameter("@UserId", param.UserId)
            };

            var result = _context.Database.ExecuteSqlRawExt<DashboardExpenseReportInfo>("exec spGetDashboardExpenseReport @UserId", list).ToList();
            return result;
        }
    }
}
