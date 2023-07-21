using OptimusExpense.Data.Abstract;
using OptimusExpense.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OptimusExpense.Model.Models;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Infrastucture;

namespace OptimusExpense.Data.Repositories
{
    public class UserActionRepository : EntityBaseRepository< UserAction>, IUserActionRepository
    {

        OptimusExpenseContext _context;
        public UserActionRepository(OptimusExpenseContext c) : base(c)
        {
            _context = c;
        }

        public List<MenuInfo> GetMenuByUser(String userId)
        {
            var typeMenu = DictionaryDetailType.Menu.GetHashCode();
            var r = (from l in _context.UserAction
                     join rxl in _context.UserActionXRoles on l.UserActionId equals rxl.UserActionId
                     join rr in _context.AspNetUserRoles on rxl.RoleId equals rr.RoleId
                     from mex in _context.UserXForbiddenUserAction.Where(p => p.UserActionId == l.UserActionId && p.UserId == userId).DefaultIfEmpty()
                     where rr.UserId == userId && l.ParentUserActionId == l.UserActionId
                     && mex.UserId == null
                     && l.Type == typeMenu
                     select new Model.DTOs.MenuInfo
                     {
                         Menu = l,

                         SubMenus = (from l1 in _context.UserAction
                                     join rxl1 in _context.UserActionXRoles on l1.UserActionId equals rxl1.UserActionId
                                     join rr1 in _context.AspNetUserRoles on rxl1.RoleId equals rr1.RoleId
                                     from mex1 in _context.UserXForbiddenUserAction.Where(pp => pp.UserActionId == l1.UserActionId && pp.UserId == userId).DefaultIfEmpty()
                                     where rr1.UserId == userId && l1.ParentUserActionId != l1.UserActionId
                                     && l1.ParentUserActionId == l.UserActionId
                                     && l.Type == typeMenu
                                     select l1).OrderBy(p => p.DisplayOrder).ToList()
                     }).OrderBy(p => p.Menu.DisplayOrder).ThenBy(p => p.Menu.UserActionId).ToList();
            return r;
        }
    }
}
