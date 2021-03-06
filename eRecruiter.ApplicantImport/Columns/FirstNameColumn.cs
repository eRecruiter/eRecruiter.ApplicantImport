﻿using eRecruiter.Api.Client;
using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;

namespace eRecruiter.ApplicantImport.Columns
{
    public class FirstNameColumn : AbstractColumn
    {
        public FirstNameColumn(string header) : base(ColumnType.FirstName, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnNotEvenOnce(configuration) && !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            return HasValue(value);
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue())
                applicant.FirstName = value;
        }
    }
}
