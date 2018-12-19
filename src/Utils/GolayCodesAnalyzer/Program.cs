namespace AppliedAlgebra.GolayCodesAnalyzer
{
    using System.Reflection;
    using CodesResearchTools.Analyzers.ListsSizesDistribution;
    using CodesResearchTools.NoiseGenerator;
    using GolayCodesTools;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Serilog;

    [UsedImplicitly]
    public class Program
    {
        private static IConfiguration _configuration;
        private static ILoggerFactory _loggerFactory;

        private static void BuildConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private static void BuildLoggerFactory()
        {
            _loggerFactory = new LoggerFactory()
                .AddSerilog(
                    new LoggerConfiguration()
                        .ReadFrom.Configuration(_configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Version", Assembly.GetEntryAssembly().GetName().Version.ToString(4))
                        .CreateLogger()
                );
        }

        [UsedImplicitly]
        static void Main(string[] args)
        {
            BuildConfiguration();
            BuildLoggerFactory();

            var listsSizesDistributionAnalyzer = new ListsSizesDistributionAnalyzer(
                new RecursiveGenerator(),
                _loggerFactory.CreateLogger<ListsSizesDistributionAnalyzer>()
            );
            var logger = _loggerFactory.CreateLogger<Program>();

            var listsSizesDistribution = listsSizesDistributionAnalyzer.Analyze(
                new G12GolayCode(),
                new ListsSizesDistributionAnalyzerOptions {MaxDegreeOfParallelism = 3}
            );
            logger.LogInformation(JsonConvert.SerializeObject(listsSizesDistribution, Formatting.Indented));
        }
    }
}
