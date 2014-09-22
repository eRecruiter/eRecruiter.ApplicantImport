using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class RegionColumn : AbstractColumn
    {
        public RegionColumn(string header) : base(ColumnType.JobProfile, header)
        {
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (value.HasValue() && !IsRegionAvailable(value, apiClient))
            {
                Program.WriteWarning("There is no region '" + value + "'.");
                return false;
            }
            return true;
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue() && IsRegionAvailable(value, apiClient))
            {
                var regions = (applicant.Regions ?? new List<string>()).ToList();
                regions.Add(value);
                applicant.Regions = regions;
            }
        }

        private static MandatorResponse _mandator;

        private bool IsRegionAvailable(string value, ApiHttpClient apiClient)
        {
            _mandator = _mandator ?? new MandatorRequest().LoadResult(apiClient);
            return IsRegionAvailable(value, _mandator.Regions);
        }

        private bool IsRegionAvailable(string value, IEnumerable<RegionResponse> regions)
        {
            // ReSharper disable PossibleMultipleEnumeration
            if (regions.Any(x => x.Name.Is(value)))
                return true;
            if (regions.Any(x => IsRegionAvailable(value, x.Regions)))
                return true;
            return false;
            // ReSharper restore PossibleMultipleEnumeration}
        }
    }
}