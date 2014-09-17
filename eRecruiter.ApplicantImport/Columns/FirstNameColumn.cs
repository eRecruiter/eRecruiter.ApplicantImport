using eRecruiter.Api.Client;
using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;

namespace eRecruiter.ApplicantImport.Columns
{
    public class FirstNameColumn : AbstractColumn
    {
        public FirstNameColumn(string additionalType, string header) : base(ColumnType.FirstName, additionalType, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnNotEvenOnce(configuration) && !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            return HasValue(value);
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant)
        {
            if (value.HasValue()) // only set the first name, if we really have a value (use default value otherwise, because first name must not be empty).
                applicant.FirstName = value;
        }
    }
}
