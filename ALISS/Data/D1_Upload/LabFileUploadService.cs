using ALISS.Data.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALISS.LabFileUpload.DTO;
using Microsoft.JSInterop;
using OfficeOpenXml;
using System.IO;
using ALISS.UserManagement.DTO;
using ALISS.STARS.Report.DTO;
using Log4NetLibrary;

namespace ALISS.Data.D1_Upload
{
    public class LabFileUploadService
    {
        private IConfiguration Configuration { get; }

        private ApiHelper _apiHelper;

        public LabFileUploadService(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration["ApiClient:ApiUrl"]);
        }

        public async Task<List<LabFileUploadDataDTO>> GetLabFileUploadListByModelAsync(LabFileUploadSearchDTO model)
        {
            List<LabFileUploadDataDTO> objList = new List<LabFileUploadDataDTO>();
            objList = await _apiHelper.GetDataListByModelAsync<LabFileUploadDataDTO, LabFileUploadSearchDTO>("labfileupload_api/Get_LabFileUploadListByModel", model);
            return objList;
        }


        public async Task<LabFileUploadDataDTO> GetLabFileUploadDataAsync(string lfu_Id)
        {
            LabFileUploadDataDTO LabFileUpload = new LabFileUploadDataDTO();

            LabFileUpload = await _apiHelper.GetDataByIdAsync<LabFileUploadDataDTO>("labfileupload_api/GetLabFileUploadDataById", lfu_Id);

            return LabFileUpload;
        }

        public async Task<LabFileUploadDataDTO> SaveLabFileUploadDataAsync(LabFileUploadDataDTO model)
        {
            if (model.lfu_id.Equals(Guid.Empty))
            {
                model.lfu_id = Guid.NewGuid();
                model.lfu_status = 'N';
                model.lfu_flagdelete = false;
            }



            var LabFile = await _apiHelper.PostDataAsync<LabFileUploadDataDTO>("labfileupload_api/Post_SaveLabFileUploadData", model);

            return LabFile;
        }

        public async Task<List<LabFileSummaryHeaderListDTO>> GetLabFileSummaryHeaderListAsync(string lfu_Id)
        {
            List<LabFileSummaryHeaderListDTO> objList = new List<LabFileSummaryHeaderListDTO>();

            objList = await _apiHelper.GetDataListByParamsAsync<LabFileSummaryHeaderListDTO>("labfileupload_api/GetLabFileSummaryHeaderBylfuId", lfu_Id);

            return objList;
        }

        public async Task<List<LabFileSummaryDetailListDTO>> GetLabFileSummaryDetailListAsync(string fsh_id)
        {
            List<LabFileSummaryDetailListDTO> objList = new List<LabFileSummaryDetailListDTO>();

            objList = await _apiHelper.GetDataListByParamsAsync<LabFileSummaryDetailListDTO>("labfileupload_api/GetLabFileSummaryDetailBylfuId", fsh_id);

            return objList;
        }
        public async Task<List<LabFileSummaryDetailListDTO>> GetLabFileSummaryDetailListBylfuIdAsync(string lfu_id)
        {
            List<LabFileSummaryDetailListDTO> objList = new List<LabFileSummaryDetailListDTO>();

            objList = await _apiHelper.GetDataListByParamsAsync<LabFileSummaryDetailListDTO>("labfileupload_api/GetLabFileSummaryDetailListBylfuIdAsync", lfu_id);

            return objList;
        }

        public async Task<List<LabFileErrorHeaderListDTO>> GetLabFileErrorHeaderListAsync(string lfu_Id)
        {
            List<LabFileErrorHeaderListDTO> objList = new List<LabFileErrorHeaderListDTO>();

            objList = await _apiHelper.GetDataListByParamsAsync<LabFileErrorHeaderListDTO>("labfileupload_api/GetLabFileErrorHeaderBylfuId", lfu_Id);

            return objList;
        }

        public async Task<List<LabFileErrorDetailListDTO>> GetLabFileErrorDetailListAsync(string lfu_Id)
        {
            List<LabFileErrorDetailListDTO> objList = new List<LabFileErrorDetailListDTO>();

            objList = await _apiHelper.GetDataListByParamsAsync<LabFileErrorDetailListDTO>("labfileupload_api/GetLabFileErrorDetailBylfuId", lfu_Id);

            return objList;
        }

        public async Task<List<LabFileLabAlertSummaryListDTO>> GetLabFileLabAlertSummaryListAsync(string lfu_Id)
        {
            List<LabFileLabAlertSummaryListDTO> objList = new List<LabFileLabAlertSummaryListDTO>();

            objList = await _apiHelper.GetDataListByParamsAsync<LabFileLabAlertSummaryListDTO>("labfileupload_api/GetLabFileLabAlertSummaryBylfuId", lfu_Id);

            return objList;
        }


