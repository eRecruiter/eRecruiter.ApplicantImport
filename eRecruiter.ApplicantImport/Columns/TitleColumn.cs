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
    public class TitleColumn : AbstractColumn
    {
        public TitleColumn(string header) : base(ColumnType.Title, header) { }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value.HasValue() && !IsTitleAvailable(value, apiClient))
            {
                Program.WriteWarning("There is no title '" + value + "'.");
                return false;
            }

            return true;
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue())
            {
                if (IsTitleAfterNameAvailable(value, apiClient))
                    applicant.TitleAfterName = value;
                else if (IsTitleBeforeNameAvailable(value, apiClient))
                    applicant.TitleBeforeName = value;
            }
        }

        private static MandatorResponse _mandator;
        private bool IsTitleBeforeNameAvailable(string value, ApiHttpClient apiClient)
        {
            return GetTitles(true, apiClient).Any(x => x.Is(value));
        }
        private bool IsTitleAfterNameAvailable(string value, ApiHttpClient apiClient)
        {
            return GetTitles(false, apiClient).Any(x => x.Is(value));
        }
        private bool IsTitleAvailable(string value, ApiHttpClient apiClient)
        {
            return IsTitleBeforeNameAvailable(value, apiClient) || IsTitleAfterNameAvailable(value, apiClient);
        }

        private IEnumerable<string> GetTitles(bool beforeName, ApiHttpClient apiClient)
        {
            _mandator = _mandator ?? new MandatorRequest(new Uri("http://does_not_matter")).LoadResult(apiClient);
            if (beforeName)
                return _mandator.TitlesBeforeName.Select(x => x.Name);
            return _mandator.TitlesAfterName.Select(x => x.Name);
        }
    }
}
