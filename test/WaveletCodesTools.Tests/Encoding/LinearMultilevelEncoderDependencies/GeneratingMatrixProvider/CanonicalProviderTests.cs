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

            public int LevelsCount { get; set; }
        }

        [UsedImplicitly]
        public static readonly TheoryData<ConstructorParametersValidationTestCase> ConstructorParametersValidationTestCases;

        static CanonicalProviderTests()
        {
            var gf3 = GaloisField.Create(3);
            var levelMatricesProvider = new RecursionBasedProvider(
                new ConvolutionBasedCalculator(),
                2,
                (
                    gf3.CreateElementsVector(2, 1, 1, 2, 0, 1, 2, 2, 0, 0, 0, 0),
                    gf3.CreateElementsVector(0, 2, 0, 2, 1, 1, 0, 0, 0, 0, 0, 0)
                )
            );

            ConstructorParametersValidationTestCases
                = new TheoryData<ConstructorParametersValidationTestCase>
                  {
                      new ConstructorParametersValidationTestCase(),
                      new ConstructorParametersValidationTestCase {LevelMatricesProvider = levelMatricesProvider}
                  };
        }

        [Theory]
        [MemberData(nameof(ConstructorParametersValidationTestCases))]
        public void NaiveProviderConstructorMustValidateParameters(ConstructorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => new CanonicalProvider(testCase.LevelMatricesProvider, testCase.LevelsCount));
        }

        [Fact]
        public void MustProvideGeneratingMatrix()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            const int levelsCount = 2;
            var generatingMatrixProvider = new CanonicalProvider(
                new RecursionBasedProvider(
                    new ConvolutionBasedCalculator(),
                    levelsCount,
                    (
                        gf3.CreateElementsVector(1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0),
                        gf3.CreateElementsVector(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                    )
                ),
                levelsCount
            );

            // When
            var actualGeneratingMatrix = generatingMatrixProvider.GetGeneratingMatrix();

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