using ALISS.Process.Batch.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ALISS.Process.Batch
{
    public static class P00_BATCH_Service
    {
        public static string GetConfigurationValue(string param)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration _iconfiguration = builder.Build();

            return _iconfiguration.GetValue<string>(param);
        }

        private static string Get_Data_FolderName(string prm_type, TRProcessRequestLabData pr_labData, int process_year)
        {
            var returnString = "";
            if (prm_type == "Hos") returnString = $"P_{pr_labData.pcl_arh_code}_{pr_labData.pcl_prv_code}_{pr_labData.pcl_hos_code}_{pr_labData.pcl_lab_code}_{process_year.ToString()}";
            else if (prm_type == "Prv") returnString = $"P_{pr_labData.pcl_arh_code}_{pr_labData.pcl_prv_code}_000000000_000_{process_year.ToString()}";
            else if (prm_type == "Arh") returnString = $"P_{pr_labData.pcl_arh_code}_00_000000000_000_{process_year.ToString()}";
            return returnString;
        }

        public static void Create_Folder_Empty(string folderName)
        {
            if (Directory.Exists(folderName) == false)
            {
                Directory.CreateDirectory(folderName);
            }
            else
            {
                Directory.GetFiles(folderName).ToList().ForEach(file => File.Delete(file));
            }
        }

        public static void Create_Folder(string folderName)
        {
            if (Directory.Exists(folderName) == false)
            {
                Directory.CreateDirectory(folderName);
            }
        }

        public static void Create_TextFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.Create(fileName);
        }

        public static void Delete_File_Exists(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        public static void Copy_File(string sourceFileName, string destinationFileName)
        {
            if (File.Exists(destinationFileName))
            {
                File.Delete(destinationFileName);
            }
            File.Copy(sourceFileName, destinationFileName);
        }

        public static void Move_File(string sourceFileName, string destinationFileName)
        {
            if (File.Exists(destinationFileName))
            {
                File.Delete(destinationFileName);
            }
            File.Copy(sourceFileName, destinationFileName);
            File.Delete(sourceFileName);
        }

        public static void Delete_Folder(string folderName)
        {
            if (Directory.Exists(folderName))
            {
                Directory.GetFiles(folderName).ToList().ForEach(file => File.Delete(file));

                Directory.Delete(folderName);
            }
        }

    }
}
