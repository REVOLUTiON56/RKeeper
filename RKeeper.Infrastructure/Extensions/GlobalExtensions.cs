using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RKeeper.Infrastructure.Extensions;

public static class GlobalExtensions
{
    public static void SetupGlobalExceptionLogging(this IApplicationBuilder app)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

        void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            var logger = GetLogger(nameof(TaskSchedulerOnUnobservedTaskException));
            logger.LogCritical(e.Exception, "Unobserved Task exception occurred");
        }

        void CurrentDomainOnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
        {
            var logger = GetLogger(nameof(CurrentDomainOnUnhandledException));
            logger.LogCritical(e.ExceptionObject as Exception, "Current domain unhandled exception occurred");
        }

        ILogger GetLogger(string name)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var provider = serviceScope.ServiceProvider;
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger(name);
        }
    }
}
