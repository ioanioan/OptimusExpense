using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    public class ExpenseNatureRepository :  EntityBaseRepository<ExpenseNature>, IExpenseNatureRepository {

        OptimusExpenseContext _context;
        public ExpenseNatureRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

        public IQueryable<ExpenseNatureInfo> GetAllExpenseNature()
        {
            var result = (from en in _context.ExpenseNature
                          from dd in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == en.VatId).DefaultIfEmpty()
                          select new Model.DTOs.ExpenseNatureInfo
                          {
                              ExpenseNature = en,
                              Vat = dd.Name
                          }).OrderBy(p => p.ExpenseNature.Name);
            return result;
        }

        public ExpenseNatureInfo GetExpenseNatureById(int idExp)
        {
            var result = (from en in _context.ExpenseNature
                          from dd in _context.DictionaryDetail.Where(p => p.DictionaryDetailId == en.VatId).DefaultIfEmpty()
                          where en.ExpenseNatureId == idExp
                          select new Model.DTOs.ExpenseNatureInfo
                          {
                              ExpenseNature = en,
                              Vat = dd.Name
                          }).OrderBy(p => p.ExpenseNature.Name).FirstOrDefault();
            return result;
        }
    }
}
