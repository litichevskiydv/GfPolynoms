namespace WaveletCodesListDecodingAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.PolynomialsGcdFinder;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using RsCodesTools.ListDecoder;
    using RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using WaveletCodesTools.GeneratingPolynomialsBuilder;
    using WaveletCodesTools.ListDecoderForFixedDistanceCodes;

    [UsedImplicitly]
    public static class Program
    {
        private static void GenerateSamples(int n, Polynomial generatingPolynomial, Polynomial m,
            int[] informationWord, int informationWordPosition,
            ICollection<AnalyzingSample> samples, int? samplesCount = null)
        {
            if (informationWordPosition == informationWord.Length)
            {
                var informationPolynomial = new Polynomial(generatingPolynomial.Field, informationWord);
                var codeWordPolynomial = (informationPolynomial.RaiseVariableDegree(2)*generatingPolynomial)%m;

                var i = 0;
                var field = generatingPolynomial.Field;
                var codeword = new Tuple<FieldElement, FieldElement>[n];
                for (; i <= codeWordPolynomial.Degree; i++)
                    codeword[i] = new Tuple<FieldElement, FieldElement>(new FieldElement(field, field.GetGeneratingElementPower(i)),
                        new FieldElement(field, codeWordPolynomial[i]));
                for (; i < n; i++)
                    codeword[i] = new Tuple<FieldElement, FieldElement>(new FieldElement(field, field.GetGeneratingElementPower(i)),
                        field.Zero());

                samples.Add(new AnalyzingSample(informationPolynomial, codeword));
            }
            else
                for (var i = 0; i < generatingPolynomial.Field.Order; i++)
                {
                    if(samplesCount.HasValue && samplesCount.Value == samples.Count)
                        break;

                    informationWord[informationWordPosition] = i;
                    GenerateSamples(n, generatingPolynomial, m, informationWord, informationWordPosition + 1, samples, samplesCount);
                }
        }

        private static IEnumerable<AnalyzingSample> GenerateSamples(int n, int k, Polynomial generatingPolynomial, int? samplesCount = null)
        {
            var samples = new List<AnalyzingSample>();
            var informationWord = new int[k];

            var m = new Polynomial(generatingPolynomial.Field, 1).RightShift(n);
            m[0] = generatingPolynomial.Field.InverseForAddition(1);

            GenerateSamples(n, generatingPolynomial, m, informationWord, 0, samples, samplesCount);
            return samples;
        }

        private static void GenerateErrorsPositions(int n, IList<int> errorsPositions, int placedErrorsCount, ICollection<int[]> allErrorsPositions)
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

        private static void PlaceNoiseIntoSamplesAndDecode(AnalyzingSample sample, IReadOnlyList<int> errorsPositions, int currentErrorPosition, 
            int n, int k, int d, Polynomial generatingPolynomial, GsBasedDecoder decoder)
        {
            if (currentErrorPosition == errorsPositions.Count)
            {
                var decodingResults = decoder.Decode(n, k, d, generatingPolynomial, sample.Codeword,
                    sample.Codeword.Length - errorsPositions.Count);

                if (decodingResults.Contains(sample.InformationPolynomial) == false)
                    throw new InvalidOperationException("Failed to decode sample");

                if (decoder.TelemetryCollector.ProcessedSamplesCount%100 == 0)
                {
                    Console.WriteLine($"\nProcessed {decoder.TelemetryCollector.ProcessedSamplesCount} samples");
                    var listsSizes = decoder.TelemetryCollector.ProcessingResults.ToArray();
                    foreach (var listSize in listsSizes)
                        Console.WriteLine(
                            $"Frequency decoding list size {listSize.Key.Item1}, time decoding list size {listSize.Key.Item2}, {listSize.Value} samples");
                }
            }
            else
            {
                var field = sample.InformationPolynomial.Field;
                var corretValue = sample.Codeword[errorsPositions[currentErrorPosition]];
                for (var i = 0; i < field.Order; i++)
                {
                    if (corretValue.Item2.Representation == i)
                        continue;

                    sample.Codeword[errorsPositions[currentErrorPosition]] = new Tuple<FieldElement, FieldElement>(corretValue.Item1,
                        new FieldElement(field, i));
                    PlaceNoiseIntoSamplesAndDecode(sample, errorsPositions, currentErrorPosition + 1, n, k, d, generatingPolynomial, decoder);
                }
                sample.Codeword[errorsPositions[currentErrorPosition]] = corretValue;
            }
        }

        private static void PlaceNoiseIntoSamplesAndDecode(AnalyzingSample sample, IEnumerable<int[]> allErrorsPositions,
            int n, int k, int d, Polynomial generatingPolynomial, GsBasedDecoder decoder)
        {
            foreach (var errorsPositions in allErrorsPositions)
                PlaceNoiseIntoSamplesAndDecode(sample, errorsPositions, 0, n, k, d, generatingPolynomial, decoder);
        }

        private static void AnalyzeCode(int n, int k, int d, Polynomial h, int errorsCount, int? samplesCount = null)
        {
            var linearSystemsSolver = new GaussSolver();
            var generatingPolynomialBuilder = new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()), linearSystemsSolver);
            var decoder =
                new GsBasedDecoder(
                    new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalcualtor()),
                        new RrFactorizator()),
                    linearSystemsSolver) {TelemetryCollector = new GsBasedDecoderTelemetryCollectorForGsBasedDecoder()};

            var generatingPolynomial = generatingPolynomialBuilder.Build(n, d, h);

            Console.WriteLine("Start samples generation");
            var samplesGenerationTimer = Stopwatch.StartNew();
            var samples = GenerateSamples(n, k, generatingPolynomial, samplesCount).ToArray();
            samplesGenerationTimer.Stop();
            Console.WriteLine("Samples were generated in {0} seconds", samplesGenerationTimer.Elapsed.TotalSeconds);

            Console.WriteLine("Start errors positions generation");
            var errorsPositionsGenerationTimer = Stopwatch.StartNew();
            var errorsPositions = GenerateErrorsPositions(n, errorsCount).ToArray();
            errorsPositionsGenerationTimer.Stop();
            Console.WriteLine("Errors positions were generated in {0} seconds", errorsPositionsGenerationTimer.Elapsed.TotalSeconds);

            Console.WriteLine("Start noise decoding");
            var noiseGenerationTimer = Stopwatch.StartNew();
            Parallel.ForEach(samples,
                new ParallelOptions {MaxDegreeOfParallelism = (int) (Environment.ProcessorCount*1.5d)},
                x => PlaceNoiseIntoSamplesAndDecode(x, errorsPositions, n, k, d, generatingPolynomial, decoder));
            noiseGenerationTimer.Stop();
            Console.WriteLine("Noise decoding was performed in {0} seconds", noiseGenerationTimer.Elapsed.TotalSeconds);
        }

        [UsedImplicitly]
        public static void Main()
        {
            AnalyzeCode(8, 4, 4, new Polynomial(new PrimePowerOrderField(9, 3, new[] {1, 0, 1}), 1, 2, 3, 1, 2, 3, 2, 1), 2);
            Console.ReadKey();
        }
    }
}
