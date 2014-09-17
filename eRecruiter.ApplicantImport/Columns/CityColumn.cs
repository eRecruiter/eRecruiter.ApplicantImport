using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;

namespace eRecruiter.ApplicantImport.Columns
{
    public class CityColumn : AbstractColumn
    {
        public CityColumn(string additionalType, string header) : base(ColumnType.City, additionalType, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant)
        {
            if (value.HasValue())
                applicant.City = value;
        }
    }
}
