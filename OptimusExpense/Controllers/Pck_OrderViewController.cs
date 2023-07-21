using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
using Microsoft.AspNetCore.Hosting.Server;
using OptimusExpense.Data.Repositories;
using System.Composition;
using System.Collections.Generic;

namespace OptimusExpense.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
   
    public class Pck_OrderViewController:BaseController
    {
        Ipck_OrderViewRepository _ipck_OrderViewRepository;
        Ipck_OrderLogRepository _ipck_OrderLogRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<Pck_OrderViewController> _logger;
        public Pck_OrderViewController(IWebHostEnvironment webHostEnvirnoment, ILogger<Pck_OrderViewController> logger,
         UserManager<AspnetUsers> _userManager,

         Ipck_OrderViewRepository ipck_OrderViewRepository, Ipck_OrderLogRepository ipck_OrderLogRepository, ILogRepository logRepository) : base(_userManager, webHostEnvirnoment)
        {
            _logger = logger;
            _webHostEnvirnoment = webHostEnvirnoment;
            _ipck_OrderViewRepository = ipck_OrderViewRepository;
            _logRepository = logRepository;
            _ipck_OrderLogRepository = ipck_OrderLogRepository;
        }

        [HttpGet("GetActiveOrders")]
        public IEnumerable< OptimusExpense.Model.DTOs.pck_OrderViewInfo> GetActiveOrders()
        {
            var result = _ipck_OrderViewRepository.GetActiveOrders(GetUserId());
            return result;
        }
        [HttpPost("GetTasksByOrder")]
        public IEnumerable<OptimusExpense.Model.DTOs.pck_TaskViewInfo> GetTasksByOrder(FilterInfo filter)
        {
            var result = _ipck_OrderViewRepository.GetTasksByOrder(filter.Value);
            return result;
        }
        [HttpPost("GetCartsByOrder")]
        public IEnumerable<OptimusExpense.Model.DTOs.pck_CartViewInfo> GetCartsByOrder(FilterInfo filter)
        {
            var result = _ipck_OrderViewRepository.GetCartsByOrder(filter.Value);
            return result;
        }


        [HttpPost("SaveOrder")]
        public OptimusExpense.Model.DTOs.pck_OrderViewInfo SaveOrder(OptimusExpense.Model.DTOs.pck_OrderViewInfo  entity)
        {
            var list=_ipck_OrderLogRepository.SaveOrder(entity, GetUserId());
            return entity;
        }
    }
}
