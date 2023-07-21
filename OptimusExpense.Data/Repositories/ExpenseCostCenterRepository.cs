using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    public class ExpenseCostCenterRepository :  EntityBaseRepository<ExpenseCostCenter>, IExpenseCostCenterRepository {

        OptimusExpenseContext _context;
        public ExpenseCostCenterRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

        public IQueryable<ExpenseCostCenterInfo> GetAllExpenseCostCenter()
        {
            var result = (from en in _context.ExpenseCostCenter
                          select new Model.DTOs.ExpenseCostCenterInfo
                          {
                              ExpenseCostCenter = en
                          }).OrderBy(p => p.ExpenseCostCenter.Name);
            return result;
        }

        public ExpenseCostCenterInfo GetExpenseCostCenterById(int id)
        {
            var result = (from en in _context.ExpenseCostCenter
                          where en.CostCenterId == id
                          select new Model.DTOs.ExpenseCostCenterInfo
                          {
                              ExpenseCostCenter = en
                          }).OrderBy(p => p.ExpenseCostCenter.Name).FirstOrDefault();
            return result;
        }
    }
}
