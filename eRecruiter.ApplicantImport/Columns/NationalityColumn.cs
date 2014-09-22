using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class NationalityColumn : AbstractColumn
    {
        public NationalityColumn(string header) : base(ColumnType.Nationality, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value.HasValue() && !IsNationalityAvailable(value, apiClient))
            {
                Program.WriteWarning("There is no nationality '" + value + "'.");
                return false;
            }

            return true;
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue() && IsNationalityAvailable(value, apiClient))
            {
                applicant.Citizenship = value;
            }
        }

        private static MandatorResponse _mandator;
        private bool IsNationalityAvailable(string value, ApiHttpClient apiClient)
        {
            _mandator = _mandator ?? new MandatorRequest().LoadResult(apiClient);
            return _mandator.Countries.Any(x => x.Is(value));
        }
    }
}
