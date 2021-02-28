namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding
{
    using System;
    using System.Linq;
    using GfAlgorithms.Matrices;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding;
    using WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.GeneratingMatrixProvider;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.LevelMatricesProvider;
    using Xunit;

    public class LinearMultilevelEncoderTests
    {
        public class ConstructorParametersValidationTestCase
        {
            public IGeneratingMatrixProvider GeneratingMatrixProvider { get; set; }

            public int LevelsCount { get; set; }
        }

        private readonly LinearMultilevelEncoder _naiveSchemaEncoder;
        private readonly LinearMultilevelEncoder _canonicalSchemaEncoder;

        [UsedImplicitly]
        public static TheoryData<ConstructorParametersValidationTestCase> ConstructorParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<EncodeParametersValidationTestCase> EncodeParametersValidationTestCases;

        static LinearMultilevelEncoderTests()
        {
            var gf3 = GaloisField.Create(3);
            const int waveletTransformLevelsCount = 2;
            var generatingMatrixProvider = new CanonicalProvider(
                new RecursionBasedProvider(
                    new ConvolutionBasedCalculator(),
                    waveletTransformLevelsCount,
                    (
                        gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                        gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                    )
                )
            );

            ConstructorParametersValidationTestCases
                = new TheoryData<ConstructorParametersValidationTestCase>
                  {
                      new ConstructorParametersValidationTestCase(),
                      new ConstructorParametersValidationTestCase
                      {
                          GeneratingMatrixProvider = generatingMatrixProvider,
                          LevelsCount = -1
                      },
                      new ConstructorParametersValidationTestCase
                      {
                          GeneratingMatrixProvider = generatingMatrixProvider,
                          LevelsCount = 50
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
                      new EncodeParametersValidationTestCase
                      {
                          CodewordLength = 12,
                          InformationWord = Enumerable.Repeat(gf3.Zero(), 4).ToArray()
                      }
                  };
        }

        public LinearMultilevelEncoderTests()
        {
            var gf3 = GaloisField.Create(3);
            const int waveletTransformLevelsCount = 2;
            const int encodingLevelsCount = 2;

            _naiveSchemaEncoder = new LinearMultilevelEncoder(
                new NaiveProvider(
                    new RecursionBasedProvider(
                        new ConvolutionBasedCalculator(),
                        waveletTransformLevelsCount,
                        (
                            gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                            gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                        )
                    ),
                    FieldElementsMatrix.CirculantMatrix(gf3, 0, 0, 0, 0, 0, 1),
                    FieldElementsMatrix.CirculantMatrix(gf3, 0, 0, 1)
                ),
                encodingLevelsCount
            );
            _canonicalSchemaEncoder = new LinearMultilevelEncoder(
                new CanonicalProvider(
                    new RecursionBasedProvider(
                        new ConvolutionBasedCalculator(),
                        waveletTransformLevelsCount,
                        (
                            gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                            gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                        )
                    )
                ),
                encodingLevelsCount
            );
        }

        [Theory]
        [MemberData(nameof(ConstructorParametersValidationTestCases))]
        public void ConstructorMustValidateParameters(ConstructorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => new LinearMultilevelEncoder(testCase.GeneratingMatrixProvider, testCase.LevelsCount)
            );
        }

        [Theory]
        [MemberData(nameof(EncodeParametersValidationTestCases))]
        public void EncodeMustValidateParameters(EncodeParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _naiveSchemaEncoder.Encode(testCase.CodewordLength, testCase.InformationWord)
            );
        }

        [Fact]
        public void MustPerformEncodingForNaiveSchema()
        {
            // Given
            var gf3 = GaloisField.Create(3);

            // When
            var actualCodeword = _naiveSchemaEncoder.Encode(12, gf3.CreateElementsVector(1, 1, 1));

            // Then
            var expectedCodeword = gf3.CreateElementsVector(1, 1, 2, 2, 1, 1, 2, 2, 1, 1, 2, 2);
            Assert.Equal(expectedCodeword, actualCodeword);
        }

        [Fact]
        public void MustPerformEncodingForCanonicalSchema()
        {
            // Given
            var gf3 = GaloisField.Create(3);

            // When
            var actualCodeword = _canonicalSchemaEncoder.Encode(12, gf3.CreateElementsVector(1, 1, 1, 1, 1, 1, 1));

            // Then
            var expectedCodeword = gf3.CreateElementsVector(0, 1, 0, 1, 0, 2, 0, 1, 0, 1, 1, 1);
            Assert.Equal(expectedCodeword, actualCodeword);
        }
    }
}