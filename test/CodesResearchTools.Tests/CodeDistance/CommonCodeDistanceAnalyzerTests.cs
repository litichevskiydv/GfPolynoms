﻿namespace AppliedAlgebra.CodesResearchTools.Tests.CodeDistance
{
    using Analyzers.CodeDistance;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.PolynomialsGcdFinder;
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Moq;
    using RsCodesTools;
    using RsCodesTools.Decoding.ListDecoder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using RsCodesTools.Decoding.StandartDecoder;
    using TestCases;
    using WaveletCodesTools;
    using WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes;
    using WaveletCodesTools.Decoding.StandartDecoderForFixedDistanceCodes;
    using WaveletCodesTools.FixedDistanceCodesFactory;
    using WaveletCodesTools.GeneratingPolynomialsBuilder;
    using Xunit;

    public class CommonCodeDistanceAnalyzerTests
    {
        [UsedImplicitly] public static TheoryData<CodeDistanceAnalyzerTestCase> CodeDistanceAnalyzysTestCases;

        static CommonCodeDistanceAnalyzerTests()
        {
            var gaussSolver = new GaussSolver();
            var codesFactory
                = new StandardCodesFactory(
                    new LiftingSchemeBasedBuilder(
                        new GcdBasedBuilder(new RecursiveGcdFinder()),
                        gaussSolver,
                        new GeneratingPolynomialsFactory()
                    ),
                    new RsBasedDecoder(new BerlekampWelchDecoder(gaussSolver), gaussSolver),
                    new GsBasedDecoder(
                        new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalculator()), new RrFactorizator()),
                        gaussSolver
                    )
                );
            var wN7K3D4 = codesFactory.CreateN7K3D4();
            var wN8K4D4First = new WaveletCode(8, 4, 4, new Polynomial(GaloisField.Create(9), 2, 0, 1, 2, 1, 1));
            var wN8K4D4Second = new WaveletCode(8, 4, 4, new Polynomial(GaloisField.Create(9), 2, 2, 1, 2, 0, 1));

            CodeDistanceAnalyzysTestCases
                = new TheoryData<CodeDistanceAnalyzerTestCase>
                  {
                      new CodeDistanceAnalyzerTestCase
                      {
                          Field = wN7K3D4.Field,
                          InformationWordLength = wN7K3D4.InformationWordLength,
                          EncodingProcedure = x => wN7K3D4.Encode(x),
                          Expected = wN7K3D4.CodeDistance
                      },
                      new CodeDistanceAnalyzerTestCase
                      {
                          Field = wN8K4D4First.Field,
                          InformationWordLength = wN8K4D4First.InformationWordLength,
                          EncodingProcedure = x => wN8K4D4First.Encode(x),
                          Expected = wN8K4D4First.CodeDistance
                      },
                      new CodeDistanceAnalyzerTestCase
                      {
                          Field = wN8K4D4Second.Field,
                          InformationWordLength = wN8K4D4Second.InformationWordLength,
                          EncodingProcedure = x => wN8K4D4Second.Encode(x),
                          Expected = wN8K4D4Second.CodeDistance
                      }
                  };
        }

        private readonly Mock<ILogger<CommonCodeDistanceAnalyzer>> _mockLogger;
        private readonly CommonCodeDistanceAnalyzer _analyzer;

        public CommonCodeDistanceAnalyzerTests()
        {
            _mockLogger = new Mock<ILogger<CommonCodeDistanceAnalyzer>>();
            _analyzer = new CommonCodeDistanceAnalyzer(new RecursiveIterator(), _mockLogger.Object);
        }

        [Theory]
        [MemberData(nameof(CodeDistanceAnalyzysTestCases))]
        public void ShouldAnalyzeCodeDistance(CodeDistanceAnalyzerTestCase testCase)
        {
            Assert.Equal(
                testCase.Expected,
                _analyzer.Analyze(testCase.Field, testCase.InformationWordLength, testCase.EncodingProcedure)
            );

            Assert.All(_mockLogger.Invocations, x => Assert.Equal(LogLevel.Information, x.Arguments[0]));
            Assert.True(_mockLogger.Invocations.Count >= 1);
        }

        [Fact]
        public void MustStopAnalysisBeforeRealDistanceWasCalculated()
        {
            // Given
            var gf2 = GaloisField.Create(2);
            var code = new ReedSolomonCode(gf2, 8, 4);
            const int codeDistanceMinimumThreshold = 7;

            // When
            var actualDistance = _analyzer.Analyze(
                gf2,
                code.InformationWordLength,
                informationWord => code.Encode(informationWord),
                new CodeDistanceAnalyzerOptions {CodeDistanceMinimumThreshold = codeDistanceMinimumThreshold, LoggingResolution = 1}
            );

            // Then
            Assert.True(actualDistance < codeDistanceMinimumThreshold);

            var informationWordsCount = gf2.Order.Pow(code.InformationWordLength);
            Assert.True(_mockLogger.Invocations.Count < (informationWordsCount - 1) * informationWordsCount / 2);

        }
    }
}