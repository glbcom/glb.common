using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Glb.Common.HealthChecks
{
    public class MongoDbHealthCheck : IHealthCheck
    {
        private readonly MongoClient? client;

        public MongoDbHealthCheck(MongoClient client)
        {
            this.client = client;
        }
        public MongoDbHealthCheck()
        {

        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {

            try
            {
                if (client == null)
                {
                    throw new Exception("MongoClient is null");
                }
                await client.ListDatabaseNamesAsync(cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }
}