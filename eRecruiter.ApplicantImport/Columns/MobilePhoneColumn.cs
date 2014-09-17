using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;

namespace eRecruiter.ApplicantImport.Columns
{
    public class MobilePhoneColumn : AbstractColumn
    {
        public MobilePhoneColumn(string additionalType, string header) : base(ColumnType.MobilePhone, additionalType, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant)
        {
            if (value.HasValue())
                applicant.MobilePhone = value;
        }
    }
}
