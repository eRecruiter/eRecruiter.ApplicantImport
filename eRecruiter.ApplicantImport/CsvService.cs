using CsvHelper;
using CsvHelper.Configuration;
using eRecruiter.Api.Client;
using eRecruiter.ApplicantImport.Columns;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace eRecruiter.ApplicantImport
{
    public class CsvService
    {
        private readonly Configuration _configuration;
        private readonly CommandLineArguments _commandLineArguments;
        private readonly ApiHttpClient _apiClient;

        public CsvService([NotNull] CommandLineArguments commandLineArguments, [NotNull] Configuration configuration)
        {
            _commandLineArguments = commandLineArguments;
            _configuration = configuration;
            _apiClient = ApiClientFactory.GetClient(configuration);
        }

        [CanBeNull]
        public Csv ReadAndVerify(out bool hasErrors, out bool hasWarnings)
        {
            hasWarnings = false;

            // read the csv file
            Csv csv;
            try
            {
                var csvConfiguration = new CsvConfiguration
                {
                    Delimiter = "\t",
                    HasHeaderRecord = true,
                    TrimFields = true,
                    TrimHeaders = true,
                    SkipEmptyRecords = true
                };

                var reader = new CsvReader(new StreamReader(_commandLineArguments.CsvFile, Encoding.UTF8), csvConfiguration);
                csv = new Csv
                {
                    Values = reader.GetRecords<dynamic>().Select(x => x as IDictionary<string, object>).ToList(),
                    Headers = reader.FieldHeaders
                };
            }
            catch (Exception ex)
            {
                Program.WriteError("Unable to read CSV: " + ex.Message);
                hasErrors = true;
                return null;
            }


            hasErrors = !IsCsvValid(csv);
            if (hasErrors)
                return csv;

            hasWarnings = !AreValuesValid(csv);
            return csv;
        }

        private bool IsCsvValid(Csv csv)
        {
            if (!csv.Values.Any())
            {
                Program.WriteWarning("No applicants found.");
                return false;
            }


            if (csv.Headers.Count() <= 1)
            {
                Program.WriteWarning("Less than two columns.");
                return false;
            }
            return true;
        }

        private bool AreValuesValid(Csv csv)
        {
            var result = true;
            foreach (var row in csv.Values)
            {
                foreach (var c in _configuration.Columns)
                {
                    var column = ColumnFactory.GetColumn(c);
                    result = column.IsValueValid(row.ContainsKey(c.Header) ? row[c.Header] as string : null, _apiClient) && result;
                }
            }
            return result;
        }
    }
}
