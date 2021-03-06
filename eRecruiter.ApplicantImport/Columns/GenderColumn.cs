﻿using eRecruiter.Api;
using eRecruiter.Api.Client;
using eRecruiter.Api.Parameters;
using eRecruiter.Utilities;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class GenderColumn : AbstractColumn
    {
        private readonly string[] _maleValues = { "H", "M", "Herr", "Mr.", "1" };
        private readonly string[] _femaleValues = { "F", "W", "Frau", "Ms.", "Mrs.", "0" };

        public GenderColumn(string header) : base(ColumnType.Gender, header) { }

        public override bool IsEntireConfigurationValid(Configuration configuration)
        {
            return !HasColumnMoreThanOnce(configuration);
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            return value.IsNoE() || IsValueInList(value, _maleValues.Concat(_femaleValues));
        }

        public override void SetValueBeforeCreate(string value, ApplicantParameter applicant, ApiHttpClient apiClient)
        {
            if (value.HasValue())
            {
                var gender = Gender.Unknown;
                if (_maleValues.Any(x => x.Is(value)))
                    gender = Gender.Male;
                else if (_femaleValues.Any(x => x.Is(value)))
                    gender = Gender.Female;
                applicant.Gender = gender;
            }
        }
    }
}
