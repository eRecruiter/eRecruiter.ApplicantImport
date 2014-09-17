using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;

namespace eRecruiter.ApplicantImport.Columns
{
    public class ZipCodeColumn : AbstractColumn
    {
        public ZipCodeColumn(string additionalType, string header) : base(ColumnType.ZipCode, additionalType, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant)
        {
            if (value.HasValue())
                applicant.ZipCode = value;
        }
    }
}
