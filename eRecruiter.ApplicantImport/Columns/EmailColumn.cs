using eRecruiter.Api.Client;
using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;
using System.Text.RegularExpressions;

namespace eRecruiter.ApplicantImport.Columns
{
    public class EmailColumn : AbstractColumn
    {
        public EmailColumn(string additionalType, string header) : base(ColumnType.Email, additionalType, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value != null && value.HasValue() && !Regex.IsMatch(value, @"[-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}"))
            {
                Program.WriteWarning("The value '" + value + "' is not a valid e-mail address.");
                return false;
            }
            return true;
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant)
        {
            if (value.HasValue()) // only set the last name, if we really have a value (use default value otherwise, because last name must not be empty).
                applicant.Email = value;
        }
    }
}
