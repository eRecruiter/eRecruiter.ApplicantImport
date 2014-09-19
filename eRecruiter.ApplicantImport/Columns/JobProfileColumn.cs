using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class JobProfileColumn : AbstractColumn
    {
        public JobProfileColumn(string additionalType, string header) : base(ColumnType.JobProfile, additionalType, header) { }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value != null && value.HasValue() && !IsJobProfileAvailable(value, apiClient))
            {
                Program.WriteWarning("There is no job profile '" + value + "'.");
                return false;
            }

            return true;
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue() && IsJobProfileAvailable(value, apiClient))
            {
                var jobProfiles = (applicant.JobProfiles ?? new List<string>()).ToList();
                jobProfiles.Add(value);
                applicant.JobProfiles = jobProfiles;
            }
        }

        private static MandatorResponse _mandator;
        private bool IsJobProfileAvailable(string value, ApiHttpClient apiClient)
        {
            _mandator = _mandator ?? new MandatorRequest(new Uri("http://does_not_matter")).LoadResult(apiClient);
            return _mandator.JobProfiles.Any(x => x.Is(value));
        }
    }
}
