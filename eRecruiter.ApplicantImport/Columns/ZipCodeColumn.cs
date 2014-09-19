using eRecruiter.Api.Client;
using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;

namespace eRecruiter.ApplicantImport.Columns
{
    public class ZipCodeColumn : AbstractColumn
    {
        public ZipCodeColumn(string header) : base(ColumnType.ZipCode, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue())
                applicant.ZipCode = value;
        }
    }
}
