using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class KnowledgeColumn : AbstractColumn
    {
        public KnowledgeColumn(string header)
            : base(ColumnType.Knowledge, header)
        {
        }

        public override bool IsColumnConfigurationValid(ApiHttpClient apiClient)
        {
            if (SubType.IsNoE())
            {
                Program.WriteWarning("No subtype specified for column '" + Header + "'.");
                return false;
            }

            if (!IsKnowledgeAvailable(apiClient))
            {
                Program.WriteWarning("There is no knowledge '" + SubType + "'.");
                return false;
            }

            return true;
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (!IsKnowledgeLevelAvailable(value, apiClient))
            {
                Program.WriteWarning("There is no knowledge level '" + value + "'.");
                return false;
            }

            return true;
        }

        public override void SetValueAfterCreate(string value, ApplicantResponse applicant, ApiHttpClient apiClient)
        {
            if (IsKnowledgeAvailable(apiClient) && IsKnowledgeLevelAvailable(value, apiClient))
            {
                new ApplicantKnowledgePutRequest(applicant.Id, new ApplicantKnowledgeParameter
                {
                    Knowledge = SubType,
                    Level = value
                }).LoadResult(apiClient);
            }
        }

        private static MandatorResponse _mandator;

        private bool IsKnowledgeAvailable(ApiHttpClient apiClient)
        {
            if (SubType.IsNoE())
                return false;

            _mandator = _mandator ?? new MandatorRequest(new Uri("http://does_not_matter")).LoadResult(apiClient);
            return _mandator.Knowledges.Any(x => x.Is(SubType));
        }

        private bool IsKnowledgeLevelAvailable(string value, ApiHttpClient apiClient)
        {
            if (value.IsNoE())
                return false;

            _mandator = _mandator ?? new MandatorRequest(new Uri("http://does_not_matter")).LoadResult(apiClient);
            return _mandator.KnowledgeLevels.Any(x => x.Is(value));
        }
    }
}
