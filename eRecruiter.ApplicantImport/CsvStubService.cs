using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using CommandLine;
using CsvHelper;
using System.IO;

namespace eRecruiter.ApplicantImport
{
    public class CsvStubService
    {
        private readonly Configuration _configuration;

        public CsvStubService(Configuration configuration)
        {
            _configuration = configuration;
        }

        public void GenerateCsvStub(string targetPath)
        {
            Program.Write("Generating CSV stub ...");
            using (var stream = new StreamWriter(targetPath, false, Encoding.UTF8))
            {
                var writer = new CsvWriter(stream, CsvService.GetDefaultCsvConfiguration());

                var dummyLine = new Dictionary<string, string>();
                foreach (var column in _configuration.Columns)
                {
                    dummyLine[column.Header] = "";
                }

                foreach (var field in dummyLine.Keys)
                    writer.WriteField(field);
                writer.NextRecord();
                foreach (var field in dummyLine.Keys)
                    writer.WriteField(dummyLine[field]);
            }

            Program.Write("CSV stub saved at '" + targetPath + "'.");
        }
    }
}
