namespace AppliedAlgebra.CodesResearchTools.Tests
{
    using System;
    using Analyzers.CodeDistance;
    using CodesAbstractions;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.PolynomialsGcdFinder;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Moq;
    using RsCodesTools.Decoding.ListDecoder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using RsCodesTools.Decoding.StandartDecoder;
    using WaveletCodesTools;
    using WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes;
    using WaveletCodesTools.Decoding.StandartDecoderForFixedDistanceCodes;
    using WaveletCodesTools.FixedDistanceCodesFactory;
    using WaveletCodesTools.GeneratingPolynomialsBuilder;
    using Xunit;

    public class CodeDistanceAnalyzerTests
    {
        [UsedImplicitly] public static TheoryData<ICode> CodeDistanceAnalyzysTestsData;

        static CodeDistanceAnalyzerTests()
        {
            var gaussSolver = new GaussSolver();
            var codesFactory
                = new StandardCodesFactory(
                    new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()), gaussSolver),
                    new RsBasedDecoder(new BerlekampWelchDecoder(gaussSolver), gaussSolver),
                    new GsBasedDecoder(
                        new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalcualtor()), new RrFactorizator()),
                        gaussSolver
                    )
                );

            CodeDistanceAnalyzysTestsData
                = new TheoryData<ICode>
                  {
                      codesFactory.CreateN7K3D4(),
                      new WaveletCode(8, 4, 4, new Polynomial(new PrimePowerOrderField(9), 2, 0, 1, 2, 1, 1)),
                      new WaveletCode(8, 4, 4, new Polynomial(new PrimePowerOrderField(9), 2, 2, 1, 2, 0, 1))
                  };
        }

        private readonly Mock<ILogger<CodeDistanceAnalyzer>> _mockLogger;
        private readonly CodeDistanceAnalyzer _analyzer;

        public CodeDistanceAnalyzerTests()
        {
            _mockLogger = new Mock<ILogger<CodeDistanceAnalyzer>>();
            _analyzer = new CodeDistanceAnalyzer(_mockLogger.Object);
        }

        [Theory]
        [MemberData(nameof(CodeDistanceAnalyzysTestsData))]
        public void ShouldAnalyzeCodeDistance(ICode code)
        {
            Assert.Equal(code.CodeDistance, _analyzer.Analyze(code));

            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()
                ),
                Times.AtLeastOnce
            );
        }
    }
}