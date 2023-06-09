﻿using ALISS.Data.Client;
using ALISS.Data.D0_Master;
using ALISS.Data.D1_Upload;
using ALISS.LabFileUpload.DTO;
using ALISS.Mapping.DTO;
using ALISS.DropDownList.DTO;
using ALISS.MasterManagement.DTO;
using DbfDataReader;
using ExcelDataReader;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Reflection.PortableExecutable;
using Microsoft.Data.Sqlite;
using ALISS.Helpers;
using SQLitePCL;

namespace ALISS.Data.D1_Upload
{
    public class FileUploadService
    {
        //private readonly IWebHostEnvironment _environment;
        //public FileUploadService(IWebHostEnvironment env)
        //{
        //    _environment = env;
        //}
        private IConfiguration configuration { get; }

        private ApiHelper _apiHelper;

        public FileUploadService(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration["ApiClient:ApiUrl"]);
        }

        private string _Path;


        public async Task<string> GetPath()
        {
            var ErrorMessage = new List<LabFileUploadErrorMessageDTO>();

            string path = "";
            List<ParameterDTO> objParamList = new List<ParameterDTO>();
            var searchModel = new ParameterDTO() { prm_code_major = "UPLOAD_PATH" };

            objParamList = await _apiHelper.GetDataListByModelAsync<ParameterDTO, ParameterDTO>("dropdownlist_api/GetParameterList", searchModel);

            if (objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH") != null)
            {
                path = objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
            }
            else
            {
                //ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                //{
                //    lfu_status = 'E',
                //    lfu_Err_type = 'E',
                //    lfu_Err_no = 1,
                //    lfu_Err_Column = "",
                //    lfu_Err_Message = "ไม่พบ Config PATH กรุณาติดต่อผู้ดูแลระบบ "
                //});

                //return ErrorMessage;
            }

            return path;
        }


        public async Task<List<LabFileUploadErrorMessageDTO>> ValidateLabFileAsync(string path, string fileName, MappingDataDTO MappingTemplate, string MLABfiletype)
        {
           
            var ErrorMessage = new List<LabFileUploadErrorMessageDTO>();
            int row = 1;
            try
            {
                WHONetMappingSearch searchWHONet = new WHONetMappingSearch();
                searchWHONet.wnm_mappingid = MappingTemplate.mp_id;
                searchWHONet.wnm_mst_code = MappingTemplate.mp_mst_code;

                List<WHONetMappingListsDTO> WHONetColumn = await _apiHelper.GetDataListByModelAsync<WHONetMappingListsDTO, WHONetMappingSearch>("mapping_api/Get_WHONetMappingListByModel", searchWHONet);
                var WHONetColumnMandatory = WHONetColumn.Where(x => x.wnm_mandatory == true);

                path = Path.Combine(path, fileName);
                #region ReadExcel
                if (Path.GetExtension(fileName) == ".xls" || Path.GetExtension(fileName) == ".xlsx")
                {
                    ExcelDataSetConfiguration option = new ExcelDataSetConfiguration();                    

                    using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                    {                      
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            DataSet result = new DataSet();

                            //First row is header
                            if (MappingTemplate.mp_firstlineisheader == true)
                            {

                                result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                    {
                                        UseHeaderRow = true
                                    }
                                }
                                );
                            }
                            else
                            {
                                result = reader.AsDataSet();
                            }

                            var dtResult = result.Tables[0];
                            ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                            {
                                lfu_status = 'I',
                                lfu_Err_type = 'I',
                                lfu_Err_no = 1,
                                lfu_Err_Column = "Total",
                                lfu_Err_Message = dtResult.Rows.Count.ToString()
                            }) ;

                            foreach (WHONetMappingListsDTO item in WHONetColumnMandatory)
                            {
                                var wnm_originalfield = item.wnm_originalfield;
                                var wnm_whonetfield = item.wnm_whonetfield;
                                if (MappingTemplate.mp_firstlineisheader == false)
                                {
                                    int index = 0;
                                    Int32.TryParse(item.wnm_originalfield.Replace("Column", ""), out index);

                                    item.wnm_originalfield = "Column" + (index - 1);
                                }
                                if (MLABfiletype == "ETEST" && wnm_whonetfield == "Specimen type")
                                {
                                    continue;
                                }
                                Boolean columnExists = dtResult.Columns.Contains(item.wnm_originalfield);
                                if (columnExists == false)
                                {
                                    ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                                    {
                                        lfu_status = 'E',
                                        lfu_Err_type = 'C',
                                        lfu_Err_no = 1,
                                        lfu_Err_Column = wnm_originalfield,
                                        lfu_Err_Message = "ไม่พบ Column " + wnm_originalfield
                                    });
                                }

                                if (columnExists)
                                {                                  
                                    var chkResult = dtResult.Select("[" + item.wnm_originalfield + "]" + " is null");

                                    if (chkResult.Length > 0)
                                    {
                                        ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                                        {
                                            lfu_status = 'E',
                                            lfu_Err_type = 'N',
                                            lfu_Err_no = 1,
                                            lfu_Err_Column = wnm_originalfield,
                                            lfu_Err_Message = "กรุณาตรวจสอบข้อมูล Column " + wnm_originalfield + " จะต้องไม่เท่ากับค่าว่าง"
                                        });
                                    }
                                }
                             } //end check mandatory column

