namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding
{
    using System;
    using GfAlgorithms.Matrices;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using WaveletCodesTools.Encoding;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.GeneratingMatrixProvider;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.LevelMatricesProvider;
    using Xunit;

    public class LinearMultilevelEncoderTests
    {

        public class ConstructorParametersValidationTestCase
        {
            public IGeneratingMatrixProvider GeneratingMatrixProvider { get; set; }
        }

        [UsedImplicitly]
        public static TheoryData<ConstructorParametersValidationTestCase> ConstructorParametersValidationTestCases;

        static LinearMultilevelEncoderTests()
        {
            ConstructorParametersValidationTestCases
                = new TheoryData<ConstructorParametersValidationTestCase>
                  {
                      new ConstructorParametersValidationTestCase()
                  };
        }

        [Theory]
        [MemberData(nameof(ConstructorParametersValidationTestCases))]
        public void ConstructorMustValidateParameters(ConstructorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => new LinearMultilevelEncoder(testCase.GeneratingMatrixProvider));
        }

        [Fact]
        public void MustPerformEncodingForNaiveSchema()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var generatingMatrixProvider = new NaiveProvider(
                new RecursionBasedProvider(
                    new ConvolutionBasedCalculator(),
                    2,
                    (
                        gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                        gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                    )
                ),
                FieldElementsMatrix.CirculantMatrix(gf3, 0, 0, 0, 0, 0, 1),
                FieldElementsMatrix.CirculantMatrix(gf3, 0, 0, 1)
            );
            var encoder = new LinearMultilevelEncoder(generatingMatrixProvider);

            // When
            var actualCodeword = encoder.Encode(12, gf3.CreateElementsVector(1, 1, 1));

            // Then
            var expectedCodeword = gf3.CreateElementsVector(1, 1, 2, 2, 1, 1, 2, 2, 1, 1, 2, 2);
            Assert.Equal(expectedCodeword, actualCodeword);
        }
    }
}