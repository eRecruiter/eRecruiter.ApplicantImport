using JetBrains.Annotations;
using System.Collections.Generic;

namespace eRecruiter.ApplicantImport
{
    public class Csv
    {
        public Csv()
        {
            Headers = new string[0];
            Values = new List<Dictionary<string, object>>();
        }

        [NotNull]
        public IEnumerable<string> Headers { get; set; }

        [NotNull]
        public IEnumerable<IDictionary<string, object>> Values { get; set; }
    }
}
