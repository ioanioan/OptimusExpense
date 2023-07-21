using Microsoft.AspNetCore.Identity;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Infrastucture.Exception;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimusExpense.Data.Repositories
{
    public class AspnetUsersRepository: EntityBaseRepository<AspnetUsers>, IAspnetUsersRepository
    {

        OptimusExpenseContext _context;
        UserManager<AspnetUsers> _userManager;
        public AspnetUsersRepository(OptimusExpenseContext c, UserManager<AspnetUsers> userManager) : base(c)
        {
            _context = c;
            _userManager = userManager;
        }

        public async Task<AspNetUsersInfo> Save(AspNetUsersInfo entity)
        {

            if (entity.ValidatePassword())
            {
                var exis = _context.AspnetUsers.Where(p => p.UserName == entity.AspNetUsers.UserName && p.Id != entity.AspNetUsers.Id).FirstOrDefault();
                if (exis != null)
                {
                    throw new HttpResponseException { Value= "Exista utilizatorul " + exis.UserName + "!" };
                }
                var u = _userManager.FindByIdAsync(entity.AspNetUsers.Id).Result;
                if (u != null)
                {
                    u.EmployeeId = entity.AspNetUsers.EmployeeId;
                    u.Active = entity.AspNetUsers.Active;
                    entity.AspNetUsers = u;
                    var uu =  await _userManager.UpdateAsync(u);
                    if (!uu.Succeeded)
                    {
                        throw new HttpResponseException { Value = "Eroare la salvare utilizator! " + string.Join(",", uu.Errors.Select(p => p.Description)) };
                    }
                    await ResetPassword(entity);
                }
                else
                {
                    entity.AspNetUsers.CreatedDate = DateTime.Now;
                    IdentityResult re = await _userManager.CreateAsync(entity.AspNetUsers, entity.Password);
                    if (!re.Succeeded)
                    {
                        throw new HttpResponseException { Value = "Eroare la salvare utilizator! " + string.Join(",", re.Errors.Select(p => p.Description)) };
                    }
                }
                SaveRole(entity);
            }
            return entity;
        }

        public async Task<AspNetUsersInfo> ChangePassword(AspNetUsersInfo entity)
        {
            if (entity.ValidatePassword())
            {
                var u = _userManager.FindByIdAsync(entity.AspNetUsers.Id).Result;
                IdentityResult result =await _userManager.ChangePasswordAsync(u, entity.OldPassword,entity.Password);
                if (!result.Succeeded)
                {
                    throw new HttpResponseException { Value = "Eroare la schimbare parola! " + string.Join(",", result.Errors.Select(p => p.Description)) };
                }
                else
                {
                    u.LastLogin = DateTime.Now;
                    await _userManager.UpdateAsync(u);
                }
            }
            return entity;
        }
        private void SaveRole(AspNetUsersInfo entity)
        {
            AspNetUserRoles rr = new AspNetUserRoles { RoleId = entity.RoleId, UserId = entity.AspNetUsers.Id };
            var r = _context.AspNetUserRoles.FirstOrDefault(p => p.UserId == entity.AspNetUsers.Id);
            if (r == null||r.RoleId!=rr.RoleId)
            {
                _context.AspNetUserRoles.Add(rr);
                if (r != null)
                {
                    _context.AspNetUserRoles.Remove(r);
                }
                _context.SaveChanges();
            }
        }
        private async Task ResetPassword(AspNetUsersInfo entity)
        {
            if (entity.Password != null && entity.Password != "")
            {
                var t = _userManager.GeneratePasswordResetTokenAsync(entity.AspNetUsers).Result;
                var re=await _userManager.ResetPasswordAsync(entity.AspNetUsers, t, entity.Password);
                if (!re.Succeeded)
                {
                    throw new HttpResponseException { Value = "Eroare la salvare utilizator! " + string.Join(",", re.Errors.Select(p => p.Description)) };
                }
            }
        }

        public IQueryable<AspNetRoles> GetAspNetRoles()
        {
            return _context.AspNetRoles;
        }

        public IQueryable<AspNetUsersInfo> GetUsers()
        {
            var result = (from u in _context.AspnetUsers
                          join ur in _context.AspNetUserRoles on u.Id equals ur.UserId
                          join r in _context.AspNetRoles on ur.RoleId equals r.Id
                          from p in _context.Person.Where(p => p.PersonId == u.EmployeeId).DefaultIfEmpty()
                          select new AspNetUsersInfo
                          {
                              AspNetUsers=u,
                              RoleId=ur.RoleId,
                              RoleName=r.Name,
                              Employee=p.FirstName+" "+p.LastName
                          });
            return result;
            

        }


        public IQueryable<String> GetSubordination(String userId)
        {
            var listC = (from aa in _context.AspnetUsers
                         from e in _context.Employee.Where(p=>p.SuperiorEmployeeId==aa.EmployeeId|| p.AccountingEmployeeId == aa.EmployeeId)// on aa.EmployeeId equals e.AccountingEmployeeId
                         join u in _context.AspnetUsers on e.EmployeeId equals u.EmployeeId
                         where aa.Id == userId
                         select u.Id);
            return listC;

        }


    }
}
