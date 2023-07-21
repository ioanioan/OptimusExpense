
using AspNetCore.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using OptimusExpense.Model.Models;
 
using OptimusExpense.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusExpense.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ReportController : BaseController
    {
        private readonly ILogger<ReportController> _logger;
        
        private readonly IExpenseReportRepository _expenseReportRepository;
        IReportRepository _reportRepository;
        public ReportController(IWebHostEnvironment webHostEnvirnoment, ILogger<ReportController> logger,
          UserManager<AspnetUsers> _userManager,
          IExpenseReportRepository expenseReportRepository,
          IReportRepository reportRepository) : base(_userManager, webHostEnvirnoment)
        {
            _logger = logger;
            _webHostEnvirnoment = webHostEnvirnoment;
            _expenseReportRepository = expenseReportRepository;
            _reportRepository = reportRepository;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }


        [HttpGet("GetExpenseReport/{id}")]
        public IActionResult GetExpenseReport(int id)
        {
            var result = _expenseReportRepository.GetRaportExpenseReport(new Model.DTOs.FilterInfo { Id = id, UserId = GetUserId() });

            var dic = new Dictionary<string, object> { };
            //am pus niste date la vrajeala, dataset e generat din interfata Reports\ReportExpenseRepor.cs
            dic.Add("dataSet", result.Tables[0]);
            return GetReport("ExpenseReport.rdlc", dic);
        }

        

        [HttpGet("GetReports")]
        public IEnumerable<Report> GetReports()
        {
            return _reportRepository.Get();
        }


        [HttpPost("RunReport")]
        public List<Dictionary<String, Object>> RunReport(ReportInfo report)
        {
            return _reportRepository.RunReport(GetUserId(), report);
        }

        [HttpPost("GetReportDetails")]
        public IEnumerable<ReportDetailInfo> GetReportDetails(Report entity)
        {
            return _reportRepository.GetReportDetails(entity);
        }


        private IActionResult GetReport(String report, Dictionary<String,Object> dataSources)
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this._webHostEnvirnoment.ContentRootPath}\\Reports\\{report}";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            LocalReport localReport = new LocalReport(path);
            foreach(var dt in dataSources)
            {
                localReport.AddDataSource(dt.Key, dt.Value);
            }
            
            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimtype);
            return File(result.MainStream, "application/pdf");
        }
    }
}
