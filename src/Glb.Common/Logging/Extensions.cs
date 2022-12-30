using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Glb.Common.Settings;

namespace Glb.Common.Logging
{
    public static class Extensions
    {
        public static IServiceCollection AddSeqLogging(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddLogging(loggingBuilder =>
            {
                var seqSettings = config.GetSection(nameof(SeqSettings)).Get<SeqSettings>();
                if (seqSettings != null)
                {
                    loggingBuilder.AddSeq(serverUrl: seqSettings.ServerUrl);
                }

            });

            return services;
        }
    }
}