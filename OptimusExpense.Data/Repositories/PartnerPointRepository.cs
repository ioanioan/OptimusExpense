using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OptimusExpense.Data.Repositories
{
    public class PartnerPointRepository :  EntityBaseRepository<PartnerPoint>, IPartnerPointRepository
    {

        OptimusExpenseContext _context;
        public PartnerPointRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

        public List<PartnerPoint> GetPartnerPointsByPersonId(int personId)
        {
            var result = (from per in _context.Person
                          join pp in _context.PartnerPoint on per.PartnerId equals pp.PartnerId
                          where per.PersonId == personId
                          select pp).OrderBy(p=>p.Name).ToList();
            return result;
        }   
        
        public IQueryable<PartnerPointInfo> GetPartnerPointsByPartnerId(int partnerId)
        {
            var result = (from p in _context.PartnerPoint
                          where p.PartnerId == partnerId
                          select new PartnerPointInfo
                          {
                              PartnerPoint = p
                          });
            return result;
        }
    }
}
