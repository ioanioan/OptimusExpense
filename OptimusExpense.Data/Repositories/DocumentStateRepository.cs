using OptimusExpense.Data.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    public class DocumentStateRepository : EntityBaseRepository<OptimusExpense.Model.Models.DocumentState>, IDocumentStateRepository
    {
        OptimusExpenseContext _context;
        public DocumentStateRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }
    }
}
