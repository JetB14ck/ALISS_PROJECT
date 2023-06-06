using ALISS.Data.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using ALISS.STARS.DTO;
using System.IO;
using Microsoft.JSInterop;

namespace ALISS.Data.D7_StarsMapping
{
    public class ReceiveSampleService
    {
        private IConfiguration Configuration { get; }

        private ApiHelper _apiHelper;

        public ReceiveSampleService(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration["ApiClient:ApiUrl"]);
        }

        #region Receive Sample

        public async Task<List<ReceiveSampleListsDTO>> GetStarsResultListByParamAsync(ReceiveSampleSearchDTO modelSearch)
        {
            List<ReceiveSampleListsDTO> objList = new List<ReceiveSampleListsDTO>();

            var searchJson = JsonSerializer.Serialize(modelSearch);

            objList = await _apiHelper.GetDataListByParamsAsync<ReceiveSampleListsDTO>("receive_sample_api/GetStarsResultList", searchJson);

            return objList;
        }

        public async Task<List<ReceiveSampleListsDTO>> SaveReceiveSampleDataAsync(List<ReceiveSampleListsDTO> models,string format)
        {
            List<ReceiveSampleListsDTO> objList = new List<ReceiveSampleListsDTO>();

            objList = await _apiHelper.PostDataByListModelAsync<ReceiveSampleListsDTO>(string.Format("receive_sample_api/Post_SaveReceiveSampleData/{0}", format), models);

            return objList;
        }

        public async void ExportLogbook(IJSRuntime iJSRuntime, List<ReceiveSampleListsDTO> data, string tempPath)
        {
            try
            {
                var filename = string.Format("ReceiveLogbook_{0}.pdf", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                var _reportPath = Path.Combine(tempPath, DateTime.Today.ToString("yyyyMMdd"));
                var outputfileInfo = new FileInfo(Path.Combine(_reportPath, filename));
                if (!Directory.Exists(outputfileInfo.DirectoryName))
                    Directory.CreateDirectory(outputfileInfo.DirectoryName);
                var response = await _apiHelper.ExportDataBarcodeAsync<List<ReceiveSampleListsDTO>>("exportstars_api/printLogbook", outputfileInfo, data);
                if (response == "OK")
                {
                    byte[] fileBytes;
                    using (FileStream fs = outputfileInfo.OpenRead())
                    {
                        fileBytes = new byte[fs.Length];
                        fs.Read(fileBytes, 0, fileBytes.Length);

                        iJSRuntime.InvokeAsync<ReceiveSampleService>(
                            "previewPDF",
                            Convert.ToBase64String(fileBytes)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void ExportBarcode(IJSRuntime iJSRuntime, List<ReceiveSampleListsDTO> data, string tempPath)
        {
            try
            {
                var filename = string.Format("ReceiveBarcode_{0}.pdf", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                var _reportPath = Path.Combine(tempPath, DateTime.Today.ToString("yyyyMMdd"));
                var outputfileInfo = new FileInfo(Path.Combine(_reportPath, filename));
                if (!Directory.Exists(outputfileInfo.DirectoryName))
                    Directory.CreateDirectory(outputfileInfo.DirectoryName);
                var response = await _apiHelper.ExportDataBarcodeAsync<List<ReceiveSampleListsDTO>>("exportstars_api/printBarcode", outputfileInfo, data);
                if (response == "OK")
                {
                    byte[] fileBytes;
                    using (FileStream fs = outputfileInfo.OpenRead())
                    {
                        fileBytes = new byte[fs.Length];
                        fs.Read(fileBytes, 0, fileBytes.Length);

                        iJSRuntime.InvokeAsync<ReceiveSampleService>(
                            "previewPDF",
                            Convert.ToBase64String(fileBytes)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
