using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    public class PropertyEntityValueRepository : EntityBaseRepository<PropertyEntityValue>, IPropertyEntityValueRepository
    {

        OptimusExpenseContext _context;
        public PropertyEntityValueRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

    }
}
