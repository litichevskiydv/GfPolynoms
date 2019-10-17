namespace AppliedAlgebra.GolayCodesAnalyzer
{
    using System;
    using System.Linq;
    using System.Reflection;
    using CodesResearchTools.Analyzers.ListsSizesDistribution;
    using CodesResearchTools.NoiseGenerator;
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GolayCodesTools;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Serilog;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

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

        private static void FindWaveletCodeGeneratingPolynomial(GolayCodeBase code, ILogger logger)
        {
            var modularPolynomial
                = new Polynomial(code.Field, 1).RightShift(code.CodewordLength)
                  + new Polynomial(code.Field, code.Field.InverseForAddition(1));

            var processedPolynomials = 0;
            var variantsIterator = new RecursiveIterator();
            foreach (var generatingPolynomial in variantsIterator.IteratePolynomials(code.Field, code.CodewordLength - 1).Skip(1))
            {
                var isValid = true;
                var codeDistance = code.CodewordLength;
                foreach (var informationPolynomial in variantsIterator.IteratePolynomials(code.Field, code.InformationWordLength - 1).Skip(1))
                {
                    var codeword = generatingPolynomial * informationPolynomial.RaiseVariableDegree(2) % modularPolynomial;

                    var codewordWeight = 0;
                    for(var i = 0; i<= codeword.Degree; i++)
                        if (codeword[i] != 0) codewordWeight++;
                    codeDistance = Math.Min(codeDistance, codewordWeight);

                    if (codeDistance < code.CodeDistance)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid && codeDistance == code.CodeDistance) 
                    logger.LogInformation(generatingPolynomial.ToString());

                processedPolynomials++;
                if (processedPolynomials % 10000 == 0) Console.WriteLine($"Processed {processedPolynomials}");
            }
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

            try
            {
                var listsSizesDistribution = listsSizesDistributionAnalyzer.Analyze(
                    new G12GolayCode(),
                    new ListsSizesDistributionAnalyzerOptions
                    {
                        MaxDegreeOfParallelism = 3,
                        FullLogsPath = @"c:\Users\litic\Documents\Study\Coding\PostGraduateWorks\Experiments\"
                    }
                );
                logger.LogInformation(JsonConvert.SerializeObject(listsSizesDistribution, Formatting.Indented));
            }
            catch (Exception exception)
            {
                logger.LogError(0, exception, "Exception occurred during analysis");
            }
        }
    }
}
