using eRecruiter.ApplicantImport.Columns;
using System.Collections.Generic;

namespace eRecruiter.ApplicantImport
{
    public class Configuration
    {
        public ApiConfiguration Api { get; set; }
        public IEnumerable<Column> Columns { get; set; }

        public class ApiConfiguration
        {
            public string Endpoint { get; set; }
            public int MandatorId { get; set; }
            public string Key { get; set; }
        }

        public class Column
        {
            public string Header { get; set; }
            public ColumnType Type { get; set; }
            public string AdditionalType { get; set; }
        }
    }
}
