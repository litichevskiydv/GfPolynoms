namespace AppliedAlgebra.GfToolbox
{
    using System;
    using System.CommandLine;
    using System.Linq;
    using System.Reflection;
    using CommandsDescriptions;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    [UsedImplicitly]
    public class Program
    {
        private static readonly IConfigurationRoot ConfigurationRoot;
        private static readonly ILoggerFactory LoggerFactory;
        private static readonly ILogger Logger;

        static Program()
        {
            ConfigurationRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            LoggerFactory = new LoggerFactory()
                .AddSerilog(
                    new LoggerConfiguration()
                        .ReadFrom.Configuration(ConfigurationRoot)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version.ToString(4))
                        .CreateLogger()
                );
            Logger = LoggerFactory.CreateLogger<Program>();
        }

        private static RootCommand AddSubCommandsFromCurrentAssembly(RootCommand rootCommand)
        {
            var commandDescriptionInterface = typeof(ICommandDescription);
            var commandsDescriptionsTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.IsAbstract == false && commandDescriptionInterface.IsAssignableFrom(x));
            foreach (var commandsDescriptionType in commandsDescriptionsTypes)
                rootCommand.AddCommand(
                    ((ICommandDescription)Activator.CreateInstance(commandsDescriptionType, ConfigurationRoot, LoggerFactory))
                    .ToCommand()
                );

            return rootCommand;
        }

        [UsedImplicitly]
        static void Main(string[] args)
        {
            var rootCommand = AddSubCommandsFromCurrentAssembly(new RootCommand("Command line interface for algorithms over finite fields"));

            try
            {
                rootCommand.Invoke(args);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Exception occurred during the execution");
            }
        }
    }
}
