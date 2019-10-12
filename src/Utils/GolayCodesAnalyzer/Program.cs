namespace AppliedAlgebra.GolayCodesAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using CodesResearchTools.Analyzers.ListsSizesDistribution;
    using CodesResearchTools.NoiseGenerator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
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

        private static IEnumerable<int[]> IterateVariants(GaloisField field, int currentPosition, int[] values)
        {
            if (currentPosition == values.Length)
            {
                yield return values;
                yield break;
            }

            for (var i = 0; i < field.Order; i++)
            {
                values[currentPosition] = i;
                foreach (var computedValues in IterateVariants(field, currentPosition + 1, values))
                    yield return computedValues;
            }
        }

        private static void FindWaveletCodeGeneratingPolynomial(GolayCodeBase golayCode, ILogger logger)
        {
            var modularPolynomial
                = new Polynomial(golayCode.Field, 1).RightShift(golayCode.CodewordLength)
                  + new Polynomial(golayCode.Field, golayCode.Field.InverseForAddition(1));

            var processedPolynomials = 0;
            foreach (var coefficients in IterateVariants(golayCode.Field, 0, new int[golayCode.CodewordLength]).Skip(1))
            {
                var generatingPolynomial = new Polynomial(golayCode.Field, coefficients.Reverse().ToArray());

                var isValid = true;
                var codeDistance = golayCode.CodewordLength;
                foreach (var values in IterateVariants(golayCode.Field, 0, new int[golayCode.InformationWordLength]).Skip(1))
                {
                    var codeword = generatingPolynomial * new Polynomial(golayCode.Field, values).RaiseVariableDegree(2) % modularPolynomial;

                    var codewordWeight = 0;
                    for(var i = 0; i<= codeword.Degree; i++)
                        if (codeword[i] != 0) codewordWeight++;
                    codeDistance = Math.Min(codeDistance, codewordWeight);

                    if (codeDistance < golayCode.CodeDistance)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid && codeDistance == golayCode.CodeDistance) 
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
