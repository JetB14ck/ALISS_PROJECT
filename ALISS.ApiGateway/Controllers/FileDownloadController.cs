using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ALISS.ApiGateway.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class FileDownloadController : ControllerBase
    {

        private readonly ILogger<FileDownloadController> _logger;

        private IConfiguration _configuration;

        public FileDownloadController(ILogger<FileDownloadController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("api/FileDownload/DownloadZip/{basePath}/{filePath}/{fileName}")]
        public IActionResult DownloadZip(string basePath, string filePath, string fileName)
        {
            var dataBasePath = _configuration[basePath];

            var fileFullname = "";

            if (string.IsNullOrEmpty(filePath))
            {
                fileFullname = Path.Combine(dataBasePath, fileName);
            }
            else
            {
                fileFullname = Path.Combine(dataBasePath, filePath, fileName);
            }

            if (System.IO.File.Exists(fileFullname))
            {

                var bytes = System.IO.File.ReadAllBytes(fileFullname);

                return File(bytes, "application/octet-stream", fileFullname);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("api/FileDownload/DownloadExcel/{basePath}/{filePath}/{fileName}")]
        public IActionResult DownloadExcel(string basePath, string filePath, string fileName)
        {
            var dataBasePath = _configuration[basePath];

            var fileFullname = "";

            if (string.IsNullOrEmpty(filePath))
            {
                fileFullname = Path.Combine(dataBasePath, fileName);
            }
            else
            {
                fileFullname = Path.Combine(dataBasePath, filePath, fileName);
            }

            if (System.IO.File.Exists(fileFullname))
            {

                var bytes = System.IO.File.ReadAllBytes(fileFullname);

                return File(bytes, "application/vnd.ms-excel", fileFullname);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("api/FileDownload/DownloadPdf/{basePath}/{filePath}/{fileName}")]
        public IActionResult DownloadPdf(string basePath, string filePath, string fileName)
        {
            var dataBasePath = _configuration[basePath];

            var fileFullname = "";

            if (string.IsNullOrEmpty(filePath))
            {
                fileFullname = Path.Combine(dataBasePath, fileName);
            }
            else
            {
                fileFullname = Path.Combine(dataBasePath, filePath, fileName);
            }

            if (System.IO.File.Exists(fileFullname))
            {

                var bytes = System.IO.File.ReadAllBytes(fileFullname);

                return File(bytes, "application/pdf", fileFullname);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
