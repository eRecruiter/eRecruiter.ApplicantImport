﻿using eRecruiter.Api.Client;
using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;

namespace eRecruiter.ApplicantImport.Columns
{
    public class LastNameColumn : AbstractColumn
    {
        public LastNameColumn(string additionalType, string header) : base(ColumnType.LastName, additionalType, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnNotEvenOnce(configuration) && !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            return HasValue(value);
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant)
        {
            if (value.HasValue()) // only set the last name, if we really have a value (use default value otherwise, because last name must not be empty).
                applicant.LastName = value;
        }
    }
}
