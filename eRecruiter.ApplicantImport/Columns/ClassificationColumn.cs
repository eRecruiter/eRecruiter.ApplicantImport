using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class CareerlevelColumn : AbstractColumn
    {
        public CareerlevelColumn(string header) : base(ColumnType.Careerlevel, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value.HasValue() && !IsCareerlevelAvailable(value, apiClient))
            {
                Program.WriteWarning("There is no career level '" + value + "'.");
                return false;
            }

            return true;
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue() && IsCareerlevelAvailable(value, apiClient))
            {
                applicant.CareerLevel = value;
            }
        }

        private static MandatorResponse _mandator;
        private bool IsCareerlevelAvailable(string value, ApiHttpClient apiClient)
        {
            _mandator = _mandator ?? new MandatorRequest(new Uri("http://does_not_matter")).LoadResult(apiClient);
            return _mandator.CareerLevels.Any(x => x.Is(value));
        }
    }
}
