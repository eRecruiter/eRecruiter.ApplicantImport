using eRecruiter.Api.Client;
using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;
using System;
using System.Globalization;

namespace eRecruiter.ApplicantImport.Columns
{
    public class BirthdateColumn : AbstractColumn
    {
        public BirthdateColumn(string header)
            : base(ColumnType.Birthdate, header)
        {
        }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value.HasValue() && ParseDate(value) == null)
            {
                Program.WriteWarning("The value '" + value + "' is not a valid date for column '" + Header + "'. Format '" + DateFormat + "' expected.");
                return false;
            }
            return true;
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue())
            {
                var date = ParseDate(value);
                if (date != null)
                    applicant.BirthDate = date.Value;
            }
        }

        private DateTime? ParseDate(string value)
        {
            DateTime date;
            if (DateTime.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return date;
            return null;
        }
    }
}
