using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System.IO;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class CvColumn : AbstractColumn
    {
        public CvColumn(string header) : base(ColumnType.Cv, header) { }

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

            var extension = (Path.GetExtension(value) ?? "").Trim('.').ToLowerInvariant();
            var allowedExtensions = new[] { "doc", "docx", "pdf", "txt", "rtf", "odt", "xls", "xlsx" };
            if (allowedExtensions.All(x => x != extension))
            {
                Program.WriteWarning("The file '" + value + "' in column '" + Header + "' does not have a valid file extension for a CV.");
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
                    Name = Path.GetFileName(value),
                    FileExtension = Path.GetExtension(value)
                }).LoadResult(apiClient);
            }
        }
    }
}
