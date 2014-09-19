﻿using System;
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
        public DocumentColumn(string additionalType, string header) : base(ColumnType.Document, additionalType, header) { }

        public override bool IsColumnConfigurationValid(ApiHttpClient apiClient)
        {
            if (AdditionalType.IsNoE())
            {
                Program.WriteWarning("No additional type specified for column '" + Header + "'.");
                return false;
            }

            if (!IsDocumentTypeAvailable(apiClient))
            {
                Program.WriteWarning("There is no document type '" + AdditionalType + "'.");
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
            if (value != null && value.HasValue() && File.Exists(value))
            {
                SetFile(value, applicant, apiClient);
            }
            else if (value != null && value.HasValue() && Directory.Exists(value))
            {
                foreach (var file in Directory.GetFiles(value))
                    SetFile(file, applicant, apiClient);
            }
        }

        private void SetFile(string filePath, ApplicantResponse applicant, ApiHttpClient apiClient)
        {
            if (IsDocumentTypeAvailable(apiClient))
            {
                var bytes = File.ReadAllBytes(filePath);
                new ApplicantDocumentPutRequest(applicant.Id, new ApplicantDocumentParameter
                {
                    Content = bytes,
                    Name = Path.GetFileName(filePath),
                    FileExtension = Path.GetExtension(filePath),
                    Type = AdditionalType
                }).LoadResult(apiClient);
            }
        }


        private static MandatorResponse _mandator;
        private bool IsDocumentTypeAvailable(ApiHttpClient apiClient)
        {
            _mandator = _mandator ?? new MandatorRequest(new Uri("http://does_not_matter")).LoadResult(apiClient);
            return _mandator.ApplicantDocumentTypes.Any(x => x.Is(AdditionalType));
        }
    }
}