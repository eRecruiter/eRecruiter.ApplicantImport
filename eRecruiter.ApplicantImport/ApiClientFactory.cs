using System;
using eRecruiter.Api.Client;
using eRecruiter.Api.Parameters;

namespace eRecruiter.ApplicantImport
{
    public static class ApiClientFactory
    {
        private static readonly ApiTokenCache TokenCache = new ApiTokenCache();

        public static ApiHttpClient GetClient(Configuration config)
        {
            var client = new ApiHttpClient(new Uri(config.Api.Endpoint),
                () => new ApiKeyParameter
                {
                    ClientInfo = "eRecruiter.ApplicantImport",
                    Key = config.Api.Key,
                    MandatorId = config.Api.MandatorId
                }, () => TokenCache);

            return client;
        }
    }
}
