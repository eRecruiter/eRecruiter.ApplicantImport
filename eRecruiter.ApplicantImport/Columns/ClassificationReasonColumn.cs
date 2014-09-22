using eRecruiter.Api.Client;
using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class ClassificationReasonColumn : AbstractColumn
    {
        public ClassificationReasonColumn(string header) : base(ColumnType.ClassificationReason, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            if (configuration.Columns.All(x => x.Type != ColumnType.Classification))
            {
                Program.WriteWarning("There is a column for 'Classification Reason', but none for 'Classification'.");
                return false;
            }

            return !HasColumnMoreThanOnce(configuration);
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue())
            {
                applicant.ClassificationReason = value;
            }
        }
    }
}
