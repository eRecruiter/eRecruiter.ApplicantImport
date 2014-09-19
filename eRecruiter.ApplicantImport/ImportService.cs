using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.ApplicantImport.Columns;
using System;
using System.Linq;

namespace eRecruiter.ApplicantImport
{
    public class ImportService
    {
        private readonly Configuration _configuration;
        private readonly Csv _csv;

        public ImportService(Configuration configuration, Csv csv)
        {
            _csv = csv;
            _configuration = configuration;
        }

        public void RunImport(out bool hasErrors)
        {
            hasErrors = false;

            var apiClient = ApiClientFactory.GetClient(_configuration);

            var count = 0;
            var total = _csv.Values.Count();
            foreach (var row in _csv.Values)
            {
                Program.Write(string.Format("Importing applicant {0}/{1} ...", ++count, total));
                var applicantParameter = new ApplicantParameter
                {
                    FirstName = "First-Name",
                    LastName = "Last-Name",
                    IsActive = true
                };

                try
                {
                    foreach (var c in _configuration.Columns)
                    {
                        var column = ColumnFactory.GetColumn(c.Type, c.AdditionalType, c.Header);
                        column.SetValueBeforeCreate(row.ContainsKey(c.Header) ? row[c.Header] as string : null, applicantParameter, apiClient);
                    }

                    var applicantResponse = new ApplicantPutRequest(applicantParameter, false, new Uri("http://does_not_matter")).LoadResult(apiClient);
                    Program.Write(string.Format("Applicant '#{0} {1} {2}' created. Setting additional attributes ...", applicantResponse.Id, applicantResponse.FirstName, applicantResponse.LastName));

                    foreach (var c in _configuration.Columns)
                    {
                        var column = ColumnFactory.GetColumn(c.Type, c.AdditionalType, c.Header);
                        column.SetValueAfterCreate(row.ContainsKey(c.Header) ? row[c.Header] as string : null, applicantResponse, apiClient);
                    }

                    Program.Write("");
                }
                catch (Exception ex)
                {
                    Program.WriteError("Unable to import applicant: " + ex.Message);
                    hasErrors = true;
                    return;
                }
            }
        }
    }
}
