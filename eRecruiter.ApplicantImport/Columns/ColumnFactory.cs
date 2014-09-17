using System;

namespace eRecruiter.ApplicantImport.Columns
{
    public static class ColumnFactory
    {
        public static AbstractColumn GetColumn(ColumnType type, string additionalType, string header)
        {
            AbstractColumn result;

            try
            {
                var columnTypeName = "eRecruiter.ApplicantImport.Columns." + type + "Column";
                var columnType = Type.GetType(columnTypeName);
                if (columnType == null)
                    throw new ApplicationException("Class '" + columnTypeName + "' not found.");
                result = (AbstractColumn)Activator.CreateInstance(columnType, additionalType, header);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Column type '" + type + "' is not supported: " + ex.Message, ex);
            }

            return result;
        }
    }
}
