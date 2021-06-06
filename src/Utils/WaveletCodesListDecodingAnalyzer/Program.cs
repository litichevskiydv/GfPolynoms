namespace AppliedAlgebra.WaveletCodesListDecodingAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using CodesAbstractions;
    using CodesResearchTools.Analyzers.CodeDistance;
    using CodesResearchTools.Analyzers.CodeSpaceCovering;
    using CodesResearchTools.Analyzers.ListsSizesDistribution;
    using CodesResearchTools.NoiseGenerator;
    using Extensions;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.Matrices;
    using GfAlgorithms.PolynomialsGcdFinder;
    using GfAlgorithms.VariantsIterator;
    using GfAlgorithms.WaveletTransform;
    using GfAlgorithms.WaveletTransform.FiltersBanksIterator;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfAlgorithms.WaveletTransform.SourceFiltersCalculator;
    using GfPolynoms;
    using GfPolynoms.Comparers;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using RsCodesTools;
    using RsCodesTools.Decoding.ListDecoder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using RsCodesTools.Decoding.StandartDecoder;
    using Serilog;
    using WaveletCodesTools;
    using WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes;
    using WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes.GsBasedDecoderDependencies;
    using WaveletCodesTools.Decoding.StandartDecoderForFixedDistanceCodes;
    using WaveletCodesTools.Encoding;
    using WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.GeneratingMatrixProvider;
    using WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.InformationVectorProvider;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.LevelMatricesProvider;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator;
    using WaveletCodesTools.FixedDistanceCodesFactory;
    using WaveletCodesTools.GeneratingPolynomialsBuilder;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    [UsedImplicitly]
    public class Program
    {
        private static readonly IVariantsIterator VariantsIterator;
        private static readonly INoiseGenerator NoiseGenerator;
        private static readonly CommonCodeDistanceAnalyzer CommonCodeDistanceAnalyzer;
        private static readonly LinearCodeDistanceAnalyzer LinearCodeDistanceAnalyzer;
        private static readonly MinimalSphereCoveringAnalyzer MinimalSphereCoveringAnalyzer;
        private static readonly ListsSizesDistributionAnalyzer ListsSizesDistributionAnalyzer;
        private static readonly IEqualityComparer<FieldElement[]> WordsComparer;
        private static readonly IFixedDistanceCodesFactory FixedDistanceCodesFactory;
        private static readonly IGsBasedDecoderTelemetryCollector TelemetryCollector;

        private static readonly ILogger Logger;

        private static int AnalyzeCodeDistance(
            int maxDegreeOfParallelism,
            GaloisField field,
            int codewordLength,
            int informationWordLength,
            IMultilevelEncoder encoder,
            int? codeDistanceLimit = null,
            bool logResult = true
        )
        {
            var encoderOptions = new MultilevelEncoderOptions {MaxDegreeOfParallelism = 1};
            var analyzer = encoder is LinearMultilevelEncoder
                ? (ICodeDistanceAnalyzer) LinearCodeDistanceAnalyzer
                : CommonCodeDistanceAnalyzer;
            var codeDistance = analyzer.Analyze(
                field,
                informationWordLength,
                informationWord => encoder.Encode(codewordLength, informationWord, encoderOptions),
                new CodeDistanceAnalyzerOptions
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism,
                    LoggingResolution = 1000000000L,
                    CodeDistanceMinimumThreshold = codeDistanceLimit
                }
            );

            if (logResult)
                Logger.LogInformation("Code distance: {codeDistance}", codeDistance);

            return codeDistance;
        }

        private static int AnalyzeCodeDistance(int codewordLength, int informationWordLength, Polynomial generatingPolynomial, bool logResult = true)
        {
            var field = generatingPolynomial.Field;
            var modularPolynomial = new Polynomial(field, 1).RightShift(codewordLength) + new Polynomial(field, field.InverseForAddition(1));

            var codeDistance = LinearCodeDistanceAnalyzer.Analyze(
                field,
                informationWordLength,
                x => (new Polynomial(x).RaiseVariableDegree(2) * generatingPolynomial % modularPolynomial).GetCoefficients(codewordLength - 1),
                new CodeDistanceAnalyzerOptions { LoggingResolution = 1000000000L }
            );
            if (logResult)
                Logger.LogInformation("Code distance: {codeDistance}", codeDistance);

            return codeDistance;

        }

        private static int AnalyzeCodeDistance(int informationWordLength, params Polynomial[] generatingPolynomials)
        {
            var field = generatingPolynomials[0].Field;
            var one = new Polynomial(field, 1);
            var codeDistance = LinearCodeDistanceAnalyzer.Analyze(
                field,
                informationWordLength,
                x =>
                {
                    var codeword = new Polynomial(x);
                    var codewordLength = informationWordLength;
                    foreach (var generatingPolynomial in generatingPolynomials)
                    {
                        codewordLength *= 2;
                        var modularPolynomial = (one >> codewordLength) - one;
                        codeword = codeword.RaiseVariableDegree(2) * generatingPolynomial % modularPolynomial;
                    }

                    return codeword.GetCoefficients(codewordLength - 1);
                },
                new CodeDistanceAnalyzerOptions { LoggingResolution = 1000000000L }
            );

            return codeDistance;
        }

        private static int AnalyzeMinimalSphereCovering(ICode code)
        {
            var minimalRadius = MinimalSphereCoveringAnalyzer.Analyze(code);
            Logger.LogInformation("Minimal radius: {minimalRadius}", minimalRadius);
            return minimalRadius;
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

        private static Polynomial FindNotFixedDistanceWaveletCode(GaloisField field, int codeDistance)
        {
            var codewordLength = field.Order - 1;
            var informationWordLength = codewordLength / 2;


            var variantsIterator = new RecursiveIterator();
            foreach (var generatingPolynomial in variantsIterator.IteratePolynomials(field, field.Order - 1).Skip(1))
                if (AnalyzeCodeDistance(codewordLength, informationWordLength, generatingPolynomial) == codeDistance
                    && generatingPolynomial.GetSpectrum(codewordLength - 1).Count(x => x.Representation == 0) == 0)
                {
                    Logger.LogInformation("Generating polynomial: {generatingPolynomial}", generatingPolynomial);
                    return generatingPolynomial;
                }

            return null;
        }

        private static void AnalyzeErrorCorrectingRate()
        {
            var field = GaloisField.Create(3);
            var one = new Polynomial(field, 1);

            const int codewordLength1 = 12;
            const int informationWordLength1 = codewordLength1 / 2;
            var modularPolynomial1 = (one >> codewordLength1) - one;

            const int codewordLength2 = informationWordLength1;
            const int informationWordLength2 = codewordLength2 / 2;
            var modularPolynomial2 = (one >> codewordLength2) - one;

            var iterationFiltersCalculator = new ConvolutionBasedCalculator();
            var complementaryFiltersBuilder = new GcdBasedBuilder(new RecursiveGcdFinder());
            foreach (var h1 in VariantsIterator.IteratePolynomials(field, codewordLength1 - 1).Skip(1))
            {
                Polynomial g1;
                try
                {
                    g1 = complementaryFiltersBuilder.Build(h1, codewordLength1);
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
                var generatingPolynomial1 = (h1 + (g1 >> 2)) % modularPolynomial1;

                var h2 = iterationFiltersCalculator.GetIterationFilter(2, h1, codewordLength1 - 1);
                var g2 = iterationFiltersCalculator.GetIterationFilter(2, g1, codewordLength1 - 1);
                var generatingPolynomial2 = (h2 + (g2 >> 2)) % modularPolynomial2;

                var relativeCodeDistance2 = AnalyzeCodeDistance(codewordLength2, informationWordLength2, generatingPolynomial2, false) / (double)codewordLength2;
                var relativeCodeDistance1 = AnalyzeCodeDistance(informationWordLength2, generatingPolynomial2, generatingPolynomial1) / (double)codewordLength1;
                if (relativeCodeDistance1 > relativeCodeDistance2 && relativeCodeDistance1 / relativeCodeDistance2 > 1.8)
                    Logger.LogInformation("Filter h: {h1}, filter g: {g1}, one iteration distance {relativeCodeDistance2}, two iterations distance {relativeCodeDistance1}", h1, g1, relativeCodeDistance2, relativeCodeDistance1);
            }
        }

        private static Timer InitializeStateLoggingTimer(IEnumerator<FiltersBankVectors> filtersBanksEnumerator, TimeSpan period) =>
            new Timer(
                state =>
                {
                    var enumerator = (IEnumerator<FiltersBankVectors>) state;
                    if (enumerator?.Current == null) return;

                    Logger.LogDebug("Processing filters bank ({filtersBank})", enumerator.Current);
                },
                filtersBanksEnumerator,
                period,
                period
            );

        private static void FindWaveletTransformsFiltersBanksForMultilevelEncoding(
            int maxDegreeOfParallelism,
            ISourceFiltersCalculator sourceFiltersCalculator,
            GaloisField field,
            int currentLevel,
            int currentLevelFiltersLength,
            FiltersBankVectors[] initialLevelsFiltersBanks,
            FiltersBankVectors[] currentLevelsFiltersBanks,
            int encodingLevelsCount,
            int codewordLength,
            int informationWordLength,
            int codeDistanceLimit,
            Func<FiltersBankVectors[], ILevelMatricesProvider> levelMatricesProvidersFactory,
            Func<ILevelMatricesProvider, int, IMultilevelEncoder> encodersFactory
        )
        {
            if (currentLevel == currentLevelsFiltersBanks.Length)
            {
                var encoder = encodersFactory(levelMatricesProvidersFactory(currentLevelsFiltersBanks), encodingLevelsCount);
                if (encoder is MultilevelEncoder)
                {
                    var (_, (zeroLevelH, zeroLevelG)) = currentLevelsFiltersBanks[0];
                    var requiredZerosCount = zeroLevelH.Length - codewordLength;
                    if (requiredZerosCount > 0)
                    {
                        var gMatrix = FieldElementsMatrix.DoubleCirculantMatrix(zeroLevelG);
                        var equationsRowsRange = Enumerable.Range(gMatrix.RowsCount - requiredZerosCount, requiredZerosCount).ToArray();
                        var variablesColumnsRange = Enumerable.Range(gMatrix.ColumnsCount - requiredZerosCount, requiredZerosCount).ToArray();

                        if (gMatrix.CreateSubmatrix(equationsRowsRange, variablesColumnsRange).CalculateDeterminant().Representation == 0)
                            return;
                    }
                }

                var codeDistance = AnalyzeCodeDistance(
                    maxDegreeOfParallelism,
                    field,
                    codewordLength,
                    informationWordLength,
                    encoder,
                    codeDistanceLimit,
                    false
                );
                if (codeDistance >= codeDistanceLimit)
                    for (var levelNumber = 0; levelNumber < currentLevelsFiltersBanks.Length; levelNumber++)
                        Logger.LogInformation(
                            "Level {levelNumber} filters bank {filtersBank}, code distance: {codeDistance}",
                            levelNumber,
                            currentLevelsFiltersBanks[levelNumber],
                            codeDistance
                        );

                return;
            }

            if (currentLevel == 0)
                Logger.LogInformation(
                    "Begin of search for wavelet transforms over field {field} for level {currentLevel} " +
                    "length {filtersLength} for code with {encodingLevelsCount} levels of transformation, " +
                    "codeword length {codewordLength}, information word length {informationWordLength} " +
                    "and code distance greater or equal than {codeDistanceLimit}",
                    field,
                    currentLevel,
                    currentLevelFiltersLength,
                    encodingLevelsCount,
                    codewordLength,
                    informationWordLength,
                    codeDistanceLimit
                );

            var filtersBanksIterator = new PerfectReconstructionFiltersBanksIterator(VariantsIterator, sourceFiltersCalculator);
            using (var filtersBanksEnumerator = filtersBanksIterator.IterateFiltersBanksVectors(field, currentLevelFiltersLength, initialLevelsFiltersBanks[currentLevel]).Skip(1).GetEnumerator())
            using (InitializeStateLoggingTimer(filtersBanksEnumerator, TimeSpan.FromMinutes(15)))
                while (filtersBanksEnumerator.MoveNext())
                {
                    currentLevelsFiltersBanks[currentLevel] = filtersBanksEnumerator.Current;
                    FindWaveletTransformsFiltersBanksForMultilevelEncoding(
                        maxDegreeOfParallelism,
                        sourceFiltersCalculator,
                        field,
                        currentLevel + 1,
                        currentLevelFiltersLength / 2,
                        initialLevelsFiltersBanks,
                        currentLevelsFiltersBanks,
                        encodingLevelsCount,
                        codewordLength,
                        informationWordLength,
                        codeDistanceLimit,
                        levelMatricesProvidersFactory,
                        encodersFactory
                    );
                }

            initialLevelsFiltersBanks[currentLevel] = null;
            if (currentLevel == 0)
                Logger.LogInformation(
                    "End of search for wavelet transforms over field {field} for level {currentLevel} " +
                    "length {filtersLength} for code with {encodingLevelsCount} levels of transformation, " +
                    "codeword length {codewordLength}, information word length {informationWordLength} " +
                    "and code distance greater or equal than {codeDistanceLimit}",
                    field,
                    currentLevel,
                    currentLevelFiltersLength,
                    encodingLevelsCount,
                    codewordLength,
                    informationWordLength,
                    codeDistanceLimit
                );
        }

        private static void FindWaveletTransformsForRecursiveLinearMultilevelEncoding(
            int maxDegreeOfParallelism,
            ISourceFiltersCalculator sourceFiltersCalculator,
            GaloisField field,
            int filtersLength,
            int codewordLength,
            int informationWordLength,
            int levelsCount,
            int codeDistanceLimit,
            FiltersBankVectors initialFiltersBank = null
        ) => FindWaveletTransformsFiltersBanksForMultilevelEncoding(
            maxDegreeOfParallelism,
            sourceFiltersCalculator,
            field,
            0,
            filtersLength,
            new[] {initialFiltersBank},
            new FiltersBankVectors[1],
            levelsCount,
            codewordLength,
            informationWordLength,
            codeDistanceLimit,
            currentLevelsFiltersBanks =>
                new RecursionBasedProvider(
                    new ConvolutionBasedCalculator(),
                    levelsCount,
                    currentLevelsFiltersBanks[0].SynthesisPair
                ),
            (levelMatricesProvider, encodingLevelsCount) =>
                new LinearMultilevelEncoder(
                    new CanonicalProvider(levelMatricesProvider),
                    new DetailsAbsenceBasedProvider(encodingLevelsCount),
                    new BasicCodewordMutator(),
                    encodingLevelsCount
                )
        );

        private static void FindWaveletTransformsForStackBasedMultilevelEncoding(
            int maxDegreeOfParallelism,
            ISourceFiltersCalculator sourceFiltersCalculator,
            GaloisField field,
            int filtersLength,
            int codewordLength,
            int informationWordLength,
            int levelsCount,
            int codeDistanceLimit,
            FiltersBankVectors[] initialLevelsFiltersBanks = null
        ) => FindWaveletTransformsFiltersBanksForMultilevelEncoding(
            maxDegreeOfParallelism,
            sourceFiltersCalculator,
            field,
            0,
            filtersLength,
            initialLevelsFiltersBanks ?? new FiltersBankVectors[levelsCount],
            new FiltersBankVectors[levelsCount],
            levelsCount,
            codewordLength,
            informationWordLength,
            codeDistanceLimit,
            currentLevelsFiltersBanks =>
                new StackBasedProvider(
                    currentLevelsFiltersBanks.Select(
                        x =>
                        (
                            FieldElementsMatrix.DoubleCirculantMatrix(x.SynthesisPair.h).Transpose(),
                            FieldElementsMatrix.DoubleCirculantMatrix(x.SynthesisPair.g).Transpose()
                        )
                    ).ToArray()
                ),
            (levelMatricesProvider, encodingLevelsCount) =>
                new MultilevelEncoder(
                    levelMatricesProvider,
                    new CanonicalGenerator(),
                    new LinearEquationsBasedCorrector(new GaussSolver()),
                    encodingLevelsCount
                )
        );

        private static void AnalyzeCodeDistanceForN3K2() =>
            AnalyzeCodeDistance(3, 2, new Polynomial(GaloisField.Create(4, new[] {1, 1, 1}), 2, 1));

        private static void AnalyzeCodeDistanceForN26K13() =>
            AnalyzeCodeDistance(26, 13, new Polynomial(GaloisField.Create(27), 2, 0, 1, 2, 1, 1));

        private static void AnalyzeCodeDistanceForN24K12() =>
            AnalyzeCodeDistance(24, 12,
                new Polynomial(GaloisField.Create(2), 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 0, 1)
            );

        private static void AnalyzeCodeDistanceForN8K4() =>
            AnalyzeCodeDistance(8, 4,
                new Polynomial(GaloisField.Create(2), 1, 1, 1, 0, 0, 0, 1)
            );

        private static void AnalyzeCodeDistanceForN16K4() =>
            AnalyzeCodeDistance(4,
                new Polynomial(GaloisField.Create(2), 1, 1, 1, 0, 0, 0, 1),
                new Polynomial(GaloisField.Create(2), 1, 1, 0, 0, 0, 1, 0, 0, 0, 1)
            );

        private static void AnalyzeCodeDistanceForN6K3() =>
            AnalyzeCodeDistance(6, 3,
                new Polynomial(GaloisField.Create(3), 1, 1, 0, 1)
            );

        private static void AnalyzeCodeDistanceForN12K3() =>
            AnalyzeCodeDistance(3,
                new Polynomial(GaloisField.Create(3), 1, 1, 0, 1),
                new Polynomial(GaloisField.Create(3), 1, 0, 0, 1, 0, 0, 0, 1)
            );

        private static void AnalyzeCodeDistanceForN9K5()
        {
            var gf3 = GaloisField.Create(3);
            const int levelsCount = 2;
            var encoder = new MultilevelEncoder(
                new RecursionBasedProvider(
                    new ConvolutionBasedCalculator(),
                    levelsCount,
                    (
                        gf3.CreateElementsVector(2, 1, 1, 2, 0, 1, 2, 2, 0, 0, 0, 0),
                        gf3.CreateElementsVector(0, 2, 0, 2, 1, 1, 0, 0, 0, 0, 0, 0)
                    )
                ),
                new CanonicalGenerator(),
                new LinearEquationsBasedCorrector(new GaussSolver()),
                levelsCount
            );
            AnalyzeCodeDistance(Process.GetCurrentProcess().ConstrainProcessorUsage(1, 0.8), gf3, 9, 5, encoder);
        }

        private static void AnalyzeMinimalSphereCoveringForN8K4D4() =>
            AnalyzeMinimalSphereCovering(
                new WaveletCode(8, 4, 4, new Polynomial(GaloisField.Create(9), 2, 0, 1, 2, 1, 1))
            );

        private static void AnalyzeSpherePackingsForRsN7K3() =>
            AnalyzeSpherePackings(new ReedSolomonCode(GaloisField.Create(8, new[] {1, 1, 0, 1}), 7, 3));

        private static void AnalyzeSpherePackingsForRsN7K4() =>
            AnalyzeSpherePackings(new ReedSolomonCode(GaloisField.Create(8, new[] {1, 1, 0, 1}), 7, 4));

        private static void AnalyzeSpherePackingsForN7K3D4First() =>
            AnalyzeSpherePackings(
                new WaveletCode(7, 3, 4, new Polynomial(GaloisField.Create(8, new[] {1, 1, 0, 1}), 0, 0, 2, 5, 6, 0, 1))
            );

        private static void AnalyzeSpherePackingsForN7K3D4Second() =>
            AnalyzeSpherePackings(
                new WaveletCode(7, 3, 4, new Polynomial(GaloisField.Create(8, new[] {1, 1, 0, 1}), 1, 2, 1, 1))
            );

        private static void AnalyzeSpherePackingsForN8K4D4Golay() =>
            AnalyzeSpherePackings(
                new WaveletCode(8, 4, 4, new Polynomial(GaloisField.Create(9), 2, 0, 1, 2, 1, 1))
            );

        private static void AnalyzeSpherePackingsForN8K4D4() =>
            AnalyzeSpherePackings(
                new WaveletCode(8, 4, 4, new Polynomial(GaloisField.Create(9), 2, 8, 3, 8, 0, 6, 2, 7))
            );

        private static void AnalyzeSpherePackingsForRsN8K4() =>
            AnalyzeSpherePackings(new ReedSolomonCode(GaloisField.Create(9), 8, 4));

        private static void AnalyzeSpherePackingsForRsN8K5() =>
            AnalyzeSpherePackings(new ReedSolomonCode(GaloisField.Create(9), 8, 5));

        private static void AnalyzeSpherePackingsForN10K5D5() =>
            AnalyzeSpherePackings(
                new WaveletCode(
                    10, 5, 5,
                    new Polynomial(GaloisField.Create(11), 8, 10, 4, 6, 8, 9, 2, 10, 4, 5)
                )
            );

        private static void AnalyzeSpherePackingsForN24K12D8() =>
            AnalyzeSpherePackings(
                new WaveletCode(24, 12, 8, new Polynomial(GaloisField.Create(2), 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 0, 1))
            );

        private static void AnalyzeCodeN7K3D4() => AnalyzeCode(FixedDistanceCodesFactory.CreateN7K3D4(), 2, 2);

        private static void AnalyzeCodeN8K4D4() => AnalyzeCode(FixedDistanceCodesFactory.CreateN8K4D4(), 2, 2);

        private static void AnalyzeCodeN15K7D8() => AnalyzeCode(FixedDistanceCodesFactory.CreateN15K7D8(), 4, 2);

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
                    ErrorsValues = new[] {1, 1, 12, 13, 18, 9, 19, 22},
                    CorrectValuesCount = 23
                },
                new AnalyzingSample(FixedDistanceCodesFactory.CreateN31K15D15())
                {
                    ErrorsPositions = new[] {0, 1, 2, 3, 4, 5, 6, 7},
                    ErrorsValues = new[] {1, 3, 1, 8, 25, 5, 8, 20},
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
                var field = GaloisField.Create(3);
                FindWaveletTransformsForRecursiveLinearMultilevelEncoding(
                    Process.GetCurrentProcess().ConstrainProcessorUsage(2, 0.7),
                    new BiorthogonalSourceFiltersCalculator(new GcdBasedBuilder(new RecursiveGcdFinder())),
                    field,
                    16,
                    16,
                    8,
                    4,
                    5/*,
                    new FiltersBankVectors(
                        (null, null),
                        (field.CreateElementsVector(1, 2, 1, 2, 0, 1, 0, 0, 1, 2, 1, 2, 0, 1, 0, 0), null)
                    )*/
                );
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

            VariantsIterator = new RecursiveIterator();
            NoiseGenerator = new RecursiveGenerator();
            WordsComparer = new FieldElementsArraysComparer();

            CommonCodeDistanceAnalyzer = new CommonCodeDistanceAnalyzer(VariantsIterator, loggerFactory.CreateLogger<CommonCodeDistanceAnalyzer>());
            LinearCodeDistanceAnalyzer = new LinearCodeDistanceAnalyzer(VariantsIterator, loggerFactory.CreateLogger<LinearCodeDistanceAnalyzer>());
            MinimalSphereCoveringAnalyzer = new MinimalSphereCoveringAnalyzer(VariantsIterator, loggerFactory.CreateLogger<MinimalSphereCoveringAnalyzer>());
            ListsSizesDistributionAnalyzer = new ListsSizesDistributionAnalyzer(VariantsIterator, loggerFactory.CreateLogger<ListsSizesDistributionAnalyzer>());

            var gaussSolver = new GaussSolver();
            TelemetryCollector = new GsBasedDecoderTelemetryCollectorForGsBasedDecoder();
            FixedDistanceCodesFactory
                = new StandardCodesFactory(
                    new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()), gaussSolver),
                    new RsBasedDecoder(new BerlekampWelchDecoder(gaussSolver), gaussSolver),
                    new GsBasedDecoder(
                        new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalculator()), new RrFactorizator()),
                        gaussSolver
                    )
                    {TelemetryCollector = TelemetryCollector}
                );
        }
    }
}
