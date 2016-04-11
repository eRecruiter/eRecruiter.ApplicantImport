using eRecruiter.Api;
using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;

namespace eRecruiter.ApplicantImport.Columns
{
    public class HistoryColumn : AbstractColumn
    {
        public HistoryColumn(string header) : base(ColumnType.History, header)
        {
        }

        public override void SetValueAfterCreate(string value, ApplicantResponse applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue())
            {
                new ApplicantHistoryPutRequest(applicant.Id, BuildHistoryParameter(value)).LoadResult(apiClient);
            }
        }

        private ApplicantHistoryParameter BuildHistoryParameter(string body)
        {
            return new ApplicantHistoryParameter
            {
                Type = HistoryType.Import,
                Subject = Header,
                Body = body,
                CreatedBy = "Importer"
            };
        }
    }
}
