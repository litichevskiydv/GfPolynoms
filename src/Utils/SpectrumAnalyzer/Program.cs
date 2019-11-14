namespace AppliedAlgebra.SpectrumAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
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
        private static readonly MdsCodesSpectrumAnalyzer MdsCodesSpectrumAnalyzer;
        private static readonly CommonSpectrumAnalyzer CommonSpectrumAnalyzer;
        private static readonly ILogger Logger;

        private static void LogSpectrum(IReadOnlyDictionary<int, BigInteger> spectrum) =>
            Logger.LogInformation(
                "Calculated spectrum: {spectrum}",
                string.Join(", ", spectrum.OrderBy(x=>x.Key).Select(x => $"(W: {x.Key}, C: {x.Value})"))
            );

        private static IReadOnlyDictionary<int, BigInteger> AnalyzeSpectrumForRsCode(
            GaloisField field,
            int codewordLength,
            int informationWordLength
        )
        {
            Logger.LogInformation(
                "Spectrum analysis for RS[{codewordLength}, {informationWordLength}] over {field}",
                codewordLength, informationWordLength, field
            );

            var spectrum = MdsCodesSpectrumAnalyzer.Analyze(
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

        private static IReadOnlyDictionary<int, BigInteger> AnalyzeSpectrumForWaveletCode(
            int codewordLength,
            int informationWordLength,
            int codeDistance,
            Polynomial generatingPolynomial,
            SpectrumAnalyzerOptions options = null
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
            var spectrum = CommonSpectrumAnalyzer.Analyze(
                generatingPolynomial.Field,
                informationWordLength,
                informationWord =>
                    (new Polynomial(informationWord).RaiseVariableDegree(2) * generatingPolynomial % modularPolynomial)
                    .GetCoefficients(codewordLength - 1),
                options
            );
            LogSpectrum(spectrum);
            return spectrum;
        }
        private static void AnalyzeSpectrumForRsN4K3() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1)),
                4, 3
            );


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

        private static void AnalyzeSpectrumForRsN6K5() => AnalyzeSpectrumForRsCode(new PrimeOrderField(11), 6, 5);

        private static void AnalyzeSpectrumForRsN10K5() => AnalyzeSpectrumForRsCode(new PrimeOrderField(11), 10, 5);

        private static void AnalyzeSpectrumForRsN10K6() => AnalyzeSpectrumForRsCode(new PrimeOrderField(11), 10, 6);


        private static void AnalyzeSpectrumForRsN8K7() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(16, new Polynomial(new PrimeOrderField(2), 1, 0, 0, 1, 1)),
                8, 7
            );

        private static void AnalyzeSpectrumForRsN15K7() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(16, new Polynomial(new PrimeOrderField(2), 1, 0, 0, 1, 1)),
                15, 7
            );

        private static void AnalyzeSpectrumForRsN15K8() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(16, new Polynomial(new PrimeOrderField(2), 1, 0, 0, 1, 1)),
                15, 8
            );

        private static void AnalyzeSpectrumForRsN15K13() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1)),
                15, 13
            );

        private static void AnalyzeSpectrumForRsN9K8() => AnalyzeSpectrumForRsCode(new PrimeOrderField(17), 9, 8);

        private static void AnalyzeSpectrumForRsN16K8() => AnalyzeSpectrumForRsCode(new PrimeOrderField(17), 16, 8);

        private static void AnalyzeSpectrumForRsN16K9() => AnalyzeSpectrumForRsCode(new PrimeOrderField(17), 16, 9);

        private static void AnalyzeSpectrumForRsN10K9() => AnalyzeSpectrumForRsCode(new PrimeOrderField(19), 10, 9);

        private static void AnalyzeSpectrumForRsN18K9() => AnalyzeSpectrumForRsCode(new PrimeOrderField(19), 18, 9);

        private static void AnalyzeSpectrumForRsN18K10() => AnalyzeSpectrumForRsCode(new PrimeOrderField(19), 18, 10);

        private static void AnalyzeSpectrumForRsN26K13() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1)),
                26, 13
            );

        private static void AnalyzeSpectrumForRsN26K15() =>
            AnalyzeSpectrumForRsCode(
                new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1)),
                26, 15
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

        private static void AnalyzeSpectrumForWvN15K7D8() =>
            AnalyzeSpectrumForWaveletCode(
                15, 7, 8,
                new Polynomial(
                    new PrimePowerOrderField(16, new Polynomial(new PrimeOrderField(2), 1, 0, 0, 1, 1)),
                    3, 3, 13, 2, 4, 5, 2, 9, 11, 11, 14, 3, 9, 11, 10
                ),
                new SpectrumAnalyzerOptions {LoggingResolution = 100000}
            );

        private static void AnalyzeSpectrumForWvN16K8D8() =>
            AnalyzeSpectrumForWaveletCode(
                16, 8, 8,
                new Polynomial(new PrimeOrderField(17), 11, 14, 15, 16, 9, 5, 4, 14, 1, 11, 15, 11, 8, 11, 2, 6),
                new SpectrumAnalyzerOptions { LoggingResolution = 10000000 }
            );

        private static void AnalyzeSpectrumForWvN18K9D9() =>
            AnalyzeSpectrumForWaveletCode(
                18, 9, 9,
                new Polynomial(new PrimeOrderField(19), 16, 2, 1, 0, 16, 11, 0, 18, 16, 1, 8, 2, 3, 6, 0, 7, 6, 1),
                new SpectrumAnalyzerOptions { LoggingResolution = 1000000000 }
            );

        private static void AnalyzeSpectrumForWvN24K12D8() =>
            AnalyzeSpectrumForWaveletCode(
                24, 12, 8,
                new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 0, 1)
            );

        private static void AnalyzeSpectrumForWvN26K13D12() =>
            AnalyzeSpectrumForWaveletCode(
                26, 13, 12,
                new Polynomial(
                    new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1)),
                    1, 0, 17, 18, 3, 12, 22, 15, 6, 10, 19, 15, 5, 11, 11, 15, 22, 11, 2, 6, 8, 0, 7, 4, 0, 15
                )
            );


        [UsedImplicitly]
        public static void Main()
        {
            try
            {
                AnalyzeSpectrumForRsN9K8();
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

            MdsCodesSpectrumAnalyzer = new MdsCodesSpectrumAnalyzer();
            CommonSpectrumAnalyzer = new CommonSpectrumAnalyzer(new RecursiveIterator(), loggerFactory.CreateLogger<CommonSpectrumAnalyzer>());
        }
    }

}
