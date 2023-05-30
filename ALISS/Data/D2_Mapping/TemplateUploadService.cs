using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ALISS.Mapping.DTO;
using ExcelDataReader;
using System.Text;
using DbfDataReader;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using Microsoft.AspNetCore.Components.Forms;

namespace ALISS.Data.D2_Mapping
{
    public class TemplateUploadService
    {
        private readonly IWebHostEnvironment _environment;
        public TemplateUploadService(IWebHostEnvironment env)
        {
            _environment = env;
        }
        public async Task<List<TemplateFileListsDTO>> UploadAsync(IBrowserFile fileEntry,bool? FirstLineIsHeader)
        {
            var Templates = new List<TemplateFileListsDTO>();
            string _tempheader = "", _tempvalue = "",path = "";
            int _temprow = 0;
            try
            {
                var g = Path.GetExtension(fileEntry.Name);
                long UploadedBytes = 0;
                long TotalBytes = fileEntry.Size;
                long chunckSize = 400000;
                long numChunks = TotalBytes / chunckSize;
                long remainder = TotalBytes % chunckSize;

                long maxFileSize = 1024L; // 1KB
                maxFileSize *= 1024L; // 1MB
                maxFileSize *= 512L; // 0.5GB
                                     //maxFileSize *= 1024L; // 1TB ???

                path = Path.Combine(Path.GetTempPath(), fileEntry.Name);
                if (File.Exists(path)) File.Delete(path);

                using (var inStream = fileEntry.OpenReadStream(long.MaxValue))
                {
                    using (var outStream = File.OpenWrite(path))
                    {
                        for (int i = 0; i < numChunks; i++)
                        {
                            var buffer = new byte[chunckSize];
                            await inStream.ReadAsync(buffer, 0, buffer.Length);
                            await outStream.WriteAsync(buffer, 0, buffer.Length);
                            UploadedBytes += chunckSize;
                        }

                        if (remainder > 0)
                        {
                            var buffer = new byte[remainder];
                            await inStream.ReadAsync(buffer, 0, buffer.Length);

                            await outStream.WriteAsync(buffer, 0, buffer.Length);
                            UploadedBytes += remainder;
                            //percent = UploadedBytes * 100 / TotalBytes;                                                             
                        }
                    }
                }

                #region ReadExcel
                if (Path.GetExtension(fileEntry.Name) == ".xls" || Path.GetExtension(fileEntry.Name) == ".xlsx")
                {                                
                    using (var fstream = File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(fstream))
                        {
                            var result = reader.AsDataSet();
                            do
                            {
                                _temprow = 1;

                                while (reader.Read() && _temprow <= 2) //Each ROW
                                {
                                    if (_temprow == 1)
                                    {
                                        for (int column = 0; column < reader.FieldCount; column++)
                                        {
                                            if (FirstLineIsHeader == true)
                                            {
                                                if (reader.GetValue(column) != null)
                                                {
                                                    _tempheader = reader.GetValue(column).ToString().Trim();
                                                }
                                                else
                                                    _tempheader = "";
                                            }
                                            else
                                            {
                                                _tempheader = "Column" + (column + 1).ToString();

                                                if (reader.GetValue(column) != null)
                                                {
                                                    _tempvalue = reader.GetValue(column).ToString();
                                                }
                                                else
                                                    _tempvalue = "";
                                            }

                                            Templates.Add(new TemplateFileListsDTO
                                            {
                                                tmp_no = column,
                                                tmp_header = _tempheader,
                                                tmp_value = _tempvalue
                                            }
                                            );
                                        }
                                    }
                                    else if (_temprow == 2 && FirstLineIsHeader == true)
                                    {
                                        for (int column = 0; column < reader.FieldCount; column++)
                                        {
                                            TemplateFileListsDTO template = Templates.Where(t => t.tmp_no == column).FirstOrDefault();
                                            if (reader.GetValue(column) != null)
                                            {
                                                template.tmp_value = reader.GetValue(column).ToString();
                                                var testvalue = reader.GetValue(column);
                                            }
                                        }
                                    }
                                    _temprow++;

                                }
                            } while (reader.NextResult());

                        }
                    }

                    if (File.Exists(path)) File.Delete(path);

                }
                #endregion
                #region Readcsv
                else if (Path.GetExtension(fileEntry.Name) == ".csv")
                {                
                    using (var fstream = File.Open(path, FileMode.Open, FileAccess.Read))
                    {          
                        var reader = ExcelReaderFactory.CreateCsvReader(fstream, new ExcelReaderConfiguration()
                        {
                            FallbackEncoding = Encoding.GetEncoding(1252),
                            AutodetectSeparators = new char[] { ',', ';', '\t', '|', '#' },
                            LeaveOpen = false,
                            AnalyzeInitialCsvRows = 0,
                        });

                        var result = reader.AsDataSet();
                        do
                        {
                            _temprow = 1;

                            while (reader.Read() && _temprow <= 2) //Each ROW
                            {
                                if (_temprow == 1)
                                {
                                    for (int column = 0; column < reader.FieldCount; column++)
                                    {
                                        if (FirstLineIsHeader == true)
                                        {
                                            if (reader.GetValue(column) != null)
                                            {
                                                _tempheader = reader.GetValue(column).ToString().Trim();
                                            }
                                            else
                                                _tempheader = "";
                                        }
                                        else
                                        {
                                            _tempheader = "Column" + (column + 1).ToString();

                                            if (reader.GetValue(column) != null)
                                            {
                                                _tempvalue = reader.GetValue(column).ToString();
                                            }
                                            else
                                                _tempvalue = "";
                                        }

                                        Templates.Add(new TemplateFileListsDTO
                                        {
                                            tmp_no = column,
                                            tmp_header = _tempheader,
                                            tmp_value = _tempvalue
                                        }
                                        );
                                    }
                                }
                                else if (_temprow == 2 && FirstLineIsHeader == true)
                                {
                                    for (int column = 0; column < reader.FieldCount; column++)
                                    {
                                        TemplateFileListsDTO template = Templates.Where(t => t.tmp_no == column).FirstOrDefault();
                                        if (reader.GetValue(column) != null)
                                        {
                                            template.tmp_value = reader.GetValue(column).ToString();
                                            var testvalue = reader.GetValue(column);
                                        }
                                    }
                                }
                                _temprow++;

                            }
                        } while (reader.NextResult());
                    }

                    if (File.Exists(path)) File.Delete(path);
                }
                #endregion
                #region ReadText
                else if (Path.GetExtension(fileEntry.Name) == ".txt")
                {
                 
                    using (TextReader tr = File.OpenText(path))
                    {
                        string line;
                        _temprow = 1;
                        while ((line = tr.ReadLine()) != null && _temprow <= 2)
                        {
                            string[] items = line.Split('\t');


                            if (_temprow == 1)
                            {
                                for (int i = 0; i < items.Length; i++)
                                {
                                    if (FirstLineIsHeader == true)
                                    {
                                        _tempheader = items[i].ToString();
                                    }
                                    else
                                    {
                                        _tempheader = "Column" + (i + 1).ToString();
                                        _tempvalue = items[i].ToString();
                                    }
                                    Templates.Add(new TemplateFileListsDTO
                                    {
                                        tmp_no = i,
                                        tmp_header = _tempheader,
                                        tmp_value = _tempvalue
                                    }
                                    );
                                }
                            }
                            else if (_temprow == 2 && FirstLineIsHeader == true)
                            {
                                for (int i = 0; i < items.Length; i++)
                                {
                                    TemplateFileListsDTO template = Templates.Where(t => t.tmp_no == i).FirstOrDefault();
                                    template.tmp_value = items[i].ToString();
                                }
                            }

                            _temprow++;

                        }
                    }

                    if (File.Exists(path)) File.Delete(path);
                }

                #endregion
                else 
                {
                    
                    var options = new DbfDataReaderOptions
                    {
                        Encoding = Encoding.GetEncoding(874)
                    };
                    using (var dbfDataReader = new DbfDataReader.DbfDataReader(path, options))
                    {
                        var dbfTable = dbfDataReader.DbfTable;

                        var header = dbfTable.Header;
                        var recordCount = header.RecordCount;
                        var column = 1;

                        if (FirstLineIsHeader == true)
                        {
                            foreach (var dbfColumn in dbfTable.Columns)
                            {

                                Templates.Add(new TemplateFileListsDTO
                                {
                                    tmp_no = column,
                                    tmp_header = dbfColumn.Name
                                }
                                );
                                column++;
                            }

                        }
                        else
                        {
                            foreach (var dbfColumn in dbfTable.Columns)
                            {
                                Templates.Add(new TemplateFileListsDTO
                                {
                                    tmp_no = column,
                                    tmp_header = "Column" + (column).ToString()
                                }
                                );
                                column++;
                            }
                        }

                        column = 1;
                        _temprow = 1;
                        var dbfRecord = new DbfRecord(dbfTable);
                        while (dbfTable.Read(dbfRecord) && _temprow < 2)
                        {
                            foreach (var dbfValue in dbfRecord.Values)
                            {
                                TemplateFileListsDTO template = Templates.Where(t => t.tmp_no == column).FirstOrDefault();
                                if (dbfValue != null)
                                {
                                    template.tmp_value = dbfValue.ToString();
                                }
                                //var stringValue = dbfValue.ToString();
                                //var obj = dbfValue.GetValue();
                                column++;
                            }
                            _temprow++;
                        }

                    }

                    if (File.Exists(path)) File.Delete(path);
                }

                if (File.Exists(path)) File.Delete(path);
            }
            catch(Exception ex)
            {
                if (File.Exists(path)) File.Delete(path);
                return Templates;
            }

            return Templates;
        }

    }
}
