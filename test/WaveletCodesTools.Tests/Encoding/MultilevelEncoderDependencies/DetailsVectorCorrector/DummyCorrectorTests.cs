namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector
{
    using System;
    using GfAlgorithms.Matrices;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector;
    using Xunit;

    public class DummyCorrectorTests
    {
        private readonly DummyCorrector _detailsVectorCorrector;

        [UsedImplicitly]
        public static TheoryData<CorrectDetailsVectorParametersValidationTestCase> CorrectDetailsVectorParametersValidationTestCases;

        static DummyCorrectorTests()
        {
            var gf3 = GaloisField.Create(3);
            CorrectDetailsVectorParametersValidationTestCases
                = new TheoryData<CorrectDetailsVectorParametersValidationTestCase>
                  {
                      new CorrectDetailsVectorParametersValidationTestCase(),
                      new CorrectDetailsVectorParametersValidationTestCase
                      {
                          DetailsVector = FieldElementsMatrix.ColumnVector(gf3, 0, 1, 2),
                          RequiredZerosNumber = -1
                      }
                  };
        }

        public DummyCorrectorTests()
        {
            _detailsVectorCorrector = new DummyCorrector();
        }

        [Theory]
        [MemberData(nameof(CorrectDetailsVectorParametersValidationTestCases))]
        public void CorrectDetailsVectorMustValidateParameters(CorrectDetailsVectorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _detailsVectorCorrector.CorrectDetailsVector(
                    testCase.IterationMatrices,
                    testCase.ApproximationVector,
                    testCase.DetailsVector,
                    testCase.CorrectableComponentsCount,
                    testCase.RequiredZerosNumber
                )
            );
        }

        [Fact]
        public void CorrectDetailsVectorMustThrowNotSupportedException()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var iterationMatrices =
            (
                FieldElementsMatrix.DoubleCirculantMatrix(gf3, 0, 0, 0, 1),
                FieldElementsMatrix.DoubleCirculantMatrix(gf3, 0, 0, 0, 1)
            );
            var approximationVector = FieldElementsMatrix.ColumnVector(gf3, 0, 2, 0, 1); 
            var detailsVector = FieldElementsMatrix.ColumnVector(gf3, 1, 0, 2, 0);
            const int correctableComponentsCount = 1;
            const int requiredZerosNumber = 1;

            // When, Then
            Assert.Throws<NotSupportedException>(
                () => _detailsVectorCorrector.CorrectDetailsVector(
                    iterationMatrices,
                    approximationVector,
                    detailsVector,
                    correctableComponentsCount,
                    requiredZerosNumber
                )
            );
        }

        [Fact]
        public void CorrectDetailsVectorMustReturnOriginalDetailsVector()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var iterationMatrices =
            (
                FieldElementsMatrix.DoubleCirculantMatrix(gf3, 0, 0, 0, 1),
                FieldElementsMatrix.DoubleCirculantMatrix(gf3, 0, 0, 0, 1)
            );
            var approximationVector = FieldElementsMatrix.ColumnVector(gf3, 0, 2, 0, 1);
            var detailsVector = FieldElementsMatrix.ColumnVector(gf3, 1, 0, 2, 0);
            const int correctableComponentsCount = 0;
            const int requiredZerosNumber = 0;

            // When
            var correctedDetailsVector = _detailsVectorCorrector.CorrectDetailsVector(
                iterationMatrices,
                approximationVector,
                detailsVector,
                correctableComponentsCount,
                requiredZerosNumber
            );

            // Then
            Assert.Equal(detailsVector, correctedDetailsVector);
        }
    }
}