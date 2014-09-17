using System.Linq;
using eRecruiter.Api.Client;
using eRecruiter.Api.Client.Requests;
using eRecruiter.ApplicantImport.Columns;
using JetBrains.Annotations;
using System;
using System.IO;

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
            hasWarnings = false;
            Configuration configuration;

            // parse the JSON and check if we get a filled configuration back
            try
            {
                configuration = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(_commandLineArguments.ConfigurationFile));
                if (configuration == null
                    || configuration.Api == null
                    || configuration.Columns == null
                    || configuration.Columns.Any(x => x == null))
                    throw new ApplicationException("Configuration invalid or missing.");
            }
            catch (Exception ex)
            {
                Program.WriteError("Unable to deserialize JSON: " + ex.Message);
                configuration = new Configuration();
                hasErrors = true;
                return configuration;
            }

            hasErrors = !IsEndpointValid(configuration);
            if (hasErrors)
                return configuration;

            var apiClient = ApiClientFactory.GetClient(configuration);
            hasWarnings = !IsEntireConfigurationValid(configuration) || !IsEveryColumnValid(configuration, apiClient);
            return configuration;
        }

        private bool IsEndpointValid(Configuration configuration)
        {
            // endpoint needs to be a valid URL
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Uri(configuration.Api.Endpoint);
            }
            catch
            {
                Program.WriteError(string.Format("'{0}' is not a valid endpoint URI.", configuration.Api.Endpoint));
                return false;
            }

            // endpoint must respond to ping
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

            // endpoint must return valid mandator response
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

        // run once for every configured column
        private bool IsEveryColumnValid(Configuration configuration, ApiHttpClient apiClient)
        {
            var result = true;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var c in configuration.Columns)
            {
                var column = ColumnFactory.GetColumn(c.Type, c.AdditionalType, c.Header);
                result = column.IsColumnConfigurationValid(apiClient) && result;
            }
            return result;
        }

        // run once for every type of column
        private bool IsEntireConfigurationValid(Configuration configuration)
        {
            var result = true;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (ColumnType c in Enum.GetValues(typeof(ColumnType)))
            {
                var column = ColumnFactory.GetColumn(c, null, null);
                result = column.IsEntireConfigurationValid(configuration) && result;
            }
            return result;
        }
    }
}
