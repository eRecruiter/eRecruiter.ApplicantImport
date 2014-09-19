using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Utilities;

namespace eRecruiter.ApplicantImport.Columns
{
    public class IdColumn : AbstractColumn
    {
        public IdColumn(string additionalType, string header) : base(ColumnType.FirstName, additionalType, header)
        {
        }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value.HasValue())
            {
                if (!value.IsInt())
                {
                    Program.WriteWarning("'" + value + "' is not a valid applicant #.");
                    return false;
                }

                try
                {
                    new ApplicantGetRequest(value.GetInt()).LoadResult(apiClient);
                    return true;
                }
                catch
                {
                    Program.WriteWarning("Applicant '#" + value + "' not found.");
                    return false;
                }
            }
            return true;
        }
    }
}
