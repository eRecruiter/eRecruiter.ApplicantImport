﻿using System.Text;
using eRecruiter.Api.Client;
using eRecruiter.Api.Parameters;
using eRecruiter.Api.Responses;
using eRecruiter.Utilities;
using JetBrains.Annotations;
using System.Linq;

namespace eRecruiter.ApplicantImport.Columns
{
    public abstract class AbstractColumn
    {
        protected AbstractColumn(ColumnType type, string additionalType, string header)
        {
            Type = type;
            Header = header;
            AdditionalType = additionalType;
        }

        public ColumnType Type { get; set; }
        public string AdditionalType { get; set; }
        public string Header { get; set; }

        public virtual bool IsEntireConfigurationValid([NotNull] Configuration configuration)
        {
            return true;
        }

        public virtual bool IsColumnConfigurationValid([NotNull] ApiHttpClient apiClient)
        {
            return true;
        }

        public virtual bool IsValueValid([CanBeNull] string value, [NotNull] ApiHttpClient apiClient)
        {
            return true;
        }

        public virtual void SetValueBeforeCreate([CanBeNull] string value, [NotNull] ApplicantParameter applicant)
        {
        }

        public virtual void SetValueAfterCreate([CanBeNull] string value, [NotNull] ApplicantResponse applicant, [NotNull] ApiHttpClient apiClient)
        {
        }

        protected bool HasColumnMoreThanOnce(Configuration configuration)
        {
            if (configuration.Columns.Count(x => x.Type == Type) > 1)
            {
                Program.WriteWarning("Column for '" + Type + "' specified more than once.");
                return true;
            }
            return false;
        }

        protected bool HasColumnNotEvenOnce(Configuration configuration)
        {
            if (configuration.Columns.Count(x => x.Type == Type) <= 0)
            {
                Program.WriteWarning("Column for '" + Type + "' not specified at all.");
                return true;
            }
            return false;
        }

        protected bool HasValue(string value)
        {
            if (!value.HasValue())
            {
                Program.WriteWarning("Empty value in column '" + Header + "' of type '" + Type + "' not allowed.");
                return false;
            }
            return true;
        }
    }
}