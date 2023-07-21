using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OptimusExpense.Data.Repositories
{
    public class SerialNumberRepository : EntityBaseRepository<SerialNumber>, ISerialNumberRepository
    {
        OptimusExpenseContext _context;
        public SerialNumberRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }
 
        public void SetNumberDocument(Document doc)
        {
            var result = (from s in _context.SerialNumber
                    join e in _context.Person on s.PartnerId equals e.PartnerId
                    join u in _context.AspnetUsers on e.PersonId equals u.EmployeeId
                    where doc.CreatedByUserId==u.Id
                    && doc.DocumentTypeId==s.DocumentTypeId
                    select s).OrderByDescending(p=>p.Number).FirstOrDefault();
            if (result != null)
            {
                var r = new SerialNumber
                {
                    Number = result.Number + 1,
                    DocumentTypeId= result.DocumentTypeId,
                    Series=result.Series,
                    PartnerId=result.PartnerId
                };
                r=base.Save(r);
                doc.SerialNumberId = r.SerialNumberId;
                doc.Number = r.Series + r.Number.ToString().PadLeft(10, '0');
            }
        }
    }
}
