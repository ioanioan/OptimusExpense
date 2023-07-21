using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    public class CurrencyRepository :  EntityBaseRepository<Currency>, ICurrencyRepository {

        OptimusExpenseContext _context;
        public CurrencyRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

    }
}
