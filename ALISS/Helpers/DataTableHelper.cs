using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System;

namespace ALISS.Helpers
{
    public class DataTableHelper
    {
        public static DataTable RenameColumn(DataTable dt, List<MappingColumn> names)
        {
            DataRow dr = dt.NewRow();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (MappingColumn name in names)
                {
                    if (name.OriginalName == column.ColumnName)
                        column.ColumnName = name.NewName;
                    else
                        continue;
                }
            }
            return dt;
        }
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        try
                        {
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        }
                        catch
                        {
                            pro.SetValue(obj, string.Empty, null);
                        }
                    }
                    else
                        continue;
                }
            }
            return obj;
        }
    }

    public class MappingColumn
    { 
        public string OriginalName { get; set; }
        public string NewName { get; set; }
    }
}
