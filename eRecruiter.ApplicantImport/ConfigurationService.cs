using ePunkt.Api.Client.Requests;
using JetBrains.Annotations;
using System;
using System.IO;
using System.Linq;

namespace eRecruiter.ApplicantImport
{
    public class ConfigurationService
    {
        private readonly CommandLineArguments _commandLineArguments;

        public ConfigurationService([NotNull] CommandLineArguments commandLineArguments)
        {
            _commandLineArguments = commandLineArguments;
        }

        [NotNull]
        public Configuration ReadAndVerify(out bool hasErrors, out bool hasWarnings)
        {
            hasErrors = false;
            hasWarnings = false;
            Configuration configuration;

            // parse the JSON and check if we get a filled configuration back
            try
            {
                configuration = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(_commandLineArguments.ConfigurationFile));
                if (configuration == null || configuration.Api == null || configuration.Columns == null)
                    throw new ApplicationException("Configuration invalid or missing.");
            }
            catch (Exception ex)
            {
                Program.WriteError("Unable to deserialize JSON: " + ex.Message);
                configuration = new Configuration();
                hasErrors = true;
                return configuration;
            }

            var errorFunctions = new Func<Configuration, bool>[]
            {
                EndpointIsValid,
                CanReachEndpoint,
                CanAuthenticateEndpoint
            };
            if (errorFunctions.Any(function => !function.Invoke(configuration)))
            {
                hasErrors = true;
                return configuration;
            }

            var warningFunctions = new Func<Configuration, bool>[]
            {
                LastNameAtLeastOnce,
                FirstNameAtLeastOnce,
                ColumnOnlyOnce
            };
            if (warningFunctions.Any(function => !function.Invoke(configuration)))
            {
                hasWarnings = true;
            }

            return configuration;
        }

        #region Error Validation
        private bool EndpointIsValid(Configuration configuration)
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Uri(configuration.Api.Endpoint);
            }
            catch
            {
                Program.WriteError(string.Format("{0} is not a valid endpoint URI.", configuration.Api.Endpoint));
                return false;
            }
            return true;
        }

        private bool CanReachEndpoint(Configuration configuration)
        {
            var apiClient = ApiClientFactory.GetClient(configuration);
            try
            {
                new PingRequest().LoadResult(apiClient);
            }
            catch (Exception ex)
            {
                Program.WriteError("API endpoint not found: " + ex.Message);
                return false;
            }
            return true;
        }

        private bool CanAuthenticateEndpoint(Configuration configuration)
        {
            var apiClient = ApiClientFactory.GetClient(configuration);
            try
            {
                new MandatorRequest(new Uri("http://does_not_matter")).LoadResult(apiClient);
            }
            catch (Exception ex)
            {
                Program.WriteError("Unable to access API: " + ex.Message);
                return false;
            }
            return true;
        }
        #endregion

        #region Warning Validation

        public bool ColumnOnlyOnce(Configuration configuration)
        {
            var types = new[]
            {
                Configuration.Column.ColumnType.LastName,
                Configuration.Column.ColumnType.FirstName,
                Configuration.Column.ColumnType.Email,
                Configuration.Column.ColumnType.Phone,
                Configuration.Column.ColumnType.MobilePhone,
                Configuration.Column.ColumnType.Street,
                Configuration.Column.ColumnType.ZipCode,
                Configuration.Column.ColumnType.City
            };

            foreach (var type in types)
                if (configuration.Columns.Count(x => x.Type == type) > 1)
                {
                    Program.WriteWarning("Column for " + type + " specified more than once.");
                    return false;
                }
            return true;
        }

        public bool LastNameAtLeastOnce(Configuration configuration)
        {
            if (configuration.Columns.Count(x => x.Type == Configuration.Column.ColumnType.LastName) <= 0)
            {
                Program.WriteWarning("Column for LastName not specified at all.");
                return false;
            }
            return true;
        }

        public bool FirstNameAtLeastOnce(Configuration configuration)
        {
            if (configuration.Columns.Count(x => x.Type == Configuration.Column.ColumnType.FirstName) <= 0)
            {
                Program.WriteWarning("Column for FirstName not specified at all.");
                return false;
            }
            return true;
        }

        #endregion
    }
}
