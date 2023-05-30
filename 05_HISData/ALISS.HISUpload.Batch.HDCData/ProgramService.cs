using ALISS.HISUpload.Batch.HDCData.DataAcess;
using ALISS.HISUpload.Batch.HDCData.DataRepos;
using ALISS.HISUpload.Batch.HDCData.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EFCore.BulkExtensions;

namespace ALISS.HISUpload.Batch.HDCData
{
    public class ProgramService : IProgramService
    {
        private readonly ILogger<ProgramService> _log;
        private readonly IConfiguration _config;
        private readonly ITRSTGHISFileUploadHeaderRepo _dataRepo;
        private readonly ITRSTGHISFileUploadDetailRepo _detailRepo;

        private HttpClient _client;
        private string HDC_Token;

        Dictionary<string, string> dataFields = new Dictionary<string, string>()
            {
                {"huh_hos_code" , ""},
                {"huh_hn_no" , ""},
                {"HospitalCode" , "HospitalCode"},
                {"IDCard" , "Identification number"},
                {"HN" , ""},
                {"AN" , "AN"},
                {"FirstName" , "First name"},
                {"LastName" , "Last name"},
                {"Gender" , "Gender"},
                {"DOB" , "Date of birth"},
                {"Nationality" , "Nationality"},
                {"Address" , ""},
                {"Subdistrict" , ""},
                {"District" , ""},
                {"Province" , ""},
                {"Occupational" , ""},
                {"VisitDate" , "Spec_Date"},
                {"AdmissionDate" , "AdmissionDate"},
                {"ReferStatus" , "ReferStatus"},
                {"Department" , ""},
                {"FirstDiagnosis" , ""},
                {"DischargeStatus" , ""},
                {"DischargeDiagnosis" , ""},
                {"DischargeDate" , ""}
            };

        public ProgramService(ILogger<ProgramService> log, IConfiguration config, ITRSTGHISFileUploadHeaderRepo dataRepo, ITRSTGHISFileUploadDetailRepo detailRepo)
        {
            _log = log;
            _config = config;
            _dataRepo = dataRepo;
            _detailRepo = detailRepo;
        }

        public async Task BATCH_HDCData_RUN()
        {
            //for (int i = 0; i < _config.GetValue<int>("LoopTimes"); i++)
            //{
            //    _log.LogInformation("Run number {runNumber} ", i);
            //}

            WebApiRepositories();

            //backup
            //await Get_HDCToken();

            await Get_HDCDataList();

            _log.LogInformation("BATCH_HDCData_RUN : Success");

            if (_config["HDCData_RUN_BATCH:HDCData_DEV_ENV"] == "Y")
            {
                Console.ReadLine();
            }
        }

        private void WebApiRepositories()
        {
            //specify to use TLS 1.2 as default connection
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            _client = new HttpClient(clientHandler);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.BaseAddress = new Uri("https://10.19.4.25/");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "BATCH_HDCData_RUN");

            _log.LogInformation("WebApiRepositories : Success");
        }

