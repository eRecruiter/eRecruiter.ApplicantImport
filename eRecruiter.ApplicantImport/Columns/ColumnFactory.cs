using System;

namespace eRecruiter.ApplicantImport.Columns
{
    public static class ColumnFactory
    {
        public static AbstractColumn GetColumn(ColumnType type, string additionalType, string header)
        {
            AbstractColumn result;

            switch (type)
            {
                case ColumnType.LastName:
                    result = new LastNameColumn(additionalType, header);
                    break;
                case ColumnType.FirstName:
                    result = new FirstNameColumn(additionalType, header);
                    break;
                case ColumnType.Email:
                    result = new EmailColumn(additionalType, header);
                    break;
                default:
                    throw new ApplicationException("Column type '" + type + "' is not supported.");
            }

            return result;
        }
    }
}
