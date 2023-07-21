using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    public class ExpenseProjectRepository : EntityBaseRepository<ExpenseProject>, IExpenseProjectRepository
    {

        OptimusExpenseContext _context;
        public ExpenseProjectRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

    }
}
