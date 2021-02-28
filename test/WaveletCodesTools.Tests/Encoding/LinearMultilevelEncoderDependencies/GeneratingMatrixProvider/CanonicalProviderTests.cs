namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.LinearMultilevelEncoderDependencies.GeneratingMatrixProvider
{
    using System;
    using GfAlgorithms.Matrices;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.GeneratingMatrixProvider;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.LevelMatricesProvider;
    using Xunit;

    public class CanonicalProviderTests
    {
        public class ConstructorParametersValidationTestCase
        {
            public ILevelMatricesProvider LevelMatricesProvider { get; set; }

        }

        [UsedImplicitly]
        public static readonly TheoryData<ConstructorParametersValidationTestCase> ConstructorParametersValidationTestCases;

        static CanonicalProviderTests()
        {
            ConstructorParametersValidationTestCases
                = new TheoryData<ConstructorParametersValidationTestCase>
                  {
                      new ConstructorParametersValidationTestCase()
                  };
        }

        [Theory]
        [MemberData(nameof(ConstructorParametersValidationTestCases))]
        public void NaiveProviderConstructorMustValidateParameters(ConstructorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => new CanonicalProvider(testCase.LevelMatricesProvider));
        }

        [Fact]
        public void MustProvideGeneratingMatrix()
        {
            // Given
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

            // When
            var actualGeneratingMatrix = generatingMatrixProvider.GetGeneratingMatrix(2);

            // Then
            var expectedGeneratingMatrix = new FieldElementsMatrix(
                gf3,
                new[,]
                {
                    {1, 0, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0},
                    {0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1},
                    {0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0},
                    {0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0},
                    {0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0},
                    {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
                    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}
                }
            ).Transpose();
            Assert.Equal(expectedGeneratingMatrix, actualGeneratingMatrix);
        }
    }
}