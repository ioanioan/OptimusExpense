using Microsoft.EntityFrameworkCore;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptimusExpense.Data.Repositories
{
    public class pck_OrderViewRepository: Ipck_OrderViewRepository
    {
        OptimusExpenseContext _context;
        public pck_OrderViewRepository(OptimusExpenseContext context)
        {
            _context = context;
        }
        public IQueryable<pck_OrderViewInfo> GetActiveOrders(String userId)
        {
            var orders = (from u in _context.AspnetUsers
                          join e in _context.Employee on u.EmployeeId equals e.EmployeeId
                          join pc in _context.pck_OrderView on e.SectionCode equals pc.SectionCode
                          where u.Id == userId
                          && pc.OrderStatus == 1
                          select new pck_OrderViewInfo
                          {
                              OrderNumber = pc.OrderNumber
                          }).Distinct();
            return orders;
        }

        public IQueryable<pck_TaskViewInfo> GetTasksByOrder(String orderNumber)
        {
            var orderLogs = (from l in _context.pck_OrderLog
                             where l.OrderNumber == orderNumber
                             group l by   l.TaskName into g
                             select new
                             {
                                 TaskName=g.Key,
                                 Date=g.Max(p=>p.InternalTime)
                             });
            var orders = (from o in _context.pck_OrderView
                          join t in _context.pck_TaskView on o.TaskCode equals t.TaskCode
                          from l in orderLogs.Where(p=>p.TaskName==t.TaskName).DefaultIfEmpty()
                          where o.OrderNumber == orderNumber
                          && t.TaskName!="NULL"
                          select new pck_TaskViewInfo
                          {
                              pck_TaskView = t,
                              pck_OrderView = o,
                              Status=l.TaskName!=null?1:0
                          }).Distinct().OrderBy(p=>p.Status).ThenBy (p => p.pck_OrderView.TaskOrder);
            return orders;
        }

        public IQueryable<pck_CartViewInfo> GetCartsByOrder(String orderNumber)
        {
            var orderLogs = (from l in _context.pck_OrderLog
                             where l.OrderNumber == orderNumber
                             group l by l.CarriageNumber into g
                             select new
                             {
                                 CarriageNumber = g.Key,
                                 Date = g.Max(p => p.InternalTime),
                                 
                             });
            var orders = (from t in _context.pck_CartView
                          from l in orderLogs.Where(p => p.CarriageNumber == t.CarriageNumber).DefaultIfEmpty()
                          where t.OrderNumber == orderNumber

                          select new pck_CartViewInfo
                          {
                              pck_CartView = t,
                              Status = l.CarriageNumber != null ? 1 : 0
                          }).OrderBy(p=>p.Status).ThenBy(p => p.pck_CartView.CarriageNumber);
            return orders;
        }
    }
}
