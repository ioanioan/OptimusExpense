using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Data.Repositories;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusExpense.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController: BaseController
    {

        private readonly ILogger<ConfigurationController> _logger;
        private readonly IUserActionRepository _repUserAction;
        private readonly IAspnetUsersRepository _repAspnetUser;
        private readonly IEmployeeRepository _employeeRepository;



        public ConfigurationController(ILogger<ConfigurationController> logger,
            UserManager<AspnetUsers> _userManager,
        
           IUserActionRepository repUserAction,
           IAspnetUsersRepository repAspnetUser, IWebHostEnvironment webHostEnvirnoment,
           IEmployeeRepository employeeRepository) : base(_userManager, webHostEnvirnoment)
        {
            _logger = logger;
            _repUserAction = repUserAction;
            _repAspnetUser = repAspnetUser;
            this._employeeRepository = employeeRepository;


        }

      

        [HttpGet("GetMenus")]
        public IEnumerable<MenuInfo> GetMenus()
        {
            var userId = GetUserId();
            var menus = _repUserAction.GetMenuByUser(userId);

            return menus;
        }

        [HttpGet("GetUsers")]
        public IEnumerable<AspNetUsersInfo> GetUsers()
        {

            var result = _repAspnetUser.GetUsers();
            return result;
        }

        [HttpGet("GetAspNetRoles")]
        public IEnumerable<AspNetRoles> GetAspNetRoles()
        {

            var result = _repAspnetUser.GetAspNetRoles();
            return result;
        }

        [HttpPost("SaveUser")]
        public async Task<OptimusExpense.Model.DTOs.AspNetUsersInfo> SaveUser(OptimusExpense.Model.DTOs.AspNetUsersInfo entity)
        {
            var  r=await _repAspnetUser.Save(entity);
            return r;
        }

        [HttpPost("ChangePasswordS")]
        public async Task<OptimusExpense.Model.DTOs.AspNetUsersInfo> ChangePasswordS(OptimusExpense.Model.DTOs.AspNetUsersInfo entity)
        {
            entity.AspNetUsers = new AspnetUsers { Id=GetUserId() };
            var r = await _repAspnetUser.ChangePassword(entity);
            return r;
        }


        [HttpPost("SaveMenu")]
        public IEnumerable<MenuInfo> SaveMenu(Model.Models.UserAction action)
        {
            var userId = GetUserId();
            var menus = _repUserAction.GetMenuByUser(userId);

            return menus;
        }


        [HttpPost("GetDateUtilizator")]
        public EmployeeInfo GetDateUtilizator(FilterInfo param)
        {
            param.UserId = GetUserId();
            var result = _employeeRepository.GetEmployeeByUserId(param.UserId);
            return result;
        }
    }
}
