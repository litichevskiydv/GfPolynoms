namespace AppliedAlgebra.WaveletCodesListDecodingAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using CodesAbstractions;
    using CodesResearchTools.Analyzers.CodeDistance;
    using CodesResearchTools.Analyzers.ListsSizesDistribution;
    using CodesResearchTools.NoiseGenerator;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.PolynomialsGcdFinder;
    using GfPolynoms;
    using GfPolynoms.Comparers;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using RsCodesTools.Decoding.ListDecoder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using RsCodesTools.Decoding.StandartDecoder;
    using Serilog;
    using WaveletCodesTools;
    using WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes;
    using WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes.GsBasedDecoderDependencies;
    using WaveletCodesTools.Decoding.StandartDecoderForFixedDistanceCodes;
    using WaveletCodesTools.FixedDistanceCodesFactory;
    using WaveletCodesTools.GeneratingPolynomialsBuilder;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    [UsedImplicitly]
    public class Program
    {
        private static readonly INoiseGenerator NoiseGenerator;
        private static readonly LinearCodeDistanceAnalyzer LinearCodeDistanceAnalyzer;
        private static readonly ListsSizesDistributionAnalyzer ListsSizesDistributionAnalyzer;
        private static readonly IEqualityComparer<FieldElement[]> WordsComparer;
        private static readonly IFixedDistanceCodesFactory FixedDistanceCodesFactory;
        private static readonly IGsBasedDecoderTelemetryCollector TelemetryCollector;

        private static readonly ILogger Logger;

        private static void AnalyzeCodeDistance(int codewordLength, int informationWordLength, Polynomial generatingPolynomial)
        {
            var field = generatingPolynomial.Field;
            var modularPolynomial = new Polynomial(field, 1).RightShift(codewordLength) + new Polynomial(field, field.InverseForAddition(1));

            var codeDistance = LinearCodeDistanceAnalyzer.Analyze(
                field,
                informationWordLength,
                x => (new Polynomial(field, x).RaiseVariableDegree(2) * generatingPolynomial % modularPolynomial).GetCoefficients(codewordLength - 1),
                new CodeDistanceAnalyzerOptions { LoggingResolution = 1000000000L}
            );
            Logger.LogInformation("Code distance: {codeDistance}", codeDistance);

        }

        private static void AnalyzeSpherePackings(ICode code)
        {
            Logger.LogInformation(
                JsonConvert.SerializeObject(
                    ListsSizesDistributionAnalyzer.Analyze(code, new ListsSizesDistributionAnalyzerOptions {MaxDegreeOfParallelism = 2}),
                    Formatting.Indented
                )
            );
        }

        private static void AnalyzeCode(ICode code, int errorsCount, int? decodingThreadsCount = null)
        {
            var informationWord = Enumerable.Repeat(code.Field.Zero(), code.InformationWordLength).ToArray();
            var codeword = code.Encode(informationWord);

            var processedNoises = 0;
            Parallel.ForEach(
                NoiseGenerator.VariatePositionsAndValues(code.Field, code.CodewordLength, errorsCount),
                new ParallelOptions {MaxDegreeOfParallelism = decodingThreadsCount ?? (int) (Environment.ProcessorCount * 1.5d)},
                x =>
                {
                    var decodingResults = code.DecodeViaList(codeword.AddNoise(x), errorsCount);
                    if (decodingResults.Contains(informationWord, WordsComparer) == false)
                        throw new InvalidOperationException($"Failed to process noise {string.Join<FieldElement>(",", x)}");

                    if (Interlocked.Increment(ref processedNoises) % 50 == 0)
                        Logger.LogInformation($"[{Thread.CurrentThread.ManagedThreadId}]: Current noise value ({string.Join<FieldElement>(",", x)})");
                    if (TelemetryCollector.ProcessedSamplesCount % 100 == 0)
                        Logger.LogInformation(TelemetryCollector.ToString());
                }
            );
        }

        private static void AnalyzeSamples(params AnalyzingSample[] samples)
        {
            Parallel.ForEach(
                samples,
                new ParallelOptions {MaxDegreeOfParallelism = Math.Min((int) (Environment.ProcessorCount * 1.5d), samples.Length)},
                x =>
                {
                    var initialAdditiveNoise = Enumerable.Repeat(x.Code.Field.Zero(), x.Code.CodewordLength).ToArray();
                    for (var i = 0; i < x.ErrorsPositions.Length; i++)
                        initialAdditiveNoise[x.ErrorsPositions[i]] = x.Code.Field.CreateElement(x.ErrorsValues[i]);

                    foreach (var additiveNoise in NoiseGenerator.VariateValues(initialAdditiveNoise))
                    {
                        var decodingResults = x.Code.DecodeViaList(x.Codeword.AddNoise(additiveNoise), x.Code.CodewordLength - x.CorrectValuesCount);
                        if(decodingResults.Contains(x.InformationWord, WordsComparer) == false)
                            throw new InvalidOperationException($"Failed to process noise {string.Join<FieldElement>(",", additiveNoise)}");

                        if (++x.ProcessedNoises % 50 == 0)
                            Logger.LogInformation($"[{Thread.CurrentThread.ManagedThreadId}]: Current noise value ({string.Join<FieldElement>(",", additiveNoise)})");
                        if (TelemetryCollector.ProcessedSamplesCount % 100 == 0)
                            Logger.LogInformation(TelemetryCollector.ToString());
                    }
                }
            );
        }

        private static void AnalyzeCodeDistanceForN26K13() =>
            AnalyzeCodeDistance(26, 13, new Polynomial(new PrimePowerOrderField(27), 2, 0, 1, 2, 1, 1));

        private static void AnalyzeSpherePackingsForN7K3D4() =>
            AnalyzeSpherePackings(
                new WaveletCode(
                    7, 3, 4,
                    new Polynomial(
                        new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1)),
                        0, 0, 2, 5, 6, 0, 1
                    )
                )
            );

        private static void AnalyzeSpherePackingsForN8K4D3() =>
            AnalyzeSpherePackings(
                new WaveletCode(
                    8, 4, 4,
                    new Polynomial(new PrimePowerOrderField(9), 2, 0, 1, 2, 1, 1)
                )
            );

        private static void AnalyzeSpherePackingsForN10K5D5() =>
            AnalyzeSpherePackings(
                new WaveletCode(
                    10, 5, 5,
                    new Polynomial(new PrimeOrderField(11), 8, 10, 4, 6, 8, 9, 2, 10, 4, 5)
                )
            );

        private static void AnalyzeCodeN7K3D4() => AnalyzeCode(FixedDistanceCodesFactory.CreateN7K3D4(), 2, 2);

        private static void AnalyzeCodeN26K13D12() => AnalyzeCode(FixedDistanceCodesFactory.CreateN26K13D12(), 6, 2);

        private static void AnalyzeSamplesForN15K7D8Code() =>
            AnalyzeSamples(
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN15K7D8())
                {
                    ErrorsPositions = new[] {2, 6, 8, 14},
                    ErrorsValues = new[] {1, 1, 12, 12},
                    CorrectValuesCount = 11
                },
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN15K7D8())
                {
                    ErrorsPositions = new[] {0, 1, 2, 3},
                    ErrorsValues = new[] {1, 3, 1, 8},
                    CorrectValuesCount = 11
                }
            );

        private static void AnalyzeSamplesForN26K13D12Code() =>
            AnalyzeSamples(
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN26K13D12())
                {
                    ErrorsPositions = new[] {2, 6, 8, 15, 16, 22},
                    ErrorsValues = new[] {1, 1, 12, 12, 4, 3},
                    CorrectValuesCount = 20
                },
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN26K13D12())
                {
                    ErrorsPositions = new[] {0, 1, 2, 3, 4, 5},
                    ErrorsValues = new[] {1, 3, 1, 8, 1, 5},
                    CorrectValuesCount = 20
                }
            );

        private static void AnalyzeSamplesForN30K15D13Code() =>
            AnalyzeSamples(
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN30K15D13())
                {
                    ErrorsPositions = new[] {2, 6, 8, 15, 16, 22, 24},
                    ErrorsValues = new[] {1, 1, 12, 12, 5, 18, 20},
                    CorrectValuesCount = 23
                },
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN30K15D13())
                {
                    ErrorsPositions = new[] {0, 1, 2, 3, 4, 5, 6},
                    ErrorsValues = new[] {1, 3, 1, 8, 2, 20, 8},
                    CorrectValuesCount = 23
                }
            );

        private static void AnalyzeSamplesForN31K15D15Code() =>
            AnalyzeSamples(
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN31K15D15())
                {
                    ErrorsPositions = new[] {2, 6, 8, 14, 18, 23, 27, 30},
                    ErrorsValues = new[] {1, 1, 12, 13, 11, 16, 31, 14},
                    CorrectValuesCount = 23
                },
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN31K15D15())
                {
                    ErrorsPositions = new[] {0, 1, 2, 3, 4, 5, 6, 7},
                    ErrorsValues = new[] {1, 3, 1, 8, 18, 24, 1, 19},
                    CorrectValuesCount = 23
                }
            );

        private static void AnalyzeSamplesForN80K40D37Code() =>
            AnalyzeSamples(
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN80K40D37())
                {
                    ErrorsPositions = new[] {2, 6, 8, 15, 16, 22, 23, 28, 35, 37, 47, 50, 51, 52, 67, 70, 71, 72, 77, 78, 79},
                    ErrorsValues = new[] {1, 1, 6, 4, 4, 14, 7, 3, 8, 23, 76, 1, 5, 8, 2, 6, 2, 14, 45, 64, 13},
                    CorrectValuesCount = 59
                },
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN80K40D37())
                {
                    ErrorsPositions = new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20},
                    ErrorsValues = new[] {1, 1, 6, 4, 4, 14, 7, 3, 8, 23, 76, 1, 5, 8, 2, 6, 2, 14, 45, 64, 13},
                    CorrectValuesCount = 59
                }
            );

        private static void AnalyzeSamplesForN100K50D49Code() =>
            AnalyzeSamples(
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN100K50D49())
                {
                    ErrorsPositions = new[] {2, 6, 8, 15, 16, 22, 23, 28, 35, 37, 47, 50, 51, 52, 67, 70, 71, 72, 77, 78, 79, 81, 83, 84, 86, 88, 91, 92},
                    ErrorsValues = new[] {1, 1, 6, 4, 4, 14, 7, 3, 8, 23, 76, 1, 5, 8, 2, 6, 2, 14, 45, 64, 13, 54, 34, 64, 34, 2, 64, 23},
                    CorrectValuesCount = 72
                },
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN100K50D49())
                {
                    ErrorsPositions = new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27},
                    ErrorsValues = new[] {1, 1, 6, 4, 4, 14, 7, 3, 8, 23, 76, 1, 5, 8, 2, 6, 2, 14, 45, 64, 13, 54, 34, 64, 34, 2, 64, 23},
                    CorrectValuesCount = 72
                }
            );


        [UsedImplicitly]
        public static void Main()
        {
            try
            {
                AnalyzeSpherePackingsForN10K5D5();
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

            LinearCodeDistanceAnalyzer = new LinearCodeDistanceAnalyzer(loggerFactory.CreateLogger<LinearCodeDistanceAnalyzer>());

            NoiseGenerator = new RecursiveGenerator();
            WordsComparer = new FieldElementsArraysComparer();
            ListsSizesDistributionAnalyzer = new ListsSizesDistributionAnalyzer(NoiseGenerator, loggerFactory.CreateLogger<ListsSizesDistributionAnalyzer>());

            var gaussSolver = new GaussSolver();
            TelemetryCollector = new GsBasedDecoderTelemetryCollectorForGsBasedDecoder();
            FixedDistanceCodesFactory
                = new StandardCodesFactory(
                    new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()), gaussSolver),
                    new RsBasedDecoder(new BerlekampWelchDecoder(gaussSolver), gaussSolver),
                    new GsBasedDecoder(
                        new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalcualtor()), new RrFactorizator()),
                        gaussSolver
                    )
                    {TelemetryCollector = TelemetryCollector}
                );
        }
    }
}
