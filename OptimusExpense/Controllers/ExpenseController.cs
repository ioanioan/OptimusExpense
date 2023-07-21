using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OptimusExpense.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ExpenseController : BaseController
    {
        private readonly ILogger<ExpenseController> _logger;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IExpenseReportRepository _expenseReportRepository;
        private readonly IEmailSender _emailSender;
        private readonly IExpenseAdvanceRepository _expenseAdvanceRepository;

        public ExpenseController(ILogger<ExpenseController> logger,
            UserManager<AspnetUsers> _userManager,
            IExpenseRepository expenseRepository,
            IExpenseReportRepository expenseReportRepository, IExpenseAdvanceRepository expenseAdvanceRepository, IEmailSender emailSender, IWebHostEnvironment webHostEnvirnoment
            ) : base(_userManager, webHostEnvirnoment)
        {
            _logger = logger;
            this._expenseRepository = expenseRepository;
            this._expenseReportRepository = expenseReportRepository;
            this._emailSender = emailSender;
            this._expenseAdvanceRepository = expenseAdvanceRepository;
        }

        [HttpPost("GetListExpenseDraft")]
        public IEnumerable<ExpenseInfo> GetListExpenseDraft(FilterInfo dt)
        {
            dt.UserId = GetUserId();
            var result = _expenseRepository.GetListExpenseDraft(dt);

            return result;
        }

        [HttpPost("GetListExpense")]
        public IEnumerable<ExpenseInfo> GetListExpense(FilterInfo dt)
        {
            dt.UserId = GetUserId();
            var result = _expenseRepository.GetListExpense(dt.UserId);

            return result;
        }

        [HttpPost("GetDashBoardExpenseReport")]
        public List<DashboardExpenseReportInfo> GetDashboardExpenseReport(FilterInfo param)
        {
            param.UserId = GetUserId();
            var result = _expenseReportRepository.GetDashboardExpenseReport(param);
            return result;
        }

        [HttpPost("GetListExpenseReport")]
        public IEnumerable<ExpenseReportInfo> GetListExpenseReport(FilterInfo param)
        {
            param.UserId = GetUserId();
            var result = _expenseReportRepository.GetListExpenseReport(param);

            return result;
        }

        [HttpPost("GetListLastTransactions")]
        public IEnumerable<ExpenseInfo> GetListLastTransactions(FilterInfo param)
        {
            param.UserId = GetUserId();

            var result = _expenseRepository.GetListLastExpenses(param);

            return result;
        }

        [HttpGet("GetGraphicExpenses")]
        public dynamic GetGraphicExpenses()
        {
            
            var result = _expenseRepository.GetGraphicExpenses(GetUserId());
            return result;
        }

         

        [HttpPost("SaveExpense")]
        public OptimusExpense.Model.DTOs.ExpenseInfo SaveExpense(OptimusExpense.Model.DTOs.ExpenseInfo entity)
        {
            if (entity.Expense.UserId == null)
            {
                entity.Expense.UserId = GetUserId();
            }

            _expenseRepository.Save(entity.Expense);
            return entity;
        }

        [HttpPost("DeleteExpense")]
        public OptimusExpense.Model.DTOs.ExpenseInfo DeleteExpense(OptimusExpense.Model.DTOs.ExpenseInfo entity)
        {
            _expenseRepository.Remove(entity.Expense);
            return entity;
        }

        [HttpPost("DeleteExpenseReport")]
        public OptimusExpense.Model.DTOs.ExpenseReportInfo DeleteExpenseReport(OptimusExpense.Model.DTOs.ExpenseReportInfo entity)
        {
            _expenseReportRepository.Remove(entity);
            return entity;
        }
        
        [HttpPost("SaveExpenseReport")]
        public OptimusExpense.Model.DTOs.ExpenseReportInfo SaveExpenseReport(OptimusExpense.Model.DTOs.ExpenseReportInfo entity)
        {             
            if (entity.Document == null)
            {
                entity.Document = new Document { };            
            }
            //entity.Document.Date = new DateTime(entity.Document.Date.Value.Year, entity.Document.Date.Value.Month, 1);
            entity.Document.CreatedByUserId = GetUserId();

            _expenseReportRepository.Save(entity);
            return entity;
        }

        [HttpPost("GetListExpenseAdvance")]
        public IEnumerable<ExpenseAdvanceInfo> GetListExpenseAdvance(FilterInfo param)
        {
            param.UserId = GetUserId();
            var result = _expenseAdvanceRepository.GetListExpenseAdvance(param);

            return result;
        }

        [HttpPost("SaveExpenseAdvance")]
        public OptimusExpense.Model.DTOs.ExpenseAdvanceInfo SaveExpenseAdvance(OptimusExpense.Model.DTOs.ExpenseAdvanceInfo entity)
        {
            if (entity.Document == null)
            {
                entity.Document = new Document { };
            }
            entity.Document.CreatedByUserId = GetUserId();

            _expenseAdvanceRepository.Save(entity);
            return entity;
        }

        [HttpPost("DeleteExpenseAdvance")]
        public OptimusExpense.Model.DTOs.ExpenseAdvanceInfo DeleteExpenseAdvance(OptimusExpense.Model.DTOs.ExpenseAdvanceInfo entity)
        {
            _expenseAdvanceRepository.Remove(entity);
            return entity;
        }
    }
}
