using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using eRecruiter.Api.Client;
using eRecruiter.ApplicantImport.Columns;
using eRecruiter.Utilities;
using JetBrains.Annotations;

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
                var reader = new CsvReader(new StreamReader(_commandLineArguments.CsvFile, Encoding.UTF8),
                    GetDefaultCsvConfiguration());
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
            {
                return csv;
            }

            hasWarnings = !AreAllColumnsFromConfigurationInCsv(csv);
            hasWarnings = !AreAllColumnsFromCsvInConfiguration(csv) || hasWarnings;
            hasWarnings = !AreValuesValid(csv) || hasWarnings;

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
                    result =
                        column.IsValueValid(row.ContainsKey(c.Header) ? row[c.Header] as string : null, _apiClient) &&
                        result;
                }
            }
            return result;
        }

        private bool AreAllColumnsFromConfigurationInCsv(Csv csv)
        {
            var result = true;
            foreach (var column in _configuration.Columns)
            {
                if (!csv.Headers.Any(x => x.Is(column.Header)))
                {
                    Program.WriteWarning("The column '" + column.Header +
                                         "' is specified in configuration, but not found in CSV.");
                    result = false;
                }
            }
            return result;
        }

        private bool AreAllColumnsFromCsvInConfiguration(Csv csv)
        {
            var result = true;
            foreach (var column in csv.Headers)
            {
                if (!_configuration.Columns.Any(x => x.Header.Is(column)))
                {
                    Program.WriteWarning("The column '" + column +
                                         "' is found in CSV, but not specified in configuration.");
                    result = false;
                }
            }
            return result;
        }

        public static CsvConfiguration GetDefaultCsvConfiguration()
        {
            return new CsvConfiguration
            {
                Delimiter = "\t",
                HasHeaderRecord = true,
                TrimFields = true,
                TrimHeaders = true,
                SkipEmptyRecords = true
            };
        }
    }
}
