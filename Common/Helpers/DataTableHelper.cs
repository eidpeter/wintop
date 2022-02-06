using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace wintop.Common.Helpers
{
    public static class DataTableHelper
    {
        public static DataTable ListToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.DisplayName, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item).ToString();
                }
                table.Rows.Add(values);
            }
            return BeautifyDataTable(table);
        }

        public static DataTable BeautifyDataTable(DataTable dataTable)
        {
            foreach (DataColumn col in dataTable.Columns)
            {
                col.ColumnName += "     ";
            }
            return dataTable;
        }
    }
}