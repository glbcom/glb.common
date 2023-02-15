using System.Linq;
using Glb.Common.Interfaces;
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
    public static IServiceCollection AddSMS_Service(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SMS_Settings>(config.GetSection(nameof(SMS_Settings)));
        services.AddTransient<ISMS_Service, SMS_Service>();
        return services;
    }

    #region  "MobileNumber"
    public static string? ToShortMobileNumber(this string MobileNumber, string Separator = "", string[]? MobileCodes = null)
    {
        if (MobileCodes == null)
            MobileCodes = new string[] { "3", "70", "71", "76", "78", "79", "81" };
        string mobilenumber = MobileNumber;
        mobilenumber = System.Text.RegularExpressions.Regex.Replace(MobileNumber, "[^0-9]", "");
        if (mobilenumber.Length < 7)
            return null;
        string BaseNumber = mobilenumber.Substring(mobilenumber.Length - 6);
        mobilenumber = mobilenumber.Replace(BaseNumber, "");
        string? Code = MobileCodes.ToList().Where(_code => mobilenumber.EndsWith((int.Parse(_code)).ToString())).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(Code))
            return null;
        mobilenumber = string.Format("{0}{1}{2}", Code.PadLeft(2, '0'), Separator, BaseNumber);
        return mobilenumber;
    }
    public static string? ToLongMobileNumber(this string MobileNumber, string[]? MobileCodes = null)
    {
        string? shortmobilenumber = ToShortMobileNumber(MobileNumber, MobileCodes: MobileCodes);
        if (!string.IsNullOrWhiteSpace(shortmobilenumber))
            return "+961" + int.Parse(shortmobilenumber).ToString();
        return null;
    }
    #endregion

}