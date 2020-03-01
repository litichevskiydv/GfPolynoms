namespace AppliedAlgebra.CodesResearchTools.Tests.Spectrum
{
    using System;
    using System.Linq;
    using Analyzers.Spectrum;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Moq;
    using RsCodesTools.CodesFactory;
    using RsCodesTools.Decoding.ListDecoder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using RsCodesTools.Decoding.StandartDecoder;
    using TestCases;
    using Xunit;

    public class MdsCodesSpectrumAnalyzerTests
    {
        [UsedImplicitly]
        public static TheoryData<SpectrumAnalyzerConstructorParametersValidationTestCase> ConstructorParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<AnalyzeSpectrumParametersValidationTestCase> AnalyzeParametersValidationTestCases;

        static MdsCodesSpectrumAnalyzerTests()
        {
            ConstructorParametersValidationTestCases
                = new TheoryData<SpectrumAnalyzerConstructorParametersValidationTestCase>
                  {
                      new SpectrumAnalyzerConstructorParametersValidationTestCase {VariantsIterator = new Mock<IVariantsIterator>().Object},
                      new SpectrumAnalyzerConstructorParametersValidationTestCase {Logger = new Mock<ILogger<CommonSpectrumAnalyzer>>().Object}
                  };
            AnalyzeParametersValidationTestCases
                = new TheoryData<AnalyzeSpectrumParametersValidationTestCase>
                  {
                      new AnalyzeSpectrumParametersValidationTestCase {InformationWordLength = 2, EncodingProcedure = x => x},
                      new AnalyzeSpectrumParametersValidationTestCase {Field = GaloisField.Create(2), EncodingProcedure = x => x},
                      new AnalyzeSpectrumParametersValidationTestCase {Field = GaloisField.Create(2), InformationWordLength = 2}
                  };
        }

        private readonly StandardCodesFactory _rsCodesFactory;
        private readonly CommonSpectrumAnalyzer _commonAnalyzer;
        private readonly MdsCodesSpectrumAnalyzer _mdsCodesAnalyzer;

        public MdsCodesSpectrumAnalyzerTests()
        {
            _rsCodesFactory = new StandardCodesFactory(
                new BerlekampWelchDecoder(new GaussSolver()),
                new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalculator()), new RrFactorizator())
            );
            _commonAnalyzer = new CommonSpectrumAnalyzer(new RecursiveIterator(), new Mock<ILogger<CommonSpectrumAnalyzer>>().Object);
            _mdsCodesAnalyzer = new MdsCodesSpectrumAnalyzer();
        }

        [Theory]
        [MemberData(nameof(ConstructorParametersValidationTestCases))]
        public void ConstructorShouldValidateParameters(SpectrumAnalyzerConstructorParametersValidationTestCase testCase)
        {
            Assert.Throws<ArgumentNullException>(() => new CommonSpectrumAnalyzer(testCase.VariantsIterator, testCase.Logger));
        }

        [Fact]
        public void AnalyzeCodeSpectrumShouldValidateParameters()
        {
            Assert.Throws<ArgumentNullException>(() => _mdsCodesAnalyzer.Analyze(null));
        }

        [Theory]
        [MemberData(nameof(AnalyzeParametersValidationTestCases))]
        public void AnalyzeCodingProcedureSpectrumShouldValidateParameters(AnalyzeSpectrumParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => _mdsCodesAnalyzer.Analyze(testCase.Field, testCase.InformationWordLength, testCase.EncodingProcedure));
        }

        [Fact]
        public void ShouldAnalyzeCodeSpectrum()
        {
            // Given
            var code = _rsCodesFactory.Create(GaloisField.Create(9), 8, 5);

            // When
            var actualSpectrum = _mdsCodesAnalyzer.Analyze(code).OrderBy(x => x.Key).ToArray();

            // Then
            var expectedSpectrum = _commonAnalyzer.Analyze(code).OrderBy(x => x.Key).ToArray();
            Assert.Equal(expectedSpectrum, actualSpectrum);
        }

        [Fact]
        public void ShouldAnalyzeCodingProcedureSpectrum()
        {
            // Given
            var code = _rsCodesFactory.Create(GaloisField.Create(9), 8, 6);

            // When
            var actualSpectrum = _mdsCodesAnalyzer.Analyze(code.Field, code.InformationWordLength, x => code.Encode(x))
                .OrderBy(x => x.Key)
                .ToArray();

            // Then
            var expectedSpectrum = _commonAnalyzer.Analyze(code.Field, code.InformationWordLength, x => code.Encode(x))
                .OrderBy(x => x.Key)
                .ToArray();
            Assert.Equal(expectedSpectrum, actualSpectrum);
        }
    }
}