using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OptimusExpense.Data.Repositories
{
    public class EmployeeRepository :  EntityBaseRepository<Employee>, IEmployeeRepository
    {

        OptimusExpenseContext _context;
        public EmployeeRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

        public List<EmployeeInfo> GetAllEmployees()
        {
            var result = (from e in _context.Employee
                          join per in _context.Person on e.EmployeeId equals per.PersonId
                          join pp in _context.PartnerPoint on e.PartnerPointId equals pp.PartnerPointId
                          from dd in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == e.StatusId).DefaultIfEmpty()
                          from sp in _context.Person.Where(p => p.PersonId == e.SuperiorEmployeeId).DefaultIfEmpty()
                          from asp in _context.Employee.Where(p => p.EmployeeId == e.SuperiorEmployeeId).DefaultIfEmpty()
                          from ap in _context.Person.Where(p => p.PersonId == e.AccountingEmployeeId).DefaultIfEmpty()
                          from aap in _context.Employee.Where(p => p.EmployeeId == e.AccountingEmployeeId).DefaultIfEmpty()
                          from cc in _context.ExpenseCostCenter.Where(p => p.CostCenterId == e.CostCenterId).DefaultIfEmpty()
                          from sc in _context.DictionaryDetail.Where(p => p.Code == e.SectionCode).DefaultIfEmpty()
                          select new Model.DTOs.EmployeeInfo
                          {
                              Employee = e,
                              PartnerPointName = pp.Name,
                              EmployeeName = per.FirstName + " " + per.LastName,
                              SuperiorName = sp.FirstName + " " + sp.LastName,
                              AccountingName = ap.FirstName + " " + ap.LastName,
                              EmailAprobator = asp.Email,
                              EmailContabil = aap.Email,
                              StatusName = dd.Name,
                              CostCenterName = cc.Name + "(" + cc.Code + ")",
                              SectionName = sc.Name
                          }).OrderBy(p=>p.EmployeeName).ToList();
            return result;
        }
        public EmployeeInfo GetEmployeeByDocumentId(int docId)
        {
            var result = (from d in _context.Document
                          join us in _context.AspnetUsers on d.CreatedByUserId equals us.Id
                          join e in _context.Employee on us.EmployeeId equals e.EmployeeId
                          join per in _context.Person on e.EmployeeId equals per.PersonId
                          join pp in _context.PartnerPoint on e.PartnerPointId equals pp.PartnerPointId
                          from dd in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == e.StatusId).DefaultIfEmpty()
                          from sp in _context.Person.Where(p => p.PersonId == e.SuperiorEmployeeId).DefaultIfEmpty()
                          from asp in _context.Employee.Where(p => p.EmployeeId == e.SuperiorEmployeeId).DefaultIfEmpty()
                          from ap in _context.Person.Where(p => p.PersonId == e.AccountingEmployeeId).DefaultIfEmpty()
                          from aap in _context.Employee.Where(p => p.EmployeeId == e.AccountingEmployeeId).DefaultIfEmpty()
                          from cc in _context.ExpenseCostCenter.Where(p => p.CostCenterId == e.CostCenterId).DefaultIfEmpty()
                          from sc in _context.DictionaryDetail.Where(p => p.Code == e.SectionCode).DefaultIfEmpty()
                          where d.DocumentId == docId
                          select new Model.DTOs.EmployeeInfo
                          {
                              Employee = e,
                              PartnerPointName = pp.Name,
                              EmployeeName = per.FirstName + " " + per.LastName,
                              SuperiorName = sp.FirstName + " " + sp.LastName,
                              AccountingName = ap.FirstName + " " + ap.LastName,
                              EmailAprobator = asp.Email,
                              EmailContabil = aap.Email,
                              StatusName = dd.Name,
                              CostCenterName = cc.Name + "(" + cc.Code + ")",
                              SectionName = sc.Name
                          }).FirstOrDefault();
            return result;
        }

        public EmployeeInfo GetEmployeeByUserId(String  userId)
        {
            var result = (from    us in _context.AspnetUsers  
                          join e in _context.Employee on us.EmployeeId equals e.EmployeeId
                          join per in _context.Person on e.EmployeeId equals per.PersonId
                          join pp in _context.PartnerPoint on e.PartnerPointId equals pp.PartnerPointId
                          from dd in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == e.StatusId).DefaultIfEmpty()
                          from sp in _context.Person.Where(p => p.PersonId == e.SuperiorEmployeeId).DefaultIfEmpty()
                          from asp in _context.Employee.Where(p => p.EmployeeId == e.SuperiorEmployeeId).DefaultIfEmpty()
                          from ap in _context.Person.Where(p => p.PersonId == e.AccountingEmployeeId).DefaultIfEmpty()
                          from aap in _context.Employee.Where(p => p.EmployeeId == e.AccountingEmployeeId).DefaultIfEmpty()
                          from cc in _context.ExpenseCostCenter.Where(p => p.CostCenterId == e.CostCenterId).DefaultIfEmpty()
                          from sc in _context.DictionaryDetail.Where(p => p.Code == e.SectionCode).DefaultIfEmpty()
                          where us.Id==userId
                          select new Model.DTOs.EmployeeInfo
                          {
                              Employee = e,
                              PartnerPointName = pp.Name,
                              EmployeeName = per.FirstName + " " + per.LastName,
                              SuperiorName = sp.FirstName + " " + sp.LastName,
                              AccountingName = ap.FirstName + " " + ap.LastName,
                              EmailAprobator = asp.Email,
                              EmailContabil = aap.Email,
                              StatusName = dd.Name,
                              CostCenterName = cc.Name + "(" + cc.Code + ")",
                              SectionName = sc.Name
                          }).FirstOrDefault();
            return result;
        }
    }
}
