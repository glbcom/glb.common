using Glb.Common.ProblemDetails;
using Glb.Common.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Glb.Common.Logging{
    public interface IGlbLoggingService<T>
    {
        void LogInformation(string message, params object?[] args);
        void LogError(string message, params object?[] args);
        void LogWarning(string message, params object?[] args);
        void LogDebug(string message, params object?[] args);
        void LogTrace(string message, params object?[] args);
        void LogCritical(string message, params object?[] args);
    }

    public class GlbLoggingService<T> : IGlbLoggingService<T>
    {

        #region Private methods
        private Entities.GlbApplicationUser? CurrentUser;
        private readonly ILogger<T> logger;
        private readonly ServiceSettings? serviceSettings;
        #endregion
        
        private readonly HttpContext? httpContext;
        public GlbLoggingService(ILogger<T> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>()!;         
        }
        private void LogMessageWithValue(LogLevel logLevel, string message, params object?[] args)
        {
            string? instance = null;
            string problemDetailString = message;
            if (args != null && args.Length > 0)
            {
                // Get all text between curly braces
                System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(problemDetailString, @"\{.*?\}");

                if (matches.Count > 0 && matches.Count <= args.Length)
                {
                    // Replace the text between curly braces with the values in the args array
                    for (int i = 0; i < matches.Count; i++)
                    {

                        problemDetailString = problemDetailString.Replace(matches[i].Value, args[i]?.ToString());
                    }
                }
            }

            message = message + " serviceName:{serviceName} userId:{userId} compId:{compId}";

            // Add the original args to the newArgs array first
            int argsLength = args == null ? 0 : args.Length;
            object?[] newArgs = new object?[argsLength + 3];
            if (args != null)
            {
                for (int i = 0; i < argsLength; i++)
                {
                    newArgs[i] = args[i];
                }
            }


            // Add the three new parameters at the end
            newArgs[argsLength] = serviceSettings == null ? "" : serviceSettings.ServiceName;
            newArgs[argsLength + 1] = CurrentUser?.Id;
            newArgs[argsLength + 2] = CurrentUser?.ScopeCompId;
            args = newArgs;


            switch (logLevel)
            {
                case LogLevel.Critical:
                    logger.LogCritical(message, args);
                    break;
                case LogLevel.Error:
                    logger.LogError(message, args);
                    break;
                case LogLevel.Warning:
                    logger.LogWarning(message, args);
                    break;
                case LogLevel.Debug:
                    logger.LogDebug(message, args);
                    break;
                case LogLevel.Information:
                    logger.LogInformation(message, args);
                    break;
                case LogLevel.Trace:
                    logger.LogTrace(message, args);
                    break;
            }
            CurrentUser  = new Entities.GlbApplicationUser() {Id = System.Guid.NewGuid(),ScopeCompId = "IDM" };
            
            GlbProblemDetails glbProblemDetails = new GlbProblemDetails(problemDetailString, instance, CurrentUser);
            if (serviceSettings != null)
            {
                glbProblemDetails.ServiceName = serviceSettings.ServiceName;
            }
            glbProblemDetails.LogLevel = logLevel.ToString();
            if(httpContext!=null){
                httpContext.Features.Set(glbProblemDetails);
            }
            
        }
        public void LogInformation(string message, params object?[] args)
        {
            LogMessageWithValue(LogLevel.Information, message, args);
        }
        public void LogError(string message, params object?[] args)
        {
            LogMessageWithValue(LogLevel.Error, message, args);
        }
        public void LogWarning(string message, params object?[] args)
        {
            LogMessageWithValue(LogLevel.Warning, message, args);
        }
        public void LogDebug(string message, params object?[] args)
        {
            LogMessageWithValue(LogLevel.Debug, message, args);
        }
        public void LogTrace(string message, params object?[] args)
        {
            LogMessageWithValue(LogLevel.Trace, message, args);
        }
        public void LogCritical(string message, params object?[] args)
        {
            LogMessageWithValue(LogLevel.Critical, message, args);
        }
    }
}