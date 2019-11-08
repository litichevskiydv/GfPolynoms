﻿namespace AppliedAlgebra.SpectrumAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using CodesResearchTools.Analyzers.Spectrum;
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    [UsedImplicitly]
    public class Program
    {
        private static readonly ISpectrumAnalyzer SpectrumAnalyzer;
        private static readonly ILogger Logger;

        private static void LogSpectrum(IReadOnlyDictionary<int, long> spectrum) =>
            Logger.LogInformation(
                "Calculated spectrum: {spectrum}",
                string.Join(", ", spectrum.OrderBy(x=>x.Key).Select(x => $"(W: {x.Key}, C: {x.Value})"))
            );

        private static IReadOnlyDictionary<int, long> AnalyzeSpectrumForRsCode(
            GaloisField field,
            int codewordLength,
            int informationWordLength
        )
        {
            Logger.LogInformation(
                "Spectrum analysis for RS[{codewordLength}, {informationWordLength}] over {field}",
                codewordLength, informationWordLength, field
            );

            var spectrum = SpectrumAnalyzer.Analyze(
                field,
                informationWordLength,
                informationWord =>
                {
                    var informationPolynomial = new Polynomial(informationWord);
                    return Enumerable.Range(0, codewordLength)
                        .Select(x => field.CreateElement(informationPolynomial.Evaluate(field.GetGeneratingElementPower(x))))
                        .ToArray();
                }
            );
            LogSpectrum(spectrum);
            return spectrum;
        }

        private static IReadOnlyDictionary<int, long> AnalyzeSpectrumForWaveletCode(
            int codewordLength,
            int informationWordLength,
            int codeDistance,
            Polynomial generatingPolynomial
        )
        {
            var field = generatingPolynomial.Field;
            Logger.LogInformation(
                "Spectrum analysis for W[{codewordLength}, {informationWordLength}, {codeDistance}] over {field}, generating polynomial {generatingPolynomial}",
                codewordLength, informationWordLength, codeDistance, field, generatingPolynomial
            );

            var modularPolynomial
                = new Polynomial(field, 1).RightShift(codewordLength)
                  + new Polynomial(field, field.InverseForAddition(1));
            var spectrum = SpectrumAnalyzer.Analyze(
                generatingPolynomial.Field,
                informationWordLength,
                informationWord =>
                    (new Polynomial(informationWord).RaiseVariableDegree(2) * generatingPolynomial % modularPolynomial)
                    .GetCoefficients(codewordLength - 1)
            );
            LogSpectrum(spectrum);
            return spectrum;
        }

        private static void AnalyzeSpectrumForRsN7K3() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1)),
                7, 3
            );

        private static void AnalyzeSpectrumForRsN7K4() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1)),
                7, 4
            );

        private static void AnalyzeSpectrumForRsN5K4() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1)),
                5, 4
            );

        private static void AnalyzeSpectrumForRsN6K4() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1)),
                6, 4
            );

        private static void AnalyzeSpectrumForRsN8K4() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1)),
                8, 4
            );

        private static void AnalyzeSpectrumForRsN8K5() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1)),
                8, 5
            );

        private static void AnalyzeSpectrumForRsN8K6() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1)),
                8, 6
            );

        private static void AnalyzeSpectrumForWvN7K3D4First() =>
            AnalyzeSpectrumForWaveletCode(
                7, 3, 4,
                new Polynomial(
                    new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1)),
                    0, 0, 2, 5, 6, 0, 1
                )
            );

        private static void AnalyzeSpectrumForWvN7K3D4Second() =>
            AnalyzeSpectrumForWaveletCode(
                7, 3, 4,
                new Polynomial(
                    new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1)),
                    1, 2, 1, 1
                )
            );

        private static void AnalyzeSpectrumForWvN8K4D4Golay() =>
            AnalyzeSpectrumForWaveletCode(
                8, 4, 4,
                new Polynomial(
                    new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1)),
                    2, 0, 1, 2, 1, 1
                )
            );

        private static void AnalyzeSpectrumForWvN8K4D4() =>
            AnalyzeSpectrumForWaveletCode(
                8, 4, 4,
                new Polynomial(
                    new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1)),
                    2, 8, 3, 8, 0, 6, 2, 7
                )
            );

        private static void AnalyzeSpectrumForWvN8K4D3() =>
            AnalyzeSpectrumForWaveletCode(
                8, 4, 3,
                new Polynomial(
                    new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1)),
                    2, 5, 5, 1, 1, 3, 2, 2
                )
            );

        private static void AnalyzeSpectrumForWvN10K5D5() =>
            AnalyzeSpectrumForWaveletCode(
                10, 5, 5,
                new Polynomial(new PrimeOrderField(11), 8, 10, 4, 6, 8, 9, 2, 10, 4, 5)
            );

        private static void AnalyzeSpectrumForWvN24K12D8() =>
            AnalyzeSpectrumForWaveletCode(
                24, 12, 8,
                new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 0, 1)
            );


        [UsedImplicitly]
        public static void Main()
        {
            try
            {
                AnalyzeSpectrumForRsN5K4();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Exception occurred during analysis");
            }
            Console.ReadKey();
        }

        static Program()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var loggerFactory = new LoggerFactory()
                .AddSerilog(
                    new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Version", Assembly.GetEntryAssembly().GetName().Version.ToString(4))
                        .CreateLogger()
                );
            Logger = loggerFactory.CreateLogger<Program>();

            SpectrumAnalyzer = new CommonSpectrumAnalyzer(new RecursiveIterator(), loggerFactory.CreateLogger<CommonSpectrumAnalyzer>());
        }
    }

}
