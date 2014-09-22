using System;
using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System.IO;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class DocumentColumn : AbstractColumn
    {
        public DocumentColumn(string header) : base(ColumnType.Document, header) { }

        public override bool IsColumnConfigurationValid(ApiHttpClient apiClient)
        {
            if (SubType.IsNoE())
            {
                Program.WriteWarning("No subtype specified for column '" + Header + "'.");
                return false;
            }

            if (!IsDocumentTypeAvailable(apiClient))
            {
                Program.WriteWarning("There is no document type '" + SubType + "'.");
                return false;
            }

            return true;
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value != null && value.HasValue() && !File.Exists(value) && !Directory.Exists(value))
            {
                Program.WriteWarning("The file or directory '" + value + "' in column '" + Header + "' does not exist or is not accessible.");
                return false;
            }

            return true;
        }

        public override void SetValueAfterCreate(string value, ApplicantResponse applicant, ApiHttpClient apiClient)
        {
            if (value != null && value.HasValue() && IsDocumentTypeAvailable(apiClient))
            {
                if (File.Exists(value))
                {
                    SetFile(value, applicant, apiClient);
                }
                else if (Directory.Exists(value))
                {
                    foreach (var file in Directory.GetFiles(value))
                        SetFile(file, applicant, apiClient);
                }
            }
        }

        private void SetFile(string filePath, ApplicantResponse applicant, ApiHttpClient apiClient)
        {
            var bytes = File.ReadAllBytes(filePath);
            new ApplicantDocumentPutRequest(applicant.Id, new ApplicantDocumentParameter
            {
                Content = bytes,
                Name = Path.GetFileName(filePath),
                FileExtension = Path.GetExtension(filePath),
                Type = SubType,
                IsPublic = false
            }).LoadResult(apiClient);
        }


        private static MandatorResponse _mandator;
        private bool IsDocumentTypeAvailable(ApiHttpClient apiClient)
        {
            if (SubType.IsNoE())
                return false;

            _mandator = _mandator ?? new MandatorRequest().LoadResult(apiClient);
            return _mandator.ApplicantDocumentTypes.Any(x => x.Is(SubType));
        }
    }
}
