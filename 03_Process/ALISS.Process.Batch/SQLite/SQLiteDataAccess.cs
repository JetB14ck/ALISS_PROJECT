using ALISS.Process.Batch.DTO;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace ALISS.Process.Batch.SQLite
{
    public class SQLiteDataAccess
    {
        private static IConfiguration _iconfiguration;

        public static void CreateTable(string filepath, string filename, List<string> columnName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE \"Isolates\" (");
            sb.AppendLine("\"ROW_IDX\"	INTEGER NOT NULL UNIQUE,");
            foreach (var item in columnName)
            {
                if (item.Contains("DATE"))
                {
                    sb.AppendLine($"\"{item}\"	DATE,");
                }
                else
                {
                    sb.AppendLine($"\"{item}\"	TEXT,");
                }
            }
            sb.AppendLine("PRIMARY KEY('ROW_IDX' AUTOINCREMENT)");
            sb.AppendLine(");");

            var fileFullName = Path.Combine(filepath, filename);
            if (File.Exists(fileFullName)) File.Delete(fileFullName);

            SQLiteConnection.CreateFile(fileFullName);

            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString(fileFullName)))
            {
                try
                {
                    using var cmd = new SQLiteCommand(con);
                    con.Open();
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public static void InsertData(string filepath, string filename, List<Dictionary<string, string>> dataList)
        {
            var fileFullName = Path.Combine(filepath, filename);

            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString(fileFullName)))
            {
                try
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        using (var trans = con.BeginTransaction())
                        {
                            foreach (var dataModel in dataList)
                            {
                                var columnList = string.Join(",", dataModel.Select(x =>  x.Key).ToList());
                                var valueList = string.Join(",", dataModel.Select(x => ("@" + x.Key.Replace(" ", ""))).ToList());

                                cmd.CommandText = $"INSERT INTO Isolates({columnList}) VALUES({valueList})";

                                foreach (var item in dataModel)
                                {
                                    cmd.Parameters.AddWithValue(("@" + item.Key.Replace(" ", "")), item.Value);
                                }
                                cmd.Prepare();

                                cmd.ExecuteNonQuery();
                            }

                            trans.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Dispose();
                }
            }
        }

        public static List<string> LoadTableName(string filepath, string filename)
        {
            var fileFullName = Path.Combine(filepath, filename);

            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString(fileFullName)))
            {
                var data = con.Query<string>("SELECT name FROM sqlite_master WHERE type ='table' AND name NOT LIKE 'SchemaVersion%' AND name NOT LIKE 'sqlite_%';", new DynamicParameters());
                return data.ToList();
            }
        }

        public static List<IsolatesDTO> LoadData(string filepath, string filename)
        {
            var fileFullName = Path.Combine(filepath, filename);

            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString(fileFullName)))
            {
                var data = con.Query<IsolatesDTO>("SELECT * FROM Isolates", new DynamicParameters());
                return data.ToList();
            }
        }

        public static List<IsolatesListing> LoadOutputDataLabAlert(string filepath, string filename)
        {
            var fileFullName = Path.Combine(filepath, filename);

            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString(fileFullName)))
            {
                var output = new List<IsolatesListing>();
                try
                {
                    output = con.Query<IsolatesListing>($"SELECT * FROM [1 - Isolate alerts]", new DynamicParameters()).ToList();

                }
                catch (Exception ex)
                {

                }
                return output;
            }
        }

        public static List<IsolatesListing> LoadOutputDataLabAlertSummary(string filepath, string filename)
        {
            var fileFullName = Path.Combine(filepath, filename);

            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString(fileFullName)))
            {
                var output = new List<IsolatesListing>();
                try
                {
                    output = con.Query<IsolatesListing>($"SELECT * FROM [2 - Alert summary]", new DynamicParameters()).ToList();

                }
                catch (Exception ex)
                {

                }
                return output;
            }
        }

        public static List<IsolatesListing> LoadOutputDataListing(string filepath, string filename, string labRunNo)
        {
            var fileFullName = Path.Combine(filepath, filename);

            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString(fileFullName)))
            {
                var output = new List<IsolatesListing>();
                try
                {
                    output = con.Query<IsolatesListing>($"SELECT * FROM [{labRunNo} - Isolate listing]", new DynamicParameters()).ToList();

                }
                catch (Exception ex)
                {

                }
                return output;
            }
        }

        public static List<PercentRIS> LoadOutput(string filepath, string filename, string labRunNo)
        {
            var fileFullName = Path.Combine(filepath, filename);

            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString(fileFullName)))
            {
                var output = new List<PercentRIS>();
                try
                {
                    output = con.Query<PercentRIS>($"SELECT * FROM [{labRunNo} - %RIS]", new DynamicParameters()).ToList();

                }
                catch (Exception ex)
                {

                }
                return output;
            }
        }

        public static List<Output_RIS> LoadOutput_RIS(string filepath, string filename, string labRunNo)
        {
            var fileFullName = Path.Combine(filepath, filename);

            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString(fileFullName)))
            {
                var output = new List<Output_RIS>();
                try
                {
                    output = con.Query<Output_RIS>($"SELECT * FROM [{labRunNo} - %RIS]", new DynamicParameters()).ToList();

                }
                catch (Exception ex)
                {

                }
                return output;
            }
        }

        public static List<Output_Sample> LoadOutput_Sample(string filepath, string filename, string labRunNo)
        {
            var fileFullName = Path.Combine(filepath, filename);

            using (SQLiteConnection con = new SQLiteConnection(LoadConnectionString(fileFullName)))
            {
                var output = new List<Output_Sample>();
                try
                {
                    output = con.Query<Output_Sample>($"SELECT * FROM [{labRunNo} - Isolate summary]", new DynamicParameters()).ToList();

                }
                catch (Exception ex)
                {

                }
                return output;
            }
        }

        public static string LoadConnectionString(string dbPath)
        {
            return string.Format("Data Source={0}", dbPath);
        }
    }
}
