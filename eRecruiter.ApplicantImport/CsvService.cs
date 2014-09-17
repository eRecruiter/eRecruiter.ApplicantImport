using CsvHelper;
using CsvHelper.Configuration;
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

        public CsvService([NotNull] CommandLineArguments commandLineArguments, [NotNull] Configuration configuration)
        {
            _commandLineArguments = commandLineArguments;
            _configuration = configuration;
        }

        [CanBeNull]
        public Csv ReadAndVerify(out bool hasError, out bool hasWarnings)
        {
            hasError = false;
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
                hasError = true;
                return null;
            }

            var errorFunctions = new Func<Csv, bool>[]
            {
            };
            if (errorFunctions.Any(function => !function.Invoke(csv)))
            {
                hasError = true;
                return csv;
            }

            var warningFunctions = new Func<Csv, bool>[]
            {
                HasAtLeastTwoColumns,
                HasRecords
            };
            if (warningFunctions.Any(function => !function.Invoke(csv)))
            {
                hasWarnings = true;
            }

            return csv;
        }

        private bool HasRecords(Csv csv)
        {
            if (!csv.Values.Any())
            {
                Program.WriteWarning("No applicants found.");
                return false;
            }
            return true;
        }

        private bool HasAtLeastTwoColumns(Csv csv)
        {
            if (csv.Headers.Count() <= 1)
            {
                Program.WriteWarning("Less than two columns.");
                return false;
            }
            return true;
        }

    }
}
