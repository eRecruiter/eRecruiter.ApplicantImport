using System.Linq;
using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System;

namespace eRecruiter.ApplicantImport.Columns
{
    public class ClassificationColumn : AbstractColumn
    {
        public ClassificationColumn(string header) : base(ColumnType.Classification, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value.HasValue() && !IsClassificationAvailable(value, apiClient))
            {
                Program.WriteWarning("There is no classification '" + value + "'.");
                return false;
            }

            return true;
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue() && IsClassificationAvailable(value, apiClient))
            {
                applicant.Classification = value;
            }
        }

        private static MandatorResponse _mandator;
        private bool IsClassificationAvailable(string value, ApiHttpClient apiClient)
        {
            _mandator = _mandator ?? new MandatorRequest().LoadResult(apiClient);
            return _mandator.ClassificationTypes.Any(x => x.Is(value));
        }
    }
}
