using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using JetBrains.Annotations;
using System;
using System.Globalization;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public class CustomFieldColumn : AbstractColumn
    {
        public CustomFieldColumn(string header)
            : base(ColumnType.CustomField, header)
        {
        }

        public override bool IsColumnConfigurationValid(ApiHttpClient apiClient)
        {
            if (!IsCustomFieldAvailable(apiClient))
            {
                Program.WriteWarning("There is no custom field '" + SubType + "'.");
                return false;
            }
            return true;
        }

        public override bool IsValueValid(string value, ApiHttpClient apiClient)
        {
            if (IsCustomFieldAvailable(apiClient) && !IsCustomFieldValueValid(value, apiClient))
            {
                Program.WriteWarning("'" + value + "' is not a valid value for '" + Header + "'.");
                return false;
            }

            return true;
        }

        public override void SetValueAfterCreate(string value, ApplicantResponse applicant, ApiHttpClient apiClient)
        {
            if (IsCustomFieldAvailable(apiClient) && IsCustomFieldValueValid(value, apiClient))
            {
                var customField = GetCustomField(apiClient);
                var parameter = new CustomFieldValueParameter
                {
                    CustomField = customField.Name
                };

                switch (customField.Type)
                {
                    case CustomFieldResponse.CustomFieldType.TextMultiLine:
                    case CustomFieldResponse.CustomFieldType.TextSingleLine:
                    case CustomFieldResponse.CustomFieldType.DropdownList:
                        parameter.Value = value;
                        break;
                    case CustomFieldResponse.CustomFieldType.CheckBox:
                        parameter.Value = value.Is("X").ToString().ToLower();
                        break;
                    case CustomFieldResponse.CustomFieldType.CheckboxList:
                        parameter.Values = (value ?? "").Split(',').Select(x => x.Trim()).Where(x => x.HasValue());
                        break;
                    case CustomFieldResponse.CustomFieldType.Date:
                        var date = ParseDate(value);
                        parameter.Value = date.HasValue ? date.Value.ToString(CultureInfo.InvariantCulture) : null;
                        break;
                }

                new ApplicantCustomFieldPostRequest(applicant.Id, parameter).LoadResult(apiClient);
            }
        }

        private static MandatorResponse _mandator;

        private bool IsCustomFieldAvailable(ApiHttpClient apiClient)
        {
            return GetCustomField(apiClient) != null;
        }

        private CustomFieldResponse GetCustomField(ApiHttpClient apiClient)
        {
            _mandator = _mandator ?? new MandatorRequest().LoadResult(apiClient);
            return _mandator.CustomFields.FirstOrDefault(x => x.Name.Is(SubType) && (x.Target == CustomFieldResponse.CustomFieldTarget.Applicant || x.Target == CustomFieldResponse.CustomFieldTarget.ApplicantCompany || x.Target == CustomFieldResponse.CustomFieldTarget.JobApplicantCompany || x.Target == CustomFieldResponse.CustomFieldTarget.JobApplicant));
        }

        private DateTime? ParseDate(string value)
        {
            DateTime date;
            if (DateTime.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return date;
            return null;
        }

        private bool IsCustomFieldValueValid([CanBeNull] string value, ApiHttpClient apiClient)
        {
            var customField = GetCustomField(apiClient);

            switch (customField.Type)
            {
                case CustomFieldResponse.CustomFieldType.TextMultiLine:
                case CustomFieldResponse.CustomFieldType.TextSingleLine:
                    return true;
                case CustomFieldResponse.CustomFieldType.CheckBox:
                    return value.IsNoE() || value.Is("X");
                case CustomFieldResponse.CustomFieldType.DropdownList:
                    return value.IsNoE() || customField.PossibleValues.Any(x => x.Is(value));
                case CustomFieldResponse.CustomFieldType.CheckboxList:
                    return value.IsNoE() || (value ?? "").Split(',').Select(x => x.Trim()).Where(x => x.HasValue()).All(x => customField.PossibleValues.Any(y => y.Is(x)));
                case CustomFieldResponse.CustomFieldType.Date:
                    return value.IsNoE() || ParseDate(value) != null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
