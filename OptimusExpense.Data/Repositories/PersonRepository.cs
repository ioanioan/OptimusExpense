using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OptimusExpense.Data.Repositories
{
    public class PersonRepository :  EntityBaseRepository<Person>, IPersonRepository
    {

        OptimusExpenseContext _context;
        public PersonRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

        public List<PersonInfo> GetAllPersons()
        {
            var result = (from per in _context.Person
                          from pp in _context.Partner.Where(p => p.PartnerId == per.PartnerId).DefaultIfEmpty()
                          from dd in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == per.PositionId).DefaultIfEmpty()
                          select new Model.DTOs.PersonInfo
                          {
                              Person = per,
                              PartnerName = pp.Name,
                              PositionName = dd.Name,
                          }).OrderBy(p=>p.Person.FirstName + " " + p.Person.LastName).ToList();
            return result;
        }
    }
}
