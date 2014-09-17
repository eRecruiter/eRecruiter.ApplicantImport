using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System.IO;

namespace eRecruiter.ApplicantImport.Columns
{
    public class CvColumn : AbstractColumn
    {
        public CvColumn(string additionalType, string header) : base(ColumnType.Cv, additionalType, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value.HasValue() && !File.Exists(value))
            {
                Program.WriteWarning("The file '" + value + "' in column '" + Header + "' does not exist or is not accessible.");
                return false;
            }
            return true;
        }

        public override void SetValueAfterCreate(string value, ApplicantResponse applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue() && File.Exists(value))
            {
                var bytes = File.ReadAllBytes(value);
                new ApplicantCvPutRequest(applicant.Id, new ApplicantDocumentParameter
                {
                    Content = bytes,
                    Name = value,
                    FileExtension = Path.GetExtension(value)
                }).LoadResult(apiClient);
            }
        }
    }
}
