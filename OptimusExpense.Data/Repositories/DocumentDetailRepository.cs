using OptimusExpense.Data.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    public class DocumentDetailRepository : EntityBaseRepository<OptimusExpense.Model.Models.DocumentDetail>, IDocumentDetailRepository
    {
        OptimusExpenseContext _context;
        public DocumentDetailRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }


    }
}
