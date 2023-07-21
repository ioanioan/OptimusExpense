using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    public class LogRepository : EntityBaseRepository<Log>, ILogRepository {  

      OptimusExpenseContext _context;
    public LogRepository(OptimusExpenseContext c) : base(c)
    {
        _context = c;
    }


        public override Log Save(Log entity)
        {
             
            return base.Save(entity);
        }
    }
}
