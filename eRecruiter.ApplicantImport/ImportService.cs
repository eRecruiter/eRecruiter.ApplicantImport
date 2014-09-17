using ePunkt.Api.Client.Requests;
using ePunkt.Api.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

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

                try
                {
                    var applicantRequest = BuildApplicant(row);
                    new ApplicantPutRequest(applicantRequest, false, new Uri("http://does_not_matter")).LoadResult(apiClient);
                }
                catch (Exception ex)
                {
                    Program.WriteError("Unable to import applicant: " + ex.Message);
                    hasErrors = true;
                    return;
                }
            }
        }

        [NotNull]
        private ApplicantParameter BuildApplicant(IDictionary<string, object> row)
        {
            var applicant = new ApplicantParameter
            {
                FirstName = "First Name",
                LastName = "Last Name"
            };

            applicant.FirstName = FindValue(row, Configuration.Column.ColumnType.FirstName);
            applicant.LastName = FindValue(row, Configuration.Column.ColumnType.LastName);
            applicant.Email = FindValue(row, Configuration.Column.ColumnType.Email);
            applicant.Phone = FindValue(row, Configuration.Column.ColumnType.Phone);
            applicant.MobilePhone = FindValue(row, Configuration.Column.ColumnType.MobilePhone);
            applicant.Street = FindValue(row, Configuration.Column.ColumnType.Street);
            applicant.ZipCode = FindValue(row, Configuration.Column.ColumnType.ZipCode);
            applicant.City = FindValue(row, Configuration.Column.ColumnType.City);

            return applicant;
        }

        [CanBeNull]
        private string FindValue(IDictionary<string, object> row, Configuration.Column.ColumnType type)
        {
            var column = _configuration.Columns.FirstOrDefault(x => x.Type == Configuration.Column.ColumnType.FirstName);
            if (column != null)
            {
                var value = row.ContainsKey(column.Header) ? row[column.Header] : null;
                if (value != null)
                    return value.ToString();
            }
            return null;
        }
    }
}
