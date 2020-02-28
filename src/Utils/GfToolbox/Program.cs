namespace AppliedAlgebra.GfToolbox
{
    using System;
    using System.CommandLine;
    using System.Reflection;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    [UsedImplicitly]
    public class Program
    {
        private static readonly ILoggerFactory LoggerFactory;
        private static readonly ILogger Logger;

        static Program()
        {
            LoggerFactory = new LoggerFactory()
                .AddSerilog(
                    new LoggerConfiguration()
                        .ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Version", Assembly.GetEntryAssembly().GetName().Version.ToString(4))
                        .CreateLogger()
                );
            Logger = LoggerFactory.CreateLogger<Program>();
        }

        [UsedImplicitly]
        static void Main(string[] args)
        {
            try
            {
                new RootCommand("Command line interface for algorithms over finite fields")
                {
                    new Command("polyphase-representation", "Computes polyphase representation for the given polynomial")
                }.Invoke(args);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Exception occurred during the execution");
            }
        }
    }
}