                            var WHONetSpecimenDate = WHONetColumn.Where(a => a.wnm_whonetfield == "Specimen date");
                            if (WHONetSpecimenDate.Count()>0)
                            {
                                WHONetMappingListsDTO wHONetMappingLists = WHONetSpecimenDate.FirstOrDefault();
                                DateTime temp = new DateTime();
                                string dateFormat = wHONetMappingLists.wnm_fieldformat;

                                dateFormat = Regex.Replace(dateFormat, "d", "d", RegexOptions.IgnoreCase);
                                //dateFormat = Regex.Replace(dateFormat, "m", "M", RegexOptions.IgnoreCase);
                                dateFormat = Regex.Replace(dateFormat, "y", "y", RegexOptions.IgnoreCase);

                                foreach (DataRow rows in dtResult.Rows)
                                {
                                    if (rows[wHONetMappingLists.wnm_originalfield].GetType() != typeof(DateTime))
                                    {
                                        DateTime.TryParseExact(rows[wHONetMappingLists.wnm_originalfield].ToString(), dateFormat, System.Globalization.CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out temp);
                                        if (temp == DateTime.MinValue)
                                        {
                                            ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                                            {
                                                lfu_status = 'E',
                                                lfu_Err_type = 'N',
                                                lfu_Err_no = 1,
                                                lfu_Err_Column = wHONetMappingLists.wnm_originalfield,
                                                lfu_Err_Message = "กรุณาตรวจสอบข้อมูล Column " + wHONetMappingLists.wnm_originalfield + " มีค่าไม่ถูกต้อง"
                                            });
                                            break;
                                        }
                                    }
                                }
                            }
                            var x = ErrorMessage;
                        }
                    }
                }
                #endregion
                #region ReadCSV
                else if (Path.GetExtension(fileName) == ".csv")
                {
                    using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        
                        var reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                        {
                            FallbackEncoding = Encoding.GetEncoding(1252),
                            AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' },
                            LeaveOpen = false,
                            AnalyzeInitialCsvRows = 0,
                        });

                        DataSet result = new DataSet();


                        //First row is header
                        if (MappingTemplate.mp_firstlineisheader == true)
                        {

                            result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            }
                            );
                        }
                        else
                        {
                            result = reader.AsDataSet();
                        }

                        ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                        {
                            lfu_status = 'I',
                            lfu_Err_type = 'I',
                            lfu_Err_no = 1,
                            lfu_Err_Column = "Total",
                            lfu_Err_Message = result.Tables[0].Rows.Count.ToString()
                        });

                        var ee = result.Tables[0];
                        foreach (WHONetMappingListsDTO item in WHONetColumnMandatory)
                        {
                            var wnm_originalfield = item.wnm_originalfield;
                            var wnm_whonetfield = item.wnm_whonetfield;
                            if (MappingTemplate.mp_firstlineisheader == false)
                            {
                                int index = 0;
                                Int32.TryParse(item.wnm_originalfield.Replace("Column", ""), out index);

                                item.wnm_originalfield = "Column" + (index - 1);
                            }
                            if (MLABfiletype == "ETEST" && wnm_whonetfield == "Specimen type")
                            {
                                continue;
                            }
                            Boolean columnExists = result.Tables[0].Columns.Contains(item.wnm_originalfield);
                            if(columnExists == false)
                            {
                                ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                                {
                                    lfu_status = 'E',
                                    lfu_Err_type = 'C',
                                    lfu_Err_no = 1,
                                    lfu_Err_Column = wnm_originalfield,
                                    lfu_Err_Message = "ไม่พบ Column " + wnm_originalfield
                                });
                            }

                            if (columnExists)
                            {
                                var chkResult = result.Tables[0].Select(item.wnm_originalfield + " = ''");

                                if (chkResult.Length > 0)
                                {
                                    ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                                    {
                                        lfu_status = 'E',
                                        lfu_Err_type = 'N',
                                        lfu_Err_no = 1,
                                        lfu_Err_Column = wnm_originalfield,
                                        lfu_Err_Message = "กรุณาตรวจสอบข้อมูล Column " + wnm_originalfield + " จะต้องไม่เท่ากับค่าว่าง"
                                    });
                                }

                            }
                        }

                        var x = ErrorMessage;

                    }
                }
                #endregion
                #region ReadText
                else if (Path.GetExtension(fileName) == ".txt")
                {
                    string line;
                    DataTable dt = new DataTable();
                    //string tempFilename = Guid.NewGuid().ToString() + ".txt";
                    //var path = Path.Combine(@"D:\Work\02-DMSC ALISS\TEMP\", tempFilename);
                    //var ms = new MemoryStream();
                    //await fileEntry.Data.CopyToAsync(ms);
                    //using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
                    //{
                    //    ms.WriteTo(file);
                    //}

                    //using (TextReader tr = File.OpenText(@"D:\Work\02-DMSC ALISS\TEMP\" + tempFilename))
                    using (TextReader tr = File.OpenText(path))
                    {    
                        while ((line = tr.ReadLine()) != null)
                        {
                            string[] items = line.Split('\t');

                            if (dt.Columns.Count == 0)
                            {
                                if (MappingTemplate.mp_firstlineisheader == false)
                                {   
                                    for (int i = 0; i < items.Length; i++)
                                        dt.Columns.Add(new DataColumn("Column" + i, typeof(string)));
                                }
                                else
                                {
                                    for (int i = 0; i < items.Length; i++)
                                        dt.Columns.Add(new DataColumn(items[i].ToString(), typeof(string)));
                                }
                            }
                            dt.Rows.Add(items);
                        }
                    }                      

                        //File.Delete(path);
                    ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                    {
                        lfu_status = 'I',
                        lfu_Err_type = 'I',
                        lfu_Err_no = 1,
                        lfu_Err_Column = "Total",
                        lfu_Err_Message = (dt.Rows.Count-1).ToString()
                    });
                    
                    foreach (WHONetMappingListsDTO item in WHONetColumnMandatory)
                    {
                        var wnm_originalfield = item.wnm_originalfield;
                        var wnm_whonetfield = item.wnm_whonetfield;
                        if (MappingTemplate.mp_firstlineisheader == false)
                        {
                            int index = 0;
                            Int32.TryParse(item.wnm_originalfield.Replace("Column", ""), out index);

                            item.wnm_originalfield = "Column" + (index - 1);
                        }
                        if (MLABfiletype == "ETEST" && wnm_whonetfield == "Specimen type")
                        {
                            continue;
                        }
                        Boolean columnExists = dt.Columns.Contains(item.wnm_originalfield);
                        if (columnExists == false)
                        {
                            ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                            {
                                lfu_status = 'E',
                                lfu_Err_type = 'C',
                                lfu_Err_no = 1,
                                lfu_Err_Column = wnm_originalfield,
                                lfu_Err_Message = "ไม่พบ Column " + wnm_originalfield
                            });
                        }

                        if (columnExists)
                        {
                            var chkResult = dt.Select(item.wnm_originalfield + " = ''");

                            if (chkResult.Length > 0)
                            {
                                ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                                {
                                    lfu_status = 'E',
                                    lfu_Err_type = 'N',
                                    lfu_Err_no = 1,
                                    lfu_Err_Column = wnm_originalfield,
                                    lfu_Err_Message = "กรุณาตรวจสอบข้อมูล Column " + wnm_originalfield + " จะต้องไม่เท่ากับค่าว่าง"
                                });
                            }

                        }

                    }

                }
                #endregion
                #region ReadSqlite
                else if (Path.GetExtension(fileName) == ".sqlite")
                {
                    Batteries.Init();
                    DataSet result = new DataSet();
                    string connectionString = string.Format("Data Source={0}", path);
                    using (SqliteConnection connection = new SqliteConnection(connectionString))
                    {
                        connection.Open();

                        string query = "SELECT * FROM Isolates";
                        using (SqliteDataAdapter dataAdapter = new SqliteDataAdapter())
                        {
                            dataAdapter.SelectCommand = new SqliteCommand(query, connection);
                            dataAdapter.Fill(result);
                        }
                    }

                    var dtResult = result.Tables[0];
                    ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                    {
                        lfu_status = 'I',
                        lfu_Err_type = 'I',
                        lfu_Err_no = 1,
                        lfu_Err_Column = "Total",
                        lfu_Err_Message = dtResult.Rows.Count.ToString()
                    });

                    foreach (WHONetMappingListsDTO item in WHONetColumnMandatory)
                    {
                        var wnm_originalfield = item.wnm_originalfield;
                        var wnm_whonetfield = item.wnm_whonetfield;
                        if (MappingTemplate.mp_firstlineisheader == false)
                        {
                            int index = 0;
                            Int32.TryParse(item.wnm_originalfield.Replace("Column", ""), out index);

                            item.wnm_originalfield = "Column" + (index - 1);
                        }
                        if (MLABfiletype == "ETEST" && wnm_whonetfield == "Specimen type")
                        {
                            continue;
                        }
                        Boolean columnExists = dtResult.Columns.Contains(item.wnm_originalfield);
                        if (columnExists == false)
                        {
                            ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                            {
                                lfu_status = 'E',
                                lfu_Err_type = 'C',
                                lfu_Err_no = 1,
                                lfu_Err_Column = wnm_originalfield,
                                lfu_Err_Message = "ไม่พบ Column " + wnm_originalfield
                            });
                        }

                        if (columnExists)
                        {
                            var chkResult = dtResult.Select("[" + item.wnm_originalfield + "]" + " is null");

                            if (chkResult.Length > 0)
                            {
                                ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                                {
                                    lfu_status = 'E',
                                    lfu_Err_type = 'N',
                                    lfu_Err_no = 1,
                                    lfu_Err_Column = wnm_originalfield,
                                    lfu_Err_Message = "กรุณาตรวจสอบข้อมูล Column " + wnm_originalfield + " จะต้องไม่เท่ากับค่าว่าง"
                                });
                            }
                        }
                    } //end check mandatory column

                    var WHONetSpecimenDate = WHONetColumn.Where(a => a.wnm_whonetfield == "Specimen date");
                    if (WHONetSpecimenDate.Count() > 0)
                    {
                        WHONetMappingListsDTO wHONetMappingLists = WHONetSpecimenDate.FirstOrDefault();
                        DateTime temp = new DateTime();
                        string dateFormat = wHONetMappingLists.wnm_fieldformat;

                        dateFormat = Regex.Replace(dateFormat, "d", "d", RegexOptions.IgnoreCase);
                        //dateFormat = Regex.Replace(dateFormat, "m", "M", RegexOptions.IgnoreCase);
                        dateFormat = Regex.Replace(dateFormat, "y", "y", RegexOptions.IgnoreCase);

                        dateFormat = "yyyy-MM-dd";

                        foreach (DataRow rows in dtResult.Rows)
                        {
                            if (rows[wHONetMappingLists.wnm_originalfield].GetType() != typeof(DateTime))
                            {
                                DateTime.TryParseExact(rows[wHONetMappingLists.wnm_originalfield].ToString(), dateFormat, System.Globalization.CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out temp);
                                if (temp == DateTime.MinValue)
                                {
                                    ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                                    {
                                        lfu_status = 'E',
                                        lfu_Err_type = 'N',
                                        lfu_Err_no = 1,
                                        lfu_Err_Column = wHONetMappingLists.wnm_originalfield,
                                        lfu_Err_Message = "กรุณาตรวจสอบข้อมูล Column " + wHONetMappingLists.wnm_originalfield + " มีค่าไม่ถูกต้อง"
                                    });
                                    break;
                                }
                            }
                        }
                    }
                    var x = ErrorMessage;
                }
                #endregion
                #region ReadDataAccess
                else if (Path.GetExtension(fileName) == ".accdb")
                {

                }
                #endregion
                else
                {
                //string tempFilename = Guid.NewGuid().ToString() + ".dbf";


                var options = new DbfDataReaderOptions
                {
                    Encoding = Encoding.GetEncoding(874)
                };


                using (var dbfDataReader = new DbfDataReader.DbfDataReader(path, options))
                //using (var dbfDataReader = new DbfDataReader.DbfDataReader(@"D:\Work\02-DMSC ALISS\TEMP\" + tempFilename, options))
                {
                    ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                    {
                        lfu_status = 'I',
                        lfu_Err_type = 'I',
                        lfu_Err_no = 1,
                        lfu_Err_Column = "Total",
                        lfu_Err_Message = dbfDataReader.DbfTable.Header.RecordCount.ToString()
                    });

                    while (dbfDataReader.Read())
                    {
                        //Validate Mandatory Field
                        foreach (WHONetMappingListsDTO item in WHONetColumnMandatory)
                        {
                            var columnExists = dbfDataReader.DbfTable.Columns.FirstOrDefault(x => x.Name == item.wnm_originalfield);
                            if (MLABfiletype == "ETEST" && item.wnm_whonetfield == "Specimen type")
                            {
                                continue;
                            }
                            //var ll = dbfDataReader.DbfTable.Rea
                            if (columnExists != null)
                            {
                                // 16/03/2021 : ถ้ายอมให้ Organism เป็น null ได้?? comment ตรงนี้ 

                                //if (dbfDataReader[item.wnm_originalfield] == "" || dbfDataReader[item.wnm_originalfield] == null)
                                //{
                                //    if (ErrorMessage.FirstOrDefault(x => x.lfu_Err_type == 'N' && x.lfu_Err_Column == item.wnm_originalfield) == null)
                                //    {
                                //        ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                                //        {
                                //            lfu_status = 'E',
                                //            lfu_Err_type = 'N',
                                //            lfu_Err_no = 1,
                                //            lfu_Err_Column = item.wnm_originalfield,
                                //            lfu_Err_Message = "กรุณาตรวจสอบข้อมูล Column " + item.wnm_originalfield + " จะต้องไม่เท่ากับค่าว่าง"
                                //        });
                                //    }
                                //}
                            }
                            else
                            {
                                if (ErrorMessage.FirstOrDefault(x => x.lfu_Err_type == 'C' && x.lfu_Err_Column == item.wnm_originalfield) == null)
                                {
                                    ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                                    {
                                        lfu_status = 'E',
                                        lfu_Err_type = 'C',
                                        lfu_Err_no = 1,
                                        lfu_Err_Column = item.wnm_originalfield,
                                        lfu_Err_Message = "ไม่พบ Column " + item.wnm_originalfield
                                    });
                                }
                            }

                        }
                        row++;
                    }
                    var x = ErrorMessage;

                    //using (FileStream file = new FileStream(@"D:\Work\02-DMSC ALISS\TEMP\"+ fileEntry.Name, FileMode.Create))
                    //{
                    //    try
                    //    {
                    //        await fileEntry.Data.CopyToAsync(file);
                    //    }
                    //    catch (Exception ex)
                    //    {

                    //    }
                    //    finally
                    //    {
                    //        file.Flush();
                    //    }
                    //}


                    //var zipStream = new MemoryStream();


                    //using (var compressedFileStream = new MemoryStream())
                    //{
                    //    //compressedFileStream.Seek(0, SeekOrigin.Begin);
                    //    using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                    //    {
                    //        var zipEntry = zipArchive.CreateEntry(fileEntry.Name);
                    //        using (var zipEntryStream = zipEntry.Open())
                    //        {
                    //            try
                    //            {
                    //                await fileEntry.Data.CopyToAsync(zipEntryStream);
                    //                //fileEntry.Data.CopyTo(zipEntryStream);
                    //            }
                    //            catch (Exception ex)
                    //            {

                    //            }
                    //            finally
                    //            {
                    //                zipEntryStream.Flush();
                    //            }
                    //        }
                    //    }

                    //    using (var fileStream = new FileStream(@"D:\Work\02-DMSC ALISS\TEMP\test.zip", FileMode.Create))
                    //    {
                    //        var bytes = compressedFileStream.GetBuffer();
                    //        fileStream.Write(bytes, 0, bytes.Length);
                    //        try
                    //        {
                    //            await compressedFileStream.CopyToAsync(fileStream);
                    //        }
                    //        catch (Exception ex)
                    //        {

                    //        }
                    //        finally
                    //        {
                    //            fileStream.Flush();
                    //        }
                    //    }
                    //}
                    //using (var outStream = new MemoryStream())
                    //{
                    //    using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                    //    {
                    //        var fileInArchive = archive.CreateEntry(fileEntry.Name, CompressionLevel.Optimal);
                    //        using (var entryStream = fileInArchive.Open())

                    //            await fileEntry.Data.CopyToAsync(entryStream);
                                 
                    //    }
                    //    using (var fileStream = new FileStream(@"D:\test.zip", FileMode.Create))
                    //    {
                    //        outStream.Seek(0, SeekOrigin.Begin);
                    //        outStream.CopyTo(fileStream);
                    //    }
                    //}
                }
                //File.Delete(path);
            }


                var chkError = ErrorMessage.FirstOrDefault(x => x.lfu_status == 'E');
                if(chkError != null)
                {
                    File.Delete(path);
                }
                else
                {
                    ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                    {
                        lfu_status = 'I',
                        lfu_Err_type = 'P',
                        lfu_Err_no = 1,
                        lfu_Err_Column = "path",
                        lfu_Err_Message = path
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Add(new LabFileUploadErrorMessageDTO
                {
                    lfu_status = 'E',
                    lfu_Err_type = 'E',
                    lfu_Err_no = 1,
                    lfu_Err_Column = "",
                    lfu_Err_Message = ex.Message
                });
            }

            return ErrorMessage;
        }


        public async Task<LabFileUploadDataDTO> UploadFileAsync(LabFileUploadDataDTO model)
        {
            LabFileUploadDataDTO objReturn = new LabFileUploadDataDTO();
                        

            if (model.lfu_id.Equals(Guid.Empty))
            {
                model.lfu_id = Guid.NewGuid();
                //model.lfu_Path = path;
                model.lfu_AerobicCulture = 0;
                model.lfu_status = 'N';
                model.lfu_flagdelete = false;
            }
            else
            {
                model.lfu_status = 'E';
            }

            model.lfu_updatedate = DateTime.Now;
            objReturn = await _apiHelper.PostDataAsync<LabFileUploadDataDTO>("labfileupload_api/Post_SaveLabFileUploadData", model);



            return objReturn;

        }


    }
}