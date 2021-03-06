﻿using System.Linq;
using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System.IO;

namespace eRecruiter.ApplicantImport.Columns
{
    public class PhotoColumn : AbstractColumn
    {
        public PhotoColumn(string header) : base(ColumnType.Photo, header) { }

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
            var allowedExtensions = new[] { "jpg", "jpeg", "png", "bmp" };
            if (allowedExtensions.All(x => x != extension))
            {
                Program.WriteWarning("The file '" + value + "' in column '" + Header + "' does not have a valid file extension for a photo.");
                return false;
            }

            return true;
        }

        public override void SetValueAfterCreate(string value, ApplicantResponse applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue() && File.Exists(value))
            {
                var bytes = File.ReadAllBytes(value);
                new ApplicantPhotoPutRequest(applicant.Id, new ApplicantPhotoParameter
                {
                    Content = bytes,
                    FileExtension = Path.GetExtension(value)
                }).LoadResult(apiClient);
            }
        }
    }
}
