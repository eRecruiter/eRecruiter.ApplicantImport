using System;
using eRecruiter.Utilities;

namespace eRecruiter.ApplicantImport.Columns
{
    public static class ColumnFactory
    {
        public static AbstractColumn GetColumn(Configuration.Column column)
        {
            AbstractColumn result;

            try
            {
                var columnTypeName = "eRecruiter.ApplicantImport.Columns." + column.Type + "Column";
                var columnType = Type.GetType(columnTypeName);
                if (columnType == null)
                {
                    throw new ApplicationException("Class '" + columnTypeName + "' not found.");
                }
                result = (AbstractColumn) Activator.CreateInstance(columnType, column.Header);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Column type '" + column.Type + "' is not supported: " + ex.Message, ex);
            }

            result.SubType = null;
            result.DateFormat = "yyyy-MM-dd";

            if (column.DateFormat.HasValue())
            {
                result.DateFormat = column.DateFormat;
            }
            if (column.SubType.HasValue())
            {
                result.SubType = column.SubType;
            }

            return result;
        }
    }
}