        public void GenerateExportSummary(IJSRuntime iJSRuntime, List<LabFileSummaryHeaderListDTO> LabFileSummaryHeaderList,
            List<LabFileErrorDetailListDTO> LabFileErrorDetailList,
            List<LabFileLabAlertSummaryListDTO> LabFileLabAlertSummaryList,
            string LabFileName)
        {
            byte[] fileContents;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("Summary");
                var workSheet2 = package.Workbook.Worksheets.Add("Error");
                var workSheet3 = package.Workbook.Worksheets.Add("LabAlertSummary");
                #region Hearder Row
                workSheet.Cells[1, 1].Value = "Specimen";
                workSheet.Cells[1, 2].Value = "Organism";
                workSheet.Cells[1, 3].Value = "Total";
                #endregion

                int row = 2;
                foreach (LabFileSummaryHeaderListDTO item in LabFileSummaryHeaderList)
                {
                    workSheet.Cells[row, 1].Value = item.fsh_code + " - " + item.fsh_desc;
                    workSheet.Cells[row, 3].Value = item.fsh_total; ;
                    row++;
                    var detail = item.LabFileSummaryDetailLists;

                    if (detail != null)
                    {
                        foreach (LabFileSummaryDetailListDTO d in detail)
                        {
                            workSheet.Cells[row, 2].Value = d.fsd_organismcode + " - " + d.fsd_organismdesc;
                            workSheet.Cells[row, 3].Value = d.fsd_total;

                            row++;
                        }
                    }

                }


                workSheet2.Cells[1, 1].Value = "Message";
                workSheet2.Cells[1, 2].Value = "Local Value";
                workSheet2.Cells[1, 3].Value = "Local Description";


                row = 2;

                foreach (LabFileErrorDetailListDTO err in LabFileErrorDetailList)
                {
                    workSheet2.Cells[row, 1].Value = err.feh_message;
                    workSheet2.Cells[row, 2].Value = err.fed_localvalue;
                    if (!string.IsNullOrEmpty(err.fed_localdescr))
                    {
                        workSheet2.Cells[row, 3].Value = err.fed_localdescr;
                    }
                    row++;
                }


                workSheet3.Cells[1, 1].Value = "ROW_IDX";
                workSheet3.Cells[1, 2].Value = "ALERT_NUM";
                workSheet3.Cells[1, 3].Value = "ALERT_ORG";
                workSheet3.Cells[1, 4].Value = "ALERT_TEXT";
                workSheet3.Cells[1, 5].Value = "ALERT_TOT";
                workSheet3.Cells[1, 6].Value = "PIORITY";
                workSheet3.Cells[1, 7].Value = "QUAL_CONT";
                workSheet3.Cells[1, 8].Value = "IMP_SPECIE";
                workSheet3.Cells[1, 9].Value = "IMP_RESIST";
                workSheet3.Cells[1, 10].Value = "SAVE_ISOL";
                workSheet3.Cells[1, 11].Value = "SEND_REF";
                workSheet3.Cells[1, 12].Value = "INF_CONT";
                workSheet3.Cells[1, 13].Value = "RX_COMMENT";
                workSheet3.Cells[1, 14].Value = "OTHER_AL";


                row = 2;

                foreach (LabFileLabAlertSummaryListDTO labAlert in LabFileLabAlertSummaryList)
                {
                    workSheet3.Cells[row, 1].Value = labAlert.plas_row_idx;
                    workSheet3.Cells[row, 2].Value = labAlert.plas_alert_num;
                    workSheet3.Cells[row, 3].Value = labAlert.plas_alert_org;
                    workSheet3.Cells[row, 4].Value = labAlert.plas_alert_text;
                    workSheet3.Cells[row, 5].Value = labAlert.plas_alert_tot;
                    workSheet3.Cells[row, 6].Value = labAlert.plas_piority;
                    workSheet3.Cells[row, 7].Value = labAlert.plas_qual_cont;
                    workSheet3.Cells[row, 8].Value = labAlert.plas_imp_specie;
                    workSheet3.Cells[row, 9].Value = labAlert.plas_imp_resist;
                    workSheet3.Cells[row, 10].Value = labAlert.plas_save_isol;
                    workSheet3.Cells[row, 11].Value = labAlert.plas_send_ref;
                    workSheet3.Cells[row, 12].Value = labAlert.plas_inf_cont;
                    workSheet3.Cells[row, 13].Value = labAlert.plas_rx_comment;
                    workSheet3.Cells[row, 14].Value = labAlert.plas_other_al;
                    row++;
                }

                //workSheet3.PrinterSettings.FitToPage = true;
                //workSheet3.PrinterSettings.FitToWidth = 1;
                //workSheet3.PrinterSettings.FitToHeight = 1;
                fileContents = package.GetAsByteArray();
            }

            try
            {
                iJSRuntime.InvokeAsync<LabFileUploadService>(
                    "saveAsFile",
                    string.Format("{0}.xlsx", "Summary"),
                    Convert.ToBase64String(fileContents)
                    );

            }
            catch (Exception ex)
            {

            }
        }


        public async void GenerateExportMappingError(IJSRuntime iJSRuntime, string[] lfu_ids, string LabFileName)
        {
            byte[] fileContents;

            List<LabFileExportMappingErrorDTO> objList = new List<LabFileExportMappingErrorDTO>();

            objList = await _apiHelper.PostDataByListAsync<List<LabFileExportMappingErrorDTO>>("labfileupload_api/GetLabFileExportMappingError", lfu_ids);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                int row_no = 2;
                var workSheet = package.Workbook.Worksheets.Add("ExportMappingError");

                #region Hearder Row
                string[] headers = new string[] { "รหัสโรงพยาบาล", "โรงพยาบาล", "เขตสุขภาพ", "File Type", "Program", "File Name", "Field", "Message", "Local Value", "Local Descr", "feh_lfu_id", "lfu_mp_id", "Whonet Code" };
                foreach (var (header, i) in headers.Select((value, i) => (value, i)))
                {
                    workSheet.Cells[1, i + 1].Value = header;
                }
                #endregion

                #region Data Row
                foreach (var row in objList)
                {
                    int col = 0;
                    foreach (var prop in row.GetType().GetProperties())
                    {
                        workSheet.Cells[row_no, col + 1].Value = prop.GetValue(row, null);
                        col++;
                    }
                    row_no++;
                }
                #endregion

                #region workSheet Properties
                workSheet.Row(1).Style.Font.Bold = true;

                workSheet.Column(11).Hidden = true;
                workSheet.Column(12).Hidden = true;

                workSheet.Column(1).Width = 16;
                workSheet.Column(2).Width = 36;
                workSheet.Column(3).Width = 11;
                workSheet.Column(4).Width = 11;
                workSheet.Column(5).Width = 11;
                workSheet.Column(6).Width = 36;
                workSheet.Column(7).Width = 14;
                workSheet.Column(8).Width = 24;
                workSheet.Column(9).Width = 24;
                workSheet.Column(10).Width = 24;
                workSheet.Column(13).Width = 14;
                #endregion

                fileContents = package.GetAsByteArray();
            }

            try
            {
                iJSRuntime.InvokeAsync<LabFileUploadService>(
                    "saveAsFile",
                    string.Format("{0}.xlsx", LabFileName),
                    Convert.ToBase64String(fileContents)
                    );

            }
            catch (Exception ex)
            {

            }
        }

        public async Task<List<LabFileUploadDataDTO>> GenerateBoxNo(List<LabFileUploadDataDTO> data, string format)
        {
            try
            {
                List<LabFileUploadDataDTO> objList = new List<LabFileUploadDataDTO>();

                objList = await _apiHelper.PostDataByListModelAsync<LabFileUploadDataDTO>(string.Format("labfileupload_api/GenerateBoxNo/{0}",format), data);

                return objList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void ExportBoxNoBarcode(IJSRuntime iJSRuntime, LabFileUploadDataDTO data, string tempPath)
        {
            try
            {
                var filename = string.Format("{0}{1}.pdf", data.lfu_BoxNo, DateTime.Now.ToString("HHmmss"));
                var _reportPath = Path.Combine(tempPath, DateTime.Today.ToString("yyyyMMdd"));
                var outputfileInfo = new FileInfo(Path.Combine(_reportPath, filename));
                if (!Directory.Exists(outputfileInfo.DirectoryName))
                    Directory.CreateDirectory(outputfileInfo.DirectoryName);
                //var hosInfo = await _apiHelper.GetDataByIdAsync<HospitalDTO>("hospital_api/Get_Data", data.lfu_hos_code);
                var param = new BoxNoBarcodeDTO() { hos_name = data.lfu_hos_name, arh_name = data.lfu_arh_name, lfu_boxno = data.lfu_BoxNo, send_date = data.lfu_SendDate_str };
                var response = await _apiHelper.ExportDataBarcodeAsync<BoxNoBarcodeDTO>("exportstars_api/ExportBoxNoBarcode", outputfileInfo, param);
                if (response == "OK")
                {
                    byte[] fileBytes;
                    using (FileStream fs = outputfileInfo.OpenRead())
                    {
                        fileBytes = new byte[fs.Length];
                        fs.Read(fileBytes, 0, fileBytes.Length);

                        iJSRuntime.InvokeAsync<LabFileUploadService>(
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
    }
}

