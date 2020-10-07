namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding
{
    using System;
    using System.Linq;
    using GfAlgorithms.Matrices;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator;
    using Xunit;

    public class MultilevelEncoderTests
    {
        public class MultilevelEncoderConstructorParametersValidationTestCase
        {
            public IIterationFiltersCalculator IterationFiltersCalculator { get; set; }

            public IWaveletCoefficientsGenerator WaveletCoefficientsGenerator { get; set; }

            public int LevelsCount { get; set; }

            public (FieldElement[] h, FieldElement[] g) SynthesisFilters { get; set; }
        }

        private readonly MultilevelEncoder _multilevelEncoder;

        [UsedImplicitly]
        public static TheoryData<MultilevelEncoderConstructorParametersValidationTestCase> MultilevelEncoderConstructorParametersValidationTestCases;
        [UsedImplicitly] 
        public static TheoryData<EncodeParametersValidationTestCase> EncodeParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<MultilevelEncodingTestCase> MultilevelEncodingTestCases;

        static MultilevelEncoderTests()
        {
            var gf3 = GaloisField.Create(3);
            MultilevelEncoderConstructorParametersValidationTestCases
                = new TheoryData<MultilevelEncoderConstructorParametersValidationTestCase>
                  {
                      new MultilevelEncoderConstructorParametersValidationTestCase(),
                      new MultilevelEncoderConstructorParametersValidationTestCase
                      {
                          IterationFiltersCalculator = new ConvolutionBasedCalculator()
                      },
                      new MultilevelEncoderConstructorParametersValidationTestCase
                      {
                          IterationFiltersCalculator = new ConvolutionBasedCalculator(),
                          WaveletCoefficientsGenerator = new NaiveGenerator()
                      },
                      new MultilevelEncoderConstructorParametersValidationTestCase
                      {
                          IterationFiltersCalculator = new ConvolutionBasedCalculator(),
                          WaveletCoefficientsGenerator = new NaiveGenerator(),
                          LevelsCount = 2,
                          SynthesisFilters = (gf3.CreateElementsVector(1, 1, 0, 0, 0, 0), gf3.CreateElementsVector(0, 1, 0, 0, 0, 0))
                      }
                  };
            EncodeParametersValidationTestCases
                = new TheoryData<EncodeParametersValidationTestCase>
                  {
                      new EncodeParametersValidationTestCase {CodewordLength = -1},
                      new EncodeParametersValidationTestCase {CodewordLength = 12},
                      new EncodeParametersValidationTestCase
                      {
                          CodewordLength = 12,
                          InformationWord = Enumerable.Repeat(gf3.Zero(), 13).ToArray()
                      },
                  };
            MultilevelEncodingTestCases
                = new TheoryData<MultilevelEncodingTestCase>
                  {
                      new MultilevelEncodingTestCase
                      {
                          CodewordLength = 12,
                          InformationWord = gf3.CreateElementsVector(1, 2, 2),
                          ExpectedCodeword = gf3.CreateElementsVector(1, 1, 0, 0, 2, 1, 0, 0, 2, 0, 1, 1)
                      }
                  };
        }

        public MultilevelEncoderTests()
        {
            var gf3 = GaloisField.Create(3);
            _multilevelEncoder = new MultilevelEncoder(
                new ConvolutionBasedCalculator(),
                new NaiveGenerator(
                    FieldElementsMatrix.CirculantMatrix(gf3.CreateElementsVector(0, 0, 0, 0, 0, 1)),
                    FieldElementsMatrix.CirculantMatrix(gf3.CreateElementsVector(0, 0, 1))
                ),
                2,
                (
                    gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                    gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                )
            );
        }

        [Theory]
        [MemberData(nameof(MultilevelEncoderConstructorParametersValidationTestCases))]
        public void MultilevelEncoderConstructorMustValidateParameters(MultilevelEncoderConstructorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => new MultilevelEncoder(
                    testCase.IterationFiltersCalculator,
                    testCase.WaveletCoefficientsGenerator,
                    testCase.LevelsCount,
                    testCase.SynthesisFilters
                )
            );
        }

        [Theory]
        [MemberData(nameof(EncodeParametersValidationTestCases))]
        public void EncodeMustValidateParameters(EncodeParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _multilevelEncoder.Encode(testCase.CodewordLength, testCase.InformationWord)
            );
        }

        [Theory]
        [MemberData(nameof(MultilevelEncodingTestCases))]
        public void MustPerformMultilevelEncoding(MultilevelEncodingTestCase testCase)
        {
            // When
            var actualCodeword = _multilevelEncoder.Encode(testCase.CodewordLength, testCase.InformationWord);

            // Then
            Assert.Equal(testCase.ExpectedCodeword, actualCodeword);
        }
    }
}