namespace WaveletCodesListDecodingAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.PolynomialsGcdFinder;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using NLog.Config;
    using NLog.Extensions.Logging;
    using RsCodesTools.ListDecoder;
    using RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using WaveletCodesTools.Encoder;
    using WaveletCodesTools.GeneratingPolynomialsBuilder;
    using WaveletCodesTools.ListDecoderForFixedDistanceCodes;

    [UsedImplicitly]
    public class Program
    {
        private static ILogger _logger;

        private static void GenerateSamples(IEncoder encoder, int n, Polynomial generatingPolynomial, Polynomial m,
            int[] informationWord, int informationWordPosition,
            ICollection<AnalyzingSample> samples, int? samplesCount = null)
        {
            if (informationWordPosition == informationWord.Length)
            {
                var informationPolynomial = new Polynomial(generatingPolynomial.Field, informationWord);
                samples.Add(new AnalyzingSample(informationPolynomial, encoder.Encode(n, generatingPolynomial, informationPolynomial, m)));
                return;
            }

            for (var i = 0; i < generatingPolynomial.Field.Order; i++)
            {
                if (samplesCount.HasValue && samplesCount.Value == samples.Count)
                    break;

                informationWord[informationWordPosition] = i;
                GenerateSamples(encoder, n, generatingPolynomial, m, informationWord, informationWordPosition + 1, samples, samplesCount);
            }
        }

        private static IEnumerable<AnalyzingSample> GenerateSamples(int n, int k, Polynomial generatingPolynomial, int? samplesCount = null)
        {
            var samples = new List<AnalyzingSample>();
            var informationWord = new int[k];

            var m = new Polynomial(generatingPolynomial.Field, 1).RightShift(n);
            m[0] = generatingPolynomial.Field.InverseForAddition(1);
            var encoder = new Encoder();

            GenerateSamples(encoder, n, generatingPolynomial, m, informationWord, 0, samples, samplesCount);
            return samples;
        }

        private static void GenerateErrorsPositions(int n, IList<int> errorsPositions, int placedErrorsCount,
            ICollection<int[]> allErrorsPositions)
        {
            if (placedErrorsCount == errorsPositions.Count)
                allErrorsPositions.Add(errorsPositions.ToArray());
            else
                for (var i = placedErrorsCount == 0 ? 0 : errorsPositions[placedErrorsCount - 1] + 1; i < n; i++)
                {
                    errorsPositions[placedErrorsCount] = i;
                    GenerateErrorsPositions(n, errorsPositions, placedErrorsCount + 1, allErrorsPositions);
                }
        }

        private static IEnumerable<int[]> GenerateErrorsPositions(int n, int errorsCount)
        {
            var allErrorsPositions = new List<int[]>();
            var errorsPositions = new int[errorsCount];

            GenerateErrorsPositions(n, errorsPositions, 0, allErrorsPositions);
            return allErrorsPositions;
        }

        private static void PlaceNoiseIntoSamplesAndDecode(AnalyzingSample sample, int currentErrorNumber,
            int n, int k, int d, Polynomial generatingPolynomial, GsBasedDecoder decoder)
        {
            if (currentErrorNumber == sample.ErrorPositions.Length)
            {
                var decodingResults = decoder.Decode(n, k, d, generatingPolynomial, sample.Codeword, sample.CorrectValuesCount);
                if (decodingResults.Contains(sample.InformationPolynomial) == false)
                    throw new InvalidOperationException("Failed to decode sample");

                if (++sample.ProcessedNoises%50 == 0)
                {
                    var errors = new int[n];
                    for (var i = 0; i < sample.ErrorPositions.Length; i++)
                        errors[sample.ErrorPositions[i]] = sample.CurrentNoiseValue[i];
                    _logger.LogInformation($"[{Thread.CurrentThread.ManagedThreadId}]: Current noise value ({string.Join(",", errors)})");
                }
                if (decoder.TelemetryCollector.ProcessedSamplesCount%100 == 0)
                    _logger.LogInformation(decoder.TelemetryCollector.ToString());
                return;
            }

            var field = sample.InformationPolynomial.Field;
            var corretValue = sample.Codeword[sample.ErrorPositions[currentErrorNumber]];

            for (var i = sample.CurrentNoiseValue[currentErrorNumber]; i < field.Order; i++)
            {
                sample.Codeword[sample.ErrorPositions[currentErrorNumber]] =
                    Tuple.Create(corretValue.Item1, corretValue.Item2 + field.CreateElement(i));
                sample.CurrentNoiseValue[currentErrorNumber] = i;

                PlaceNoiseIntoSamplesAndDecode(sample, currentErrorNumber + 1, n, k, d, generatingPolynomial, decoder);
            }

            sample.Codeword[sample.ErrorPositions[currentErrorNumber]] = corretValue;
            sample.CurrentNoiseValue[currentErrorNumber] = 1;
        }

        private static void AnalyzeCode(int n, int k, int d, Polynomial h,
            int? placedErrorsCount = null, int? minCorrectValuesCount = null,
            int? samplesCount = null, int? decodingThreadsCount = null)
        {
            var maxErrorsCount = (int) Math.Floor(n - Math.Sqrt(n*(n - d)));
            var errorsCount = placedErrorsCount ?? maxErrorsCount;
            if (errorsCount > maxErrorsCount)
                throw new ArgumentException("Errors count is too large");
            if (errorsCount < d - maxErrorsCount)
                throw new ArgumentException("Errors count is too small");

            var correctValuesCount = minCorrectValuesCount ?? n - errorsCount;
            if (correctValuesCount*correctValuesCount <= n*(n - d))
                throw new ArgumentException("Correct values count is too small for decoding");
            if (correctValuesCount > n - errorsCount)
                throw new ArgumentException($"Correct values count can't be larger than {n - errorsCount} for errors count {errorsCount}");
            if (correctValuesCount >= n - (d - 1)/2)
                throw new ArgumentException("List size will be always equal to 1");

            var linearSystemsSolver = new GaussSolver();
            var generatingPolynomialBuilder = new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()),
                                                  linearSystemsSolver);
            var decoder =
                new GsBasedDecoder(
                    new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalcualtor()),
                        new RrFactorizator()),
                    linearSystemsSolver) {TelemetryCollector = new GsBasedDecoderTelemetryCollectorForGsBasedDecoder()};

            var generatingPolynomial = generatingPolynomialBuilder.Build(n, d, h);

            _logger.LogInformation("Start samples generation");
            var samplesGenerationTimer = Stopwatch.StartNew();
            var samples = GenerateSamples(n, k, generatingPolynomial, samplesCount).ToArray();
            samplesGenerationTimer.Stop();
            _logger.LogInformation("Samples were generated in {0} seconds", samplesGenerationTimer.Elapsed.TotalSeconds);

            _logger.LogInformation("Start errors positions generation");
            var errorsPositionsGenerationTimer = Stopwatch.StartNew();
            var errorsPositions = GenerateErrorsPositions(n, errorsCount).ToArray();
            errorsPositionsGenerationTimer.Stop();
            _logger.LogInformation("Errors positions were generated in {0} seconds", errorsPositionsGenerationTimer.Elapsed.TotalSeconds);

            _logger.LogInformation("Start noise decoding");
            var noiseGenerationTimer = Stopwatch.StartNew();
            samples = samples.SelectMany(x => errorsPositions.Select(y =>
                                                                         new AnalyzingSample(x)
                                                                         {
                                                                             ErrorPositions = y,
                                                                             CurrentNoiseValue = Enumerable.Repeat(1, errorsCount).ToArray(),
                                                                             CorrectValuesCount = correctValuesCount
                                                                         }))
                .ToArray();
            Parallel.ForEach(samples,
                new ParallelOptions {MaxDegreeOfParallelism = decodingThreadsCount ?? (int) (Environment.ProcessorCount*1.5d)},
                x => PlaceNoiseIntoSamplesAndDecode(x, 0, n, k, d, generatingPolynomial, decoder));
            noiseGenerationTimer.Stop();
            _logger.LogInformation("Noise decoding was performed in {0} seconds", noiseGenerationTimer.Elapsed.TotalSeconds);
        }

        private static void AnalyzeSamples(int n, int k, int d, Polynomial h, params AnalyzingSample[] samples)
        {
            var linearSystemsSolver = new GaussSolver();
            var generatingPolynomialBuilder = new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()),
                                                  linearSystemsSolver);
            var decoder =
                new GsBasedDecoder(
                        new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalcualtor()),
                            new RrFactorizator()),
                        linearSystemsSolver)
                    {TelemetryCollector = new GsBasedDecoderTelemetryCollectorForGsBasedDecoder()};

            var generatingPolynomial = generatingPolynomialBuilder.Build(n, d, h);
            _logger.LogInformation("Generating polynomial was restored");

            _logger.LogInformation("Samples analysis was started");
            Parallel.ForEach(samples,
                new ParallelOptions {MaxDegreeOfParallelism = Math.Min((int) (Environment.ProcessorCount*1.5d), samples.Length)},
                x => PlaceNoiseIntoSamplesAndDecode(x, 0, n, k, d, generatingPolynomial, decoder));
        }

        private static void AnalyzeCodeN26K13D12()
        {
            try
            {
                AnalyzeCode(26, 13, 12,
                    new Polynomial(new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1)),
                        0, 0, 0, 1, 2, 3, 4, 1, 6, 7, 8, 9, 1, 10, 1, 12, 1, 14, 1, 17, 1, 19, 20, 1, 1, 1, 22),
                    placedErrorsCount: 6, minCorrectValuesCount: 20, samplesCount: 1, decodingThreadsCount: 2);
            }
            catch (Exception exception)
            {
                _logger.LogError(0, exception, "Exception occurred during analysis");
                throw;
            }
        }

        private static void AnalyzeSamplesForN26K13D12Code()
        {
            var field = new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1));
            var informationPolynomial = new Polynomial(field);
            var encoder = new Encoder();
            var samples = new[]
                          {
                              new AnalyzingSample(informationPolynomial, encoder.Encode(26, informationPolynomial, informationPolynomial))
                              {
                                  ErrorPositions = new[] {2, 6, 8, 15, 16, 22},
                                  CurrentNoiseValue = new[] {1, 1, 6, 4, 4, 14},
                                  CorrectValuesCount = 20
                              },
                              new AnalyzingSample(informationPolynomial, encoder.Encode(26, informationPolynomial, informationPolynomial))
                              {
                                  ErrorPositions = new[] {0, 1, 2, 3, 4, 5},
                                  CurrentNoiseValue = new[] {1, 1, 6, 4, 4, 14},
                                  CorrectValuesCount = 20
                              }
                          };

            try
            {
                AnalyzeSamples(26, 13, 12,
                    new Polynomial(field, 0, 0, 0, 1, 2, 3, 4, 1, 6, 7, 8, 9, 1, 10, 1, 12, 1, 14, 1, 17, 1, 19, 20, 1, 1, 1, 22),
                    samples);
            }
            catch (Exception exception)
            {
                _logger.LogError(0, exception, "Exception occurred during analysis");
                throw;
            }
        }

        [UsedImplicitly]
        public static void Main()
        {
            NLog.LogManager.Configuration = new XmlLoggingConfiguration("./nlog.config", true);
            var loggerFactory = new LoggerFactory()
                .AddNLog()
                .AddConsole();
            _logger = loggerFactory.CreateLogger<Program>();

            AnalyzeCodeN26K13D12();

            Console.ReadKey();
        }
    }
}
