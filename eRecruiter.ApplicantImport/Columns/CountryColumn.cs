using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class CountryColumn : AbstractColumn
    {
        public CountryColumn(string header) : base(ColumnType.Country, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value.HasValue() && !IsCountryAvailable(value, apiClient))
            {
                Program.WriteWarning("There is no country '" + value + "'.");
                return false;
            }

            return true;
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue() && IsCountryAvailable(value, apiClient))
            {
                applicant.Country = value;
            }
        }

        private static MandatorResponse _mandator;
        private bool IsCountryAvailable(string value, ApiHttpClient apiClient)
        {
            _mandator = _mandator ?? new MandatorRequest().LoadResult(apiClient);
            return _mandator.Countries.Any(x => x.Is(value));
        }
    }
}
