using Microsoft.EntityFrameworkCore;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    public abstract class EntityBaseRepository<T> : EntityBaseRepositoryA<T, OptimusExpenseContext>
        where T : class, IEntityBase, new()
    {
       public EntityBaseRepository(OptimusExpenseContext c) : base(c)
        {

        }
    }
}
