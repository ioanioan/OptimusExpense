using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OptimusExpense.Data.Repositories
{
    public class PartnerRepository :  EntityBaseRepository<Partner>, IPartnerRepository
    {

        OptimusExpenseContext _context;
        public PartnerRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

        public List<PartnerInfo> GetAllPartners()
        {
            var result = (from p in _context.Partner
                          join dd in _context.DictionaryDetail on p.StatusId equals dd.DictionaryDetailId
                          join pt in _context.DictionaryDetail on p.PartnerTypeId equals pt.DictionaryDetailId
                          join c in _context.Partner on p.CompanyId equals c.PartnerId
                          select new Model.DTOs.PartnerInfo
                          {
                              Partner = p,
                              PartnerStatus = dd.Name,
                              PartnerType = pt.Name,
                              Company = c.Name
                          }).OrderBy(p=>p.Partner.Name).ToList();
            return result;
        }

        public PartnerInfo GetCompanyByUserId(String userId)
        {
            var result = (from u in _context.AspnetUsers
                          join per in _context.Person on u.EmployeeId equals per.PersonId
                          join pp in _context.Partner on per.PartnerId equals pp.PartnerId
                          join dd in _context.DictionaryDetail on pp.StatusId equals dd.DictionaryDetailId
                          join pt in _context.DictionaryDetail on pp.PartnerTypeId equals pt.DictionaryDetailId
                          join c in _context.Partner on pp.CompanyId equals c.PartnerId
                          where u.Id == userId
                          select new Model.DTOs.PartnerInfo
                          {
                              Partner = pp,
                              PartnerStatus = dd.Name,
                              PartnerType = pt.Name,
                              LastLogin =u.LastLogin,
                              Company = c.Name
                          }).FirstOrDefault();
            return result;
        }
    }
}