        private async Task Get_HDCToken()
        {
            var parameters = new Dictionary<string, string> { { "username", "NIH_EXC" }, { "password", "8gcTXi4aISIEZY" } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            try
            {
                var response = await _client.PostAsync("/ecd/nih/token", encodedContent).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                HDC_Token = await response.Content.ReadAsStringAsync();

                _log.LogInformation("HDC_Token : " + HDC_Token);
            }
            catch (Exception ex)
            {
                _log.LogError("Error : " + ex.Message);
            }

            _log.LogInformation("Get_HDCToken : Success");
        }

        private async Task Get_HDCDataList()
        {
            var TRSTGHISFileUploadHeadersList = new List<TRSTGHISFileUploadHeader>();

            if (_config["HDCData_RUN_BATCH:HDCData_RUN_BATCH_TYPE"] == "A")
            {
                TRSTGHISFileUploadHeadersList = _dataRepo.Get_DataList_Auto();
            }
            else if (_config["HDCData_RUN_BATCH:HDCData_RUN_BATCH_TYPE"] == "M")
            {
                var hos_code = _config["HDCData_RUN_BATCH:HOS_CODE"];

                TRSTGHISFileUploadHeadersList = _dataRepo.Get_DataList_Manual(hos_code);
            }

            foreach (var item in TRSTGHISFileUploadHeadersList.Select(x => x.huh_hos_code).Distinct())
            {
                var dataList = new List<string>();

                var dataItem = TRSTGHISFileUploadHeadersList.Where(x => x.huh_hos_code == item).Select(x => x.huh_hos_code.Substring(2, 5) + "|" + ($"000000000000000{x.huh_hn_no}").Substring(x.huh_hn_no.Length, 15)).Distinct();

                var dataContent = string.Join(',', dataItem);

                _log.LogInformation("========================================");
                _log.LogInformation("hos_code : " + item);
                _log.LogInformation("dataContent : " + dataContent);

                HttpContent content = new StringContent(dataContent);

                //backup
                //if (string.IsNullOrEmpty(HDC_Token) == false)

                //if (true)
                {
                    try
                    {
                        _client.DefaultRequestHeaders.Add("Token", "Bearer " + HDC_Token);

                        //backup
                        //var response = await _client.PostAsync("/ecd/nih/hospcode/hn", content).ConfigureAwait(false);

                        //backup
                        //response.EnsureSuccessStatusCode();

                        //backup
                        //var responseContent = await response.Content.ReadAsStreamAsync();

                        #region Test load JSON file
                        //var response = await loadJsonData();
                        ////var responseContent = await Encoding.UTF8.GetString(response);
                        //Stream responseContent = new MemoryStream(response);
                        ////var xx = Encoding.UTF8.GetString(response);
                        //var dataReturn = await JsonSerializer.DeserializeAsync<List<TRSTGHDCData>>(responseContent);

                        var response = await loadJsonData();
                        Stream responseContent = new MemoryStream(response);
                        var dataReturn = await JsonSerializer.DeserializeAsync<List<TRSTGHDCData>>(responseContent);

                        #endregion

                        //backup
                        //var dataReturn = await JsonSerializer.DeserializeAsync<List<TRSTGHDCData>>(responseContent);

                        //Insert**
                        Add_TRSTGHDCData_Data(dataReturn, item, TRSTGHISFileUploadHeadersList);

                        //ticket = responseContent;

                        //backup
                        ////_log.LogInformation("JsonSerializer.Serialize : " + JsonSerializer.Serialize(dataReturn));

                    }
                    catch (Exception ex)
                    {
                        _log.LogError("Error : " + ex.Message);
                    }
                }
            }

            _log.LogInformation("Get_HDCDataList : Success");
        }

        private void Add_TRSTGHDCData_Data(List<TRSTGHDCData> tRSTGHDCDatas, string huh_hos_code, List<TRSTGHISFileUploadHeader> TRSTGHISFileUploadHeadersList)
        {
            var currentDateTime = DateTime.Now;
            List<TRSTGHISFileUploadDetail> tRSTGHISFileUploadDetailAddList = new List<TRSTGHISFileUploadDetail>();
            List<TRSTGHDCData> tRSTGHDCDataAddList = new List<TRSTGHDCData>();
            List<TRHISFileUpload> tRHISFileUploadUpdateList = new List<TRHISFileUpload>();
            List<int> hfu_ids = new List<int>();

            List<TRSTGHISFileUploadDetail> TRSTGHISFileUploadDetailList = _detailRepo.getTRSTGHISFileUploadDetailListByHosCode(huh_hos_code);

            foreach (TRSTGHDCData tRSTGHDCData in tRSTGHDCDatas)
            {
                var header = TRSTGHISFileUploadHeadersList.FirstOrDefault<TRSTGHISFileUploadHeader>
                                                                             (x => x.huh_hos_code == huh_hos_code
                                                                                && x.huh_hn_no == tRSTGHDCData.HN.Substring(tRSTGHDCData.HN.Length - 8, 8)
                                                                                && x.huh_date >= Convert.ToDateTime(tRSTGHDCData.AdmissionDate)
                                                                                && x.huh_date <= Convert.ToDateTime(tRSTGHDCData.DischargeDate));
                if (header != null)
                {
                    tRSTGHDCData.hdc_code = Guid.NewGuid().ToString();
                    tRSTGHDCData.huh_hfu_id = header.huh_hfu_id;
                    tRSTGHDCData.huh_hos_code = header.huh_hos_code;
                    tRSTGHDCData.huh_hn_no = header.huh_hn_no;
                    tRSTGHDCData.createdate = currentDateTime;
                    tRSTGHDCData.createuser = "BATCH";
                    tRSTGHDCDataAddList.Add(tRSTGHDCData);
                    tRSTGHISFileUploadDetailAddList.AddRange(Update_TRSTGHISFileUploadHeader_Data(header, TRSTGHISFileUploadDetailList, tRSTGHDCData, huh_hos_code, currentDateTime));
                    if (hfu_ids.IndexOf(header.huh_hfu_id) == -1)
                    {
                        hfu_ids.Add(header.huh_hfu_id);
                    }
                }
            }

            using (var _db = new HDCDataContext(new Microsoft.EntityFrameworkCore.DbContextOptions<HDCDataContext>()))
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        List<TRSTGHISFileUploadDetail> insertList = tRSTGHISFileUploadDetailAddList.Where(x => x.hud_id == 0).ToList();
                        List<TRSTGHISFileUploadDetail> updateList = tRSTGHISFileUploadDetailAddList.Where(x => x.hud_id > 0).ToList();

                        foreach (int hfu_id in hfu_ids)
                        {
                            TRHISFileUpload tRHISFileUpload = _db.TRHISFileUploads.FirstOrDefault<TRHISFileUpload>(x => x.hfu_id == hfu_id);
                            if (tRHISFileUpload != null)
                            {
                                tRHISFileUpload.hfu_hdc_status = "F"; // Finish
                                tRHISFileUpload.hfu_updateuser = "BATCH";
                                tRHISFileUpload.hfu_updatedate = currentDateTime;
                                tRHISFileUploadUpdateList.Add(tRHISFileUpload);
                            }
                        }

                        _db.BulkInsert(tRSTGHDCDataAddList);
                        _db.BulkInsert(insertList);
                        _db.BulkUpdate(updateList);
                        _db.BulkUpdate(tRHISFileUploadUpdateList);
                        _db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        // TODO: Handle failure
                        _log.LogError("Error : " + ex.Message);
                        trans.Rollback();
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }
        }

