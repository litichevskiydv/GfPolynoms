namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.MultilevelEncoderDependencies.GeneratingMatrixProvider
{
    using System;
    using GfAlgorithms.Matrices;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.GeneratingMatrixProvider;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.LevelMatricesProvider;
    using Xunit;

    public class NaiveProviderTests
    {
        public class ConstructorParametersValidationTestCase
        {
            public ILevelMatricesProvider LevelMatricesProvider { get; set; }  
            
            public FieldElementsMatrix[] LevelsTransforms { get; set; }
        }

        [UsedImplicitly]
        public static readonly TheoryData<ConstructorParametersValidationTestCase> ConstructorParametersValidationTestCases;

        static NaiveProviderTests()
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
                      new ConstructorParametersValidationTestCase
                      {
                          LevelMatricesProvider = levelMatricesProvider
                      },
                      new ConstructorParametersValidationTestCase
                      {
                          LevelMatricesProvider = levelMatricesProvider,
                          LevelsTransforms = Array.Empty<FieldElementsMatrix>()
                      }
                  };
        }

        [Theory]
        [MemberData(nameof(ConstructorParametersValidationTestCases))]
        public void NaiveProviderConstructorMustValidateParameters(ConstructorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => new NaiveProvider(testCase.LevelMatricesProvider, testCase.LevelsTransforms));
        }

        [Fact]
        public void MustProvideGeneratingMatrix()
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

            // When
            var actualGeneratingMatrix = generatingMatrixProvider.GetGeneratingMatrix();

            // Then
            var expectedGeneratingMatrix = new FieldElementsMatrix(
                gf3,
                new[,]
                {
                    {1, 1, 1, 1, 0, 1, 1, 1, 0, 2, 0, 0},
                    {0, 2, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1},
                    {0, 1, 1, 1, 0, 2, 0, 0, 1, 1, 1, 1}
                }
            ).Transpose();
            Assert.Equal(expectedGeneratingMatrix, actualGeneratingMatrix);
        }
    }
}