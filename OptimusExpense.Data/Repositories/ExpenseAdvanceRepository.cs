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

namespace OptimusExpense.Data.Repositories
{
    public class ExpenseAdvanceRepository: EntityBaseRepository<ExpenseAdvance>, IExpenseAdvanceRepository
    {
        IDocumentRepository _repDoc;
        IDocumentDetailRepository _repDocDetail;
        IPropertyEntityValueRepository _propertyEntityValueRepository;
        OptimusExpenseContext _context;
        IEmailSender _emailSender;
        IEmployeeRepository _repEmp;
        
        ISerialNumberRepository _serialNumberRepository;

        public ExpenseAdvanceRepository(OptimusExpenseContext c, IDocumentRepository repDoc, IDocumentDetailRepository repDocDetail, IEmailSender emailSender, IEmployeeRepository repEmp,
             IPropertyEntityValueRepository propertyEntityValueRepository,
            ISerialNumberRepository  serialNumberRepository) : base(c)
        {
            _repDoc = repDoc;
            _repDocDetail = repDocDetail;
            _emailSender = emailSender;
            _repEmp = repEmp;
            _context = c;
            _propertyEntityValueRepository = propertyEntityValueRepository;
            _serialNumberRepository = serialNumberRepository;
        }

        public ExpenseAdvanceInfo Save(ExpenseAdvanceInfo entity)
        {
            entity.Document.DocumentTypeId = DocumentTypeEnum.ExpenseAdvance.GetHashCode();
            if (entity.Document.Number == null)
            {
                _serialNumberRepository.SetNumberDocument(entity.Document);
            }

            if ( entity.Document!=null!&&entity.ApproveType!=null&& entity.ApproveType!="")
            {
                int? propType = null;
                switch (entity.ApproveType)
                {
                    case "send":
                        if(entity.Document.StatusId >= DictionaryDetailType.Validated.GetHashCode())
                        {
                            entity.Document.StatusId = DictionaryDetailType.Validated.GetHashCode();
                        }
                        break;
                    case "approve":
                        entity.Document.StatusId = entity.Document.StatusId > OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode() ? OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode() : OptimusExpense.Infrastucture.DictionaryDetailType.ApproveCont.GetHashCode();
                        propType = entity.Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.ApproveSup.GetHashCode() ? PropertyType.AdvanceObsSup.GetHashCode(): PropertyType.AdvanceObsCont.GetHashCode();
                        break;
                    case "canceled":
                        entity.Document.StatusId = entity.Document.StatusId > OptimusExpense.Infrastucture.DictionaryDetailType.CanceledSup.GetHashCode() ? OptimusExpense.Infrastucture.DictionaryDetailType.CanceledSup.GetHashCode() : OptimusExpense.Infrastucture.DictionaryDetailType.CanceledCont.GetHashCode();
                        propType = entity.Document.StatusId == OptimusExpense.Infrastucture.DictionaryDetailType.CanceledSup.GetHashCode() ? PropertyType.AdvanceObsSup.GetHashCode() : PropertyType.AdvanceObsCont.GetHashCode();
                        break;                    
                }

                if (propType != null && entity.ApproveDescription != null&& entity.ApproveDescription!="")
                {
                    _propertyEntityValueRepository.Save(new PropertyEntityValue { Entity_Id=entity.Document.DocumentId, PropertyId=(int)propType, Value=entity.ApproveDescription });
                }
                var dic=_context.DictionaryDetail.FirstOrDefault(p => p.DictionaryDetailId == entity.Document.StatusId);
                entity.StatusName = dic != null ? dic.Name : "";
                entity.ApproveType = null;
                entity.ApproveDescription = null;
            }
            _repDoc.Save(entity.Document);
            entity.ExpenseAdvance.ExpenseAdvanceId = entity.Document.DocumentId;
            
            base.Save(entity.ExpenseAdvance);
            return entity;
        }

        public ExpenseAdvanceInfo Remove(ExpenseAdvanceInfo entity)
        {
            _repDoc.Remove(entity.Document);
            base.Remove(entity.ExpenseAdvance);
            return entity;
        }



        public IQueryable<ExpenseAdvanceInfo> GetListExpenseAdvance(FilterInfo param)
        {
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

            var list = (from er in _context.ExpenseAdvance
                        join d in _context.Document on er.ExpenseAdvanceId equals d.DocumentId
                        join dd in _context.DictionaryDetail on d.StatusId equals dd.DictionaryDetailId
                        from ep in _context.ExpenseProject.Where(p => p.ExpenseProjectId == er.ExpenseProjectId).DefaultIfEmpty()
                        from py in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == er.PaymentMethodId).DefaultIfEmpty()
                        from uA in listA.Where(p => p.UserId == d.CreatedByUserId).DefaultIfEmpty()
                        from uC in listC.Where(p => p.UserId == d.CreatedByUserId).DefaultIfEmpty()
                        where ((d.CreatedByUserId == param.UserId || (uA.UserId != null && d.StatusId <= Validatet) || (uC.UserId != null && d.StatusId <= ApproveSup))
                          &&
                          (param.Id == 0 || er.ExpenseAdvanceId == param.Id) && (param.Date == null || (param.Date <= d.Date && param.DateEnd >= d.Date))) || isAdmin
                        select new Model.DTOs.ExpenseAdvanceInfo
                        {
                            ExpenseAdvance = er,
                            Document = d,
                            StatusName = dd.Name,
                            IsCont = uC.UserId != null,
                            IsSup = uA.UserId != null,
                            ExpenseProject = ep.Name,
                            PaymentMethod = py.Name
                        });

            var result = param.NrRows > 0 ? list.OrderByDescending(p => p.Document.StatusId).ThenByDescending(p => p.Document.Date).Take(param.NrRows) : list.OrderByDescending(p => p.Document.StatusId).ThenByDescending(p => p.Document.Date);

            return result;
        }
    }
}
