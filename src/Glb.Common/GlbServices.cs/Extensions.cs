using Glb.Common.Inerfaces;
using Glb.Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Glb.Common.GlbServices;
public static class Extensions
{
    public static IServiceCollection AddMailService(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MailSettings>(config.GetSection(nameof(MailSettings)));
        services.AddTransient<IMailService, MailService>();
        return services;
    }
}