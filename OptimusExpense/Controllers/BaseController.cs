using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OptimusExpense.Model.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OptimusExpense.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController: ControllerBase
    {
        protected readonly UserManager<AspnetUsers> _userManager;
        protected IWebHostEnvironment _webHostEnvirnoment;
        public BaseController(UserManager<AspnetUsers> _userManager, IWebHostEnvironment  webHostEnvirnoment )
        {
            this._userManager = _userManager;
            _webHostEnvirnoment = webHostEnvirnoment;
        }

    

        protected String GetUserId()
        {
             
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
             
            return userId;
        }

        [HttpPost("Upload")]
        public IActionResult Upload(IFormFile file)
        {
            var fileName ="upload\\"+Guid.NewGuid()+ ContentDispositionHeaderValue
          .Parse(file.ContentDisposition)
          .FileName
          .Trim('"');

            var filePath = _webHostEnvirnoment.WebRootPath  +"\\" +fileName;


            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return Ok(fileName);
        }
    }
}