        private List<TRSTGHISFileUploadDetail> Update_TRSTGHISFileUploadHeader_Data(TRSTGHISFileUploadHeader tRSTGHISFileUploadHeader, List<TRSTGHISFileUploadDetail> tRSTGHISFileUploadDetailList, TRSTGHDCData tRSTGHDCData, string huh_hos_code, DateTime currentDateTime)
        {
            List<TRSTGHISFileUploadDetail> result = new List<TRSTGHISFileUploadDetail>();

            foreach (KeyValuePair<string, string> key in dataFields)
            {
                TRSTGHISFileUploadDetail tRSTGHISFileUploadDetail = new TRSTGHISFileUploadDetail();
                if (!string.IsNullOrEmpty(key.Value))
                    tRSTGHISFileUploadDetail = tRSTGHISFileUploadDetailList.FirstOrDefault(x => x.hud_huh_id == tRSTGHISFileUploadHeader.huh_id && x.hud_field_name == key.Value);
                
                else tRSTGHISFileUploadDetail = null;

                if (tRSTGHDCData.GetType().GetProperty(key.Key).GetValue(tRSTGHDCData, null) != null
                    && !string.IsNullOrEmpty(tRSTGHDCData.GetType().GetProperty(key.Key).GetValue(tRSTGHDCData, null).ToString()))
                {
                    if (tRSTGHISFileUploadDetail == null)
                    {
                        //insert
                        tRSTGHISFileUploadDetail = new TRSTGHISFileUploadDetail()
                        {
                            hud_huh_id = tRSTGHISFileUploadHeader.huh_id,
                            hud_seq_no = tRSTGHISFileUploadHeader.huh_seq_no,
                            hud_field_name = string.Empty,
                            hud_field_value = string.Empty,
                            hdc_field_name = key.Key,
                            hdc_field_value = tRSTGHDCData.GetType().GetProperty(key.Key).GetValue(tRSTGHDCData, null).ToString(),
                            hud_status = 'F',
                            hud_remarks = string.Empty,
                            hud_createdate = currentDateTime,
                            hud_createuser = "BATCH"
                        };
                        result.Add(tRSTGHISFileUploadDetail);
                    }
                    else
                    {
                        //Update
                        tRSTGHISFileUploadDetail.hud_updatedate = currentDateTime;
                        tRSTGHISFileUploadDetail.hud_updateuser = "BATCH";
                        tRSTGHISFileUploadDetail.hdc_field_name = key.Key;
                        tRSTGHISFileUploadDetail.hdc_field_value = tRSTGHDCData.GetType().GetProperty(key.Key).GetValue(tRSTGHDCData, null).ToString();
                        result.Add(tRSTGHISFileUploadDetail);
                    }
                }
            }

            return result;
        }

        async Task<byte[]> loadJsonData()
        {
            string filename = "C:\\Users\\Admin\\Desktop\\ALISS_UAT_2\\2. Connect HDC\\rawData.txt";
            byte[] result;

            using (FileStream SourceStream = File.Open(filename, FileMode.Open))
            {
                //result = Encoding.UTF8.GetBytes(SourceStream.);
                result = new byte[SourceStream.Length];
                await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
            }

            return result;
        }
    }
}
