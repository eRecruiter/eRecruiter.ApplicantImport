using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class ReferrerColumn : AbstractColumn
    {
        public ReferrerColumn(string header) : base(ColumnType.Referrer, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value.HasValue() && !IsReferrerAvailable(value, apiClient))
            {
                Program.WriteWarning("There is no referrer '" + value + "'.");
                return false;
            }

            return true;
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue() && IsReferrerAvailable(value, apiClient))
            {
                applicant.Referrer = value;
            }
        }

        private static MandatorResponse _mandator;
        private bool IsReferrerAvailable(string value, ApiHttpClient apiClient)
        {
            _mandator = _mandator ?? new MandatorRequest().LoadResult(apiClient);
            return _mandator.Referrers.Any(x => x.Name.Is(value));
        }
    }
}
