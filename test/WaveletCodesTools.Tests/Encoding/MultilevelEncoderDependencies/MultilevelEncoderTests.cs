namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.MultilevelEncoderDependencies
{
    using System;
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

    public class MultilevelEncodingTestCase : EncodeParametersValidationTestCase
    {
        public FieldElement[] ExpectedCodeword { get; set; }
    }

    public class MultilevelEncoderTests
    {
        private readonly MultilevelEncoder _multilevelEncoder;

        [UsedImplicitly] 
        public static TheoryData<EncodeParametersValidationTestCase> EncodeParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<MultilevelEncodingTestCase> MultilevelEncodingTestCases;

        static MultilevelEncoderTests()
        {
            var gf3 = GaloisField.Create(3);
            EncodeParametersValidationTestCases
                = new TheoryData<EncodeParametersValidationTestCase>
                  {
                      new EncodeParametersValidationTestCase {CodewordLength = -1},
                      new EncodeParametersValidationTestCase {CodewordLength = 12, LevelsCount = -1},
                      new EncodeParametersValidationTestCase {CodewordLength = 12, LevelsCount = 2},
                      new EncodeParametersValidationTestCase
                      {
                          CodewordLength = 12,
                          LevelsCount = 2,
                          SynthesisFilters = (gf3.CreateElementsVector(1, 1, 0, 0, 0, 0), gf3.CreateElementsVector(0, 1, 0, 0, 0, 0))
                      },
                      new EncodeParametersValidationTestCase
                      {
                          CodewordLength = 12,
                          LevelsCount = 2,
                          SynthesisFilters
                              = (
                                  gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                                  gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                              )
                      }
                  };
            MultilevelEncodingTestCases
                = new TheoryData<MultilevelEncodingTestCase>
                  {
                      new MultilevelEncodingTestCase
                      {
                          CodewordLength = 12,
                          LevelsCount = 2,
                          SynthesisFilters
                              = (
                                  gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                                  gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                              ),
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
                )
            );
        }

        [Theory]
        [MemberData(nameof(EncodeParametersValidationTestCases))]
        public void EncodeMustValidateParameters(EncodeParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _multilevelEncoder.Encode(testCase.CodewordLength, testCase.LevelsCount, testCase.SynthesisFilters, testCase.InformationWord)
            );
        }

        [Theory]
        [MemberData(nameof(MultilevelEncodingTestCases))]
        public void MustPerformMultilevelEncoding(MultilevelEncodingTestCase testCase)
        {
            // When
            var actualCodeword = _multilevelEncoder.Encode(testCase.CodewordLength, testCase.LevelsCount, testCase.SynthesisFilters, testCase.InformationWord);

            // Then
            Assert.Equal(testCase.ExpectedCodeword, actualCodeword);
        }
    }
}