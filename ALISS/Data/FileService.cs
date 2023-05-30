using ALISS.Data.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ALISS.Data
{
    public class FileService
    {
        private IConfiguration _configuration { get; }

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<byte[]> GetFileZipDownloadAsync(string basePath, string filePath, string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                var url = $"{_configuration[basePath]}/api/FileDownload/DownloadZip/{basePath}/{filePath}/{fileName}";
                var file = await client.GetAsync(url);
                return await file.Content.ReadAsByteArrayAsync();
            }
        }

        public async Task<byte[]> GetFileExcelDownloadAsync(string basePath, string filePath, string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                var url = $"{_configuration[basePath]}/api/FileDownload/DownloadExcel/{basePath}/{filePath}/{fileName}";
                var file = await client.GetAsync(url);
                return await file.Content.ReadAsByteArrayAsync();
            }
        }

        public async Task<byte[]> GetFilePdfDownloadAsync(string basePath, string filePath, string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                var url = $"{_configuration[basePath]}/api/FileDownload/DownloadPdf/{basePath}/{filePath}/{fileName}";
                var file = await client.GetAsync(url);
                return await file.Content.ReadAsByteArrayAsync();
            }
        }

        public async Task<byte[]> GetHISExportDownloadAsync(string searchModel)
        {
            using (HttpClient client = new HttpClient())
            {
                var url = $"{_configuration["ApiClient:ApiUrl"]}/his_api/ExportLabDataWithHISFile/{searchModel}";
                var file = await client.GetAsync(url);
                return await file.Content.ReadAsByteArrayAsync();
            }
        }

    }
}
