using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OptimusExpense.Model.Models;
 
using   Microsoft.AspNetCore.Mvc;
using OptimusExpense.Certificate;
using Microsoft.AspNetCore.Authorization;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Model.DTOs;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Xml;
using System.Text;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.StaticFiles;
using System.Threading;

namespace OptimusExpense.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [ActionFilter]

    public class EFacturaUploadController:   BaseController
    {
        private readonly ILogger<EFacturaUploadController> _logger;
        private readonly IEF_RaportareRepository _eF_RaportareRepository;
        private readonly ILogRepository _logRepository;
        private string _server;
        private string _port;
        IConfiguration _configuration;
        public EFacturaUploadController(IWebHostEnvironment webHostEnvirnoment, ILogger<EFacturaUploadController> logger,
         UserManager<AspnetUsers> _userManager,
         IConfiguration configuration,
         IEF_RaportareRepository eF_RaportareRepository, ILogRepository logRepository) : base(_userManager, webHostEnvirnoment)
        {
            _logger = logger;
            _webHostEnvirnoment = webHostEnvirnoment;
             _eF_RaportareRepository = eF_RaportareRepository;
            _logRepository = logRepository;
            _configuration =configuration;
            _server = "" + _configuration["EFacturaServer"];
            _port = "" + _configuration["EFacturaPort"];
        }



        [HttpGet("Get")]
        public IActionResult Get()
        {
              return Ok("It works!");

            
        }



        [HttpPost("GetEF_Raportare")]
        public IActionResult GetEF_Raportare(FilterInfo param)
        {
            param.UserId = GetUserId();
            //  _eF_RaportareRepository.UpdateDataFactura();
            var result = _eF_RaportareRepository.GetRaportari(param).Select(p=>new { EF_Raportare=p.EF_Raportare, User=p.User });  ;
               return Ok(result); ;
        }

        [HttpPost("UploadNewFacturi")]
        public IActionResult UploadNewFacturi(FilterInfo param)
        {
            _logRepository.Save(new Log { Action = "Incarca facturi", Value = "manual", Date = System.DateTime.Now, UserId = GetUserId() });

            param.UserId = GetUserId();
            _eF_RaportareRepository.UplodNewFacturi(param);
            return Ok(new {OK="OK" });
        }


        [HttpPost("UploadEFactura")]
        public IActionResult UploadEFactura(FilterInfo param)
        {
            _logRepository.Save(new Log { Action = "Incarca o factura", Value = "manual", Date = System.DateTime.Now, UserId = GetUserId() });

            var eFacturaXMl=System.IO.File.ReadAllText(  _webHostEnvirnoment.WebRootPath + "\\" + param.Value);
            var r=_eF_RaportareRepository.UploadEFactura(GetUserId(),null, eFacturaXMl);
            return Ok(r);
        }


        [HttpPost("RemoveEFactura")]
        public IActionResult RemoveEFactura(OptimusExpense.Model.DTOs.EF_RaportareInfo entity)
        {
            _logRepository.Save(new Log { Action = "Sterge factura", Value="manual", Date=System.DateTime.Now, UserId=GetUserId()});
             
            var r = _eF_RaportareRepository.Remove(entity);
            return Ok(r);
        }

        [HttpGet("DownloadXml/{idEFRaportare}")]
        public IActionResult DownloadXml(int idEFRaportare)
        {
            _logRepository.Save(new Log { Action = "Descarca XML", Value = "manual", Date = System.DateTime.Now, UserId = GetUserId() });

            var stringXml = _eF_RaportareRepository.GetRaportari(new FilterInfo { }).Where(p => p.EF_Raportare.IdEFRaportare == idEFRaportare).First().Xml;

            new FileExtensionContentTypeProvider()
              .TryGetContentType(idEFRaportare+".xml", out string contentType);
            var bytes = Encoding.UTF8.GetBytes(stringXml ?? "");
            return File(bytes, contentType, idEFRaportare + ".xml");
            
        }


        [HttpGet("DescarcaEFactura/{idEFRaportare}")]
        public IActionResult DescarcaEFactura(int idEFRaportare)
        {
            _logRepository.Save(new Log { Action = "Descarca raspuns", Value = "manual", Date = System.DateTime.Now, UserId = GetUserId() });

            var bytes = _eF_RaportareRepository.DownLoadEFactura(_server,_port,idEFRaportare);

            new FileExtensionContentTypeProvider()
              .TryGetContentType(idEFRaportare + ".zip", out string contentType);
            
            return File(bytes, contentType, idEFRaportare + ".zip");

        }


        [HttpPost("SendEFactura")]
        public IActionResult SendEFactura(OptimusExpense.Model.DTOs.EF_RaportareInfo entity)
        {
            _logRepository.Save(new Log { Action = "Trimite", Value = "manual", Date = System.DateTime.Now, UserId = GetUserId() });

            var result = _eF_RaportareRepository.SendEFactura(_server,_port, entity.EF_Raportare.IdEFRaportare);
            return Ok(result);
        }

        [HttpPost("SendEFacturi")]
        public IActionResult SendEFacturi(List<int> list)
        {
            _logRepository.Save(new Log { Action = "Trimite facturi", Value = "manual", Date = System.DateTime.Now, UserId = GetUserId() });

            EF_RaportareInfo result = null;
           
            foreach (var id in list)
            {

                result= _eF_RaportareRepository.SendEFactura(_server, _port,  id);
                
            }
            return Ok(result);
        }

        [HttpPost("GetStareEFactura")]
        public IActionResult GetStareEFactura(OptimusExpense.Model.DTOs.EF_RaportareInfo entity)
        {
            _logRepository.Save(new Log { Action = "Refresh", Value = "manual", Date = System.DateTime.Now, UserId = GetUserId() });

            var result = _eF_RaportareRepository.GetStareEFactura(_server, _port, entity  );
            return Ok(result);
        }

        [HttpPost("GetStariEFacturi")]
        public IActionResult GetStariEFacturi(FilterInfo param)
        {
            _logRepository.Save(new Log { Action = "Preluare stari", Value = "manual", Date = System.DateTime.Now, UserId = GetUserId() });

            var result = _eF_RaportareRepository.GetStariEFacturi(_server, _port, param);
            return Ok(result);
        }
    }
}
