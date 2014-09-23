using eRecruiter.Api.Client;
using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class ReferrerAdditionalInfoColumn : AbstractColumn
    {
        public ReferrerAdditionalInfoColumn(string header) : base(ColumnType.ReferrerAdditionalInfo, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            if (configuration.Columns.Any(x => x.Type == Type) && configuration.Columns.All(x => x.Type != ColumnType.Referrer))
            {
                Program.WriteWarning("There is a column for 'Referrer Additional Info', but none for 'Referrer'.");
                return false;
            }

            return !HasColumnMoreThanOnce(configuration);
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue())
            {
                applicant.ReferrerAdditionalInfo = value;
            }
        }
    }
}
