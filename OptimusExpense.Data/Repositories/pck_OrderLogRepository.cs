using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    internal class pck_OrderLogRepository: EntityBaseRepository<Model.Models.pck_OrderLog>, Ipck_OrderLogRepository
    {

        OptimusExpenseContext _context;
        public pck_OrderLogRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

        public List<Model.Models.pck_OrderLog> SaveOrder(pck_OrderViewInfo entity, String userId)
        {
            var list=(from c in entity.ListCarts
                     from t in entity.ListTasks
                     select new Model.Models.pck_OrderLog
                     {
                         TaskName = t.pck_TaskView.TaskName,
                         TaskCode=t.pck_TaskView.TaskName,
                         UserId=userId,
                         CarriageNumber=c.pck_CartView.CarriageNumber??0,
                         InternalTime=DateTime.Now,
                         OrderNumber=t.pck_OrderView.OrderNumber,

                     }).ToList();
            foreach(var item in list)
            {
                base.Save(item);
            }
            return list;
        }
    
    }
}
