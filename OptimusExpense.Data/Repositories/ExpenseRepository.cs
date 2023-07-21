using Microsoft.Extensions.Logging;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    class ExpenseRepository : EntityBaseRepository<Expense>, IExpenseRepository
    {

        OptimusExpenseContext _context;
        IAspnetUsersRepository  _usersRepository;
        ICurrencyRepository _repCur;
        private readonly ILogger<ExpenseRepository> _logger;
        public ExpenseRepository(OptimusExpenseContext c, IAspnetUsersRepository usersRepository, ICurrencyRepository repCur, ILogger<ExpenseRepository> _logger) : base(c)
        {
            _context = c;
            _usersRepository = usersRepository;
            _repCur = repCur;
            this._logger = _logger;
        }

        public override Expense Save(Expense entity)
        {
            if (entity.CurrencyRate == null)
            {
                var curr= _repCur.Get().FirstOrDefault(p=> p.CurrencyId== entity.CurrencyId);
                if (curr != null)
                {
                    decimal? rate = null;
                    try
                    {
                        rate = Infrastucture.Utils.CurrencyRate.Rate(curr.Code);
                        _logger.LogInformation("Rate", rate);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError("RateError", rate,ex.Message);
                    }

                    if (rate == null)
                    {
                       var r= _context.Expense.Where(p => p.CurrencyId == entity.CurrencyId && p.CurrencyRate > 0).OrderByDescending(p => p.Date).FirstOrDefault();
                        if (r != null)
                        {
                            rate = r.CurrencyRate;
                        }
                    }

                    entity.CurrencyRate = rate;
                }

                
            }
            return base.Save(entity);
        }

        public IQueryable<ExpenseInfo> GetListForExpenseReport(String userId=null)
        {
            var result = (from r in _context.Expense
                          from v in _context.DictionaryDetail.Where(p=>p.DictionaryDetailId==r.VatId).DefaultIfEmpty()
                          join au in _context.AspnetUsers on r.UserId equals au.Id
                          join pers in _context.Person on au.EmployeeId equals pers.PersonId
                          join c in _context.Currency on r.CurrencyId equals c.CurrencyId
                          from en in _context.ExpenseNature.Where(p => p.ExpenseNatureId == r.ExpenseNatureId).DefaultIfEmpty()
                          from ep in _context.ExpenseProject.Where(p => p.ExpenseProjectId == r.ExpenseProjectId).DefaultIfEmpty()
                          from pr in _context.Partner.Where(p => p.PartnerId == r.ProviderId).DefaultIfEmpty() 
                          from py in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == r.PaymentMethodId).DefaultIfEmpty()
                          from ext in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == r.ExpenseDocumentTypeId).DefaultIfEmpty()
                          where (userId==null||r.UserId==userId)
                          select new ExpenseInfo
                          {
                              Expense = r,
                              Vat=v.Name,
                              ExpenseNature=en.Name==null? en.Name: en.Name + "(" + en.ContContabil + ")",
                              ExpenseProject =ep.Name,
                              Provider=pr.Name+"(" + pr.FiscalCode+ ")",
                              PaymentMethod=py.Name,
                              ExpenseDocumentType=ext.Name,
                              CurrencyName = c.Code,
                              NumeAngajat = pers.FirstName + " " + pers.LastName

                          }).OrderByDescending(p=>p.Expense.ExpenseId);
            return result;
        }

        public IQueryable<ExpenseInfo> GetListExpense(String userId = null)
        {
            var isAdmin = (from r in _context.AspNetRoles 
                             join ur in _context.AspNetUserRoles on r.Id equals ur.RoleId
                             where r.Name == "Admin" && ur.UserId == userId 
                             select r).FirstOrDefault() != null;
            
            var listA = (from aa in _context.AspnetUsers
                         join e in _context.Employee on aa.EmployeeId equals e.SuperiorEmployeeId
                         join u in _context.AspnetUsers on e.EmployeeId equals u.EmployeeId
                         where aa.Id == userId
                         select new { UserId = u.Id });

            var listC = (from aa in _context.AspnetUsers
                         join e in _context.Employee on aa.EmployeeId equals e.AccountingEmployeeId
                         join u in _context.AspnetUsers on e.EmployeeId equals u.EmployeeId
                         where aa.Id == userId
                         select new { UserId = u.Id });

            var result = (from r in _context.Expense
                          from dd in _context.DocumentDetail.Where(p => p.ItemId == r.ExpenseId).DefaultIfEmpty()
                          from d in _context.Document.Where(p => p.DocumentId == dd.DocumentId).DefaultIfEmpty()
                          from st in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == d.StatusId).DefaultIfEmpty()
                          from v in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == r.VatId).DefaultIfEmpty()
                          join au in _context.AspnetUsers on r.UserId equals au.Id
                          join pers in _context.Person on au.EmployeeId equals pers.PersonId
                          join c in _context.Currency on r.CurrencyId equals c.CurrencyId
                          from en in _context.ExpenseNature.Where(p => p.ExpenseNatureId == r.ExpenseNatureId).DefaultIfEmpty()
                          from ep in _context.ExpenseProject.Where(p => p.ExpenseProjectId == r.ExpenseProjectId).DefaultIfEmpty()
                          from pr in _context.Partner.Where(p => p.PartnerId == r.ProviderId).DefaultIfEmpty()
                          from py in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == r.PaymentMethodId).DefaultIfEmpty()
                          from ext in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == r.ExpenseDocumentTypeId).DefaultIfEmpty()
                          from uA in listA.Where(p => p.UserId == r.UserId).DefaultIfEmpty()
                          from uC in listC.Where(p => p.UserId == r.UserId).DefaultIfEmpty()
                          where (r.UserId == userId || (uA.UserId != null) || (uC.UserId != null)) || isAdmin
                          select new ExpenseInfo
                          {
                              Expense = r,
                              Vat = v.Name,
                              ExpenseNature = en.Name == null ? en.Name : en.Name + "(" + en.ContContabil + ")",
                              ExpenseProject = ep.Name,
                              Provider = pr.Name + "(" + pr.FiscalCode + ")",
                              PaymentMethod = py.Name,
                              ExpenseDocumentType = ext.Name,
                              CurrencyName = c.Code,
                              NumeAngajat = pers.FirstName + " " + pers.LastName,
                              DataDecont = d.Date,
                              NumarDecont = d.Number,
                              StatusDecont = st.Name??"Fara decont"

                          }).OrderByDescending(p => p.Expense.Date).ThenBy(pp=>pp.NumeAngajat);
            return result;
        }

        public IQueryable<ExpenseInfo> GetListExpenseDraft(FilterInfo param)
        {
            var y = param.Date.Value.Year;
            var m = param.Date.Value.Month;
            string userIdCreate = param.UserId;
            if (param.Id>0)
            {
                var doc=_context.Document.FirstOrDefault(pp=> pp.DocumentId == param.Id);
                if (doc != null)
                {
                    userIdCreate = doc.CreatedByUserId;
                }
            }
            var list = GetListForExpenseReport(userIdCreate);
            var result = (from l in list
                          from d in _context.DocumentDetail.Where(p => p.ItemId == l.Expense.ExpenseId).DefaultIfEmpty()
                          from er in _context.ExpenseReport.Where(p => p.ExpenseReportId == d.DocumentId).DefaultIfEmpty()
                          where //l.Expense.Date.Year == y
                          //&& l.Expense.Date.Month == m &&
                          ((param.Id>0&& er.ExpenseReportId==param.Id)|| (param.UserId==userIdCreate&&er.ExpenseReportId == null))
                          select new { l = l,Selectd = er.ExpenseReportId == param.Id }).OrderByDescending(p=>p.Selectd).Select(p=>p.l).OrderByDescending(p=>p.Expense.Date);
            return result;
        }

        public List<ExpenseInfo> GetListLastExpenses(FilterInfo param)
        {
            var list = (from r in _context.Expense
                        from dd in _context.DocumentDetail.Where(p => p.ItemId == r.ExpenseId).DefaultIfEmpty()
                        from v in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == r.VatId).DefaultIfEmpty()
                        join c in _context.Currency on r.CurrencyId equals c.CurrencyId
                        from en in _context.ExpenseNature.Where(p => p.ExpenseNatureId == r.ExpenseNatureId).DefaultIfEmpty()
                        from ep in _context.ExpenseProject.Where(p => p.ExpenseProjectId == r.ExpenseProjectId).DefaultIfEmpty()
                        from pr in _context.Partner.Where(p => p.PartnerId == r.ProviderId).DefaultIfEmpty()
                        from py in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == r.PaymentMethodId).DefaultIfEmpty()
                        from ext in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == r.ExpenseDocumentTypeId).DefaultIfEmpty()
                        where (param.UserId == null || r.UserId == param.UserId) && dd.DocumentDetailId == null
                        select new ExpenseInfo
                        {
                            Expense = r,
                            Vat = v.Name,
                            ExpenseNature = en.Name,
                            ExpenseProject = ep.Name,
                            Provider = pr.Name + "(" + pr.FiscalCode + ")",
                            PaymentMethod = py.Name,
                            ExpenseDocumentType = ext.Name,
                            CurrencyName = c.Code
                        });

            var result = param.NrRows > 0 ? list.OrderByDescending(p => p.Expense.ExpenseId).Take(param.NrRows).ToList() : list.OrderByDescending(p => p.Expense.ExpenseId).ToList();

            return result;
        }

        public dynamic GetGraphicExpenses(String userId)
        {
            var dt = DateTime.Now.Date.AddMonths(-6);
            var users = _usersRepository.GetSubordination(userId);
            var rr = (from e in _context.Expense
                      join u in users on e.UserId equals u
                      where e.Date>= dt
                      group new { e.Date.Year, e.Date.Month, e.Price } by new { e.Date.Year, e.Date.Month } into g
                      select new
                      {
                          Year = g.Key.Year,
                          Month = g.Key.Month,
                          Sum = g.Sum(pp => pp.Price)
                      });
            return rr;
        }
    }
}
