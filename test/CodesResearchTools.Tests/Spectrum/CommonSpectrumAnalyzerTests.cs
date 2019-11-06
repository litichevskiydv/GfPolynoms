namespace AppliedAlgebra.CodesResearchTools.Tests.Spectrum
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Analyzers.Spectrum;
    using CodesAbstractions;
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class CommonSpectrumAnalyzerTests
    {
        private class FakeCode : ICode
        {
            public GaloisField Field => new PrimeOrderField(2);
            public int CodewordLength => 3;
            public int InformationWordLength => 2;
            public int CodeDistance => 1;

            public FieldElement[] Encode(FieldElement[] informationWord)
            {
                return informationWord.Prepend(Field.Zero()).ToArray();
            }

            public FieldElement[] Decode(FieldElement[] noisyCodeword)
            {
                throw new NotImplementedException();
            }

            public IReadOnlyList<FieldElement[]> DecodeViaList(FieldElement[] noisyCodeword, int? listDecodingRadius = null)
            {
                throw new NotImplementedException();
            }
        }

        #region TestCases

        public class ConstructorParametersValidationTestCase
        {
            public IVariantsIterator VariantsIterator { get; set; }

            public ILogger<CommonSpectrumAnalyzer> Logger { get; set; }
        }

        public class AnalyzeParametersValidationTestCase
        {
            public GaloisField Field { get; set; }

            public int InformationWordLength { get; set; }

            public Func<FieldElement[], FieldElement[]> EncodingProcedure { get; set; }
        }

        #endregion

        [UsedImplicitly] 
        public static TheoryData<ConstructorParametersValidationTestCase> ConstructorParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<AnalyzeParametersValidationTestCase> AnalyzeParametersValidationTestCases;

        static CommonSpectrumAnalyzerTests()
        {
            ConstructorParametersValidationTestCases
                = new TheoryData<ConstructorParametersValidationTestCase>
                  {
                      new ConstructorParametersValidationTestCase {VariantsIterator = new Mock<IVariantsIterator>().Object},
                      new ConstructorParametersValidationTestCase {Logger = new Mock<ILogger<CommonSpectrumAnalyzer>>().Object}
                  };
            AnalyzeParametersValidationTestCases
                = new TheoryData<AnalyzeParametersValidationTestCase>
                  {
                      new AnalyzeParametersValidationTestCase {InformationWordLength = 2, EncodingProcedure = x => x},
                      new AnalyzeParametersValidationTestCase {Field = new PrimeOrderField(2), EncodingProcedure = x => x},
                      new AnalyzeParametersValidationTestCase {Field = new PrimeOrderField(2), InformationWordLength = 2}
                  };
        }

        private readonly Mock<ILogger<CommonSpectrumAnalyzer>> _mockLogger;
        private readonly CommonSpectrumAnalyzer _analyzer;

        public CommonSpectrumAnalyzerTests()
        {
            _mockLogger = new Mock<ILogger<CommonSpectrumAnalyzer>>();
            _analyzer = new CommonSpectrumAnalyzer(new RecursiveIterator(), _mockLogger.Object);
        }

        [Theory]
        [MemberData(nameof(ConstructorParametersValidationTestCases))]
        public void ConstructorShouldValidateParameters(ConstructorParametersValidationTestCase testCase)
        {
            Assert.Throws<ArgumentNullException>(() => new CommonSpectrumAnalyzer(testCase.VariantsIterator, testCase.Logger));
        }

        [Fact]
        public void AnalyzeCodeSpectrumShouldValidateParameters()
        {
            Assert.Throws<ArgumentNullException>(() => _analyzer.Analyze(null));
        }

        [Theory]
        [MemberData(nameof(AnalyzeParametersValidationTestCases))]
        public void AnalyzeCodingProcedureSpectrumShouldValidateParameters(AnalyzeParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => _analyzer.Analyze(testCase.Field, testCase.InformationWordLength, testCase.EncodingProcedure));
        }

        [Fact]
        public void ShouldAnalyzeCodeSpectrum()
        {
            // Given
            var code = new FakeCode();

            // When
            var actualSpectrum = _analyzer.Analyze(code, new SpectrumAnalyzerOptions {LoggingResolution = 1})
                .OrderBy(x => x.Key)
                .ToArray();

            // Then
            var expectedSpectrum = new[] {new KeyValuePair<int, long>(1, 2), new KeyValuePair<int, long>(2, 1)};
            Assert.Equal(expectedSpectrum, actualSpectrum);

            Assert.All(_mockLogger.Invocations, x => Assert.Equal(LogLevel.Information, x.Arguments[0]));
            Assert.Equal(3, _mockLogger.Invocations.Count);
        }

        [Fact]
        public void ShouldAnalyzeCodingProcedureSpectrum()
        {
            // Given
            var code = new FakeCode();

            // When
            var actualSpectrum = _analyzer.Analyze(
                    code.Field,
                    code.InformationWordLength,
                    x => code.Encode(x),
                    new SpectrumAnalyzerOptions {LoggingResolution = 1}
                )
                .OrderBy(x => x.Key)
                .ToArray();

            // Then
            var expectedSpectrum = new[] { new KeyValuePair<int, long>(1, 2), new KeyValuePair<int, long>(2, 1) };
            Assert.Equal(expectedSpectrum, actualSpectrum);

            Assert.All(_mockLogger.Invocations, x => Assert.Equal(LogLevel.Information, x.Arguments[0]));
            Assert.Equal(3, _mockLogger.Invocations.Count);
        }
    }
}