namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector
{
    using System;
    using System.Linq;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.Matrices;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector;
    using Xunit;

    public class LinearEquationsBasedCorrectorTests
    {
        private readonly LinearEquationsBasedCorrector _detailsVectorCorrector;

        [UsedImplicitly]
        public static TheoryData<CorrectDetailsVectorParametersValidationTestCase> CorrectDetailsVectorParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<DetailsVectorCorrectionTestCase> DetailsVectorCorrectionTestCases;

        static LinearEquationsBasedCorrectorTests()
        {
            var gf3 = GaloisField.Create(3);
            CorrectDetailsVectorParametersValidationTestCases
                = new TheoryData<CorrectDetailsVectorParametersValidationTestCase>
                  {
                      new CorrectDetailsVectorParametersValidationTestCase(),
                      new CorrectDetailsVectorParametersValidationTestCase
                      {
                          ApproximationVector = FieldElementsMatrix.IdentityMatrix(gf3, 4)
                      },
                      new CorrectDetailsVectorParametersValidationTestCase
                      {
                          ApproximationVector = FieldElementsMatrix.ColumnVector(gf3, 0, 2, 1, 1)
                      },
                      new CorrectDetailsVectorParametersValidationTestCase
                      {
                          ApproximationVector = FieldElementsMatrix.ColumnVector(gf3, 0, 2, 1, 1),
                          DetailsVector = FieldElementsMatrix.IdentityMatrix(gf3, 4)
                      },
                      new CorrectDetailsVectorParametersValidationTestCase
                      {
                          ApproximationVector = FieldElementsMatrix.ColumnVector(gf3, 0, 2, 1, 1),
                          DetailsVector = FieldElementsMatrix.ColumnVector(gf3, 0, 2, 1)
                      },
                      new CorrectDetailsVectorParametersValidationTestCase
                      {
                          ApproximationVector = FieldElementsMatrix.ColumnVector(gf3, 0, 2, 1, 1),
                          DetailsVector = FieldElementsMatrix.ColumnVector(gf3, 2, 1, 1, 2),
                          CorrectableComponentsCount = -1
                      },
                      new CorrectDetailsVectorParametersValidationTestCase
                      {
                          ApproximationVector = FieldElementsMatrix.ColumnVector(gf3, 0, 2, 1, 1),
                          DetailsVector = FieldElementsMatrix.ColumnVector(gf3, 2, 1, 1, 2),
                          CorrectableComponentsCount = 5
                      },
                      new CorrectDetailsVectorParametersValidationTestCase
                      {
                          ApproximationVector = FieldElementsMatrix.ColumnVector(gf3, 0, 2, 1, 1),
                          DetailsVector = FieldElementsMatrix.ColumnVector(gf3, 2, 1, 0, 0),
                          CorrectableComponentsCount = 2,
                          RequiredZerosNumber = -1
                      },
                      new CorrectDetailsVectorParametersValidationTestCase
                      {
                          ApproximationVector = FieldElementsMatrix.ColumnVector(gf3, 0, 2, 1, 1),
                          DetailsVector = FieldElementsMatrix.ColumnVector(gf3, 2, 1, 0, 0),
                          CorrectableComponentsCount = 2,
                          RequiredZerosNumber = 3
                      }
                  };

            var iterationMatrices =
            (
                FieldElementsMatrix.DoubleCirculantMatrix(gf3, 2, 1, 2, 1, 1, 0, 2, 1, 2, 0, 2, 1).Transpose(),
                FieldElementsMatrix.DoubleCirculantMatrix(gf3, 1, 1, 0, 1, 2, 1, 2, 0, 2, 1, 0, 0).Transpose()
            );
            var approximationVector = FieldElementsMatrix.ColumnVector(gf3, 1, 0, 0, 0, 0, 0);
            var detailsVector = FieldElementsMatrix.ColumnVector(gf3, 1, 1, 2, 1, 2, 2);
            DetailsVectorCorrectionTestCases
                = new TheoryData<DetailsVectorCorrectionTestCase>
                  {
                      new DetailsVectorCorrectionTestCase
                      {
                          IterationMatrices = iterationMatrices,
                          ApproximationVector = approximationVector,
                          DetailsVector = detailsVector,
                          CorrectableComponentsCount = 0,
                          RequiredZerosNumber = 0
                      },
                      new DetailsVectorCorrectionTestCase
                      {
                          IterationMatrices = iterationMatrices,
                          ApproximationVector = approximationVector,
                          DetailsVector = detailsVector,
                          CorrectableComponentsCount = 1,
                          RequiredZerosNumber = 1
                      },
                      new DetailsVectorCorrectionTestCase
                      {
                          IterationMatrices = iterationMatrices,
                          ApproximationVector = approximationVector,
                          DetailsVector = detailsVector,
                          CorrectableComponentsCount = 2,
                          RequiredZerosNumber = 2
                      },
                      new DetailsVectorCorrectionTestCase
                      {
                          IterationMatrices = iterationMatrices,
                          ApproximationVector = approximationVector,
                          DetailsVector = detailsVector,
                          CorrectableComponentsCount = 3,
                          RequiredZerosNumber = 3
                      },
                      new DetailsVectorCorrectionTestCase
                      {
                          IterationMatrices = iterationMatrices,
                          ApproximationVector = approximationVector,
                          DetailsVector = detailsVector,
                          CorrectableComponentsCount = 4,
                          RequiredZerosNumber = 4
                      },
                      new DetailsVectorCorrectionTestCase
                      {
                          IterationMatrices = iterationMatrices,
                          ApproximationVector = approximationVector,
                          DetailsVector = detailsVector,
                          CorrectableComponentsCount = 5,
                          RequiredZerosNumber = 5
                      },
                      new DetailsVectorCorrectionTestCase
                      {
                          IterationMatrices = iterationMatrices,
                          ApproximationVector = approximationVector,
                          DetailsVector = detailsVector,
                          CorrectableComponentsCount = 6,
                          RequiredZerosNumber = 6
                      }
                  };
        }

        public LinearEquationsBasedCorrectorTests()
        {
            _detailsVectorCorrector = new LinearEquationsBasedCorrector(new GaussSolver());
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

        [Theory]
        [MemberData(nameof(DetailsVectorCorrectionTestCases))]
        public void MustCorrectDetailsVector(DetailsVectorCorrectionTestCase testCase)
        {
            // When
            var correctedDetailsVector = _detailsVectorCorrector.CorrectDetailsVector(
                testCase.IterationMatrices,
                testCase.ApproximationVector,
                testCase.DetailsVector,
                testCase.CorrectableComponentsCount,
                testCase.RequiredZerosNumber
            );

            // Then
            var (hMatrix, gMatrix) = testCase.IterationMatrices;
            var signal = (hMatrix * testCase.ApproximationVector + gMatrix * correctedDetailsVector).GetColumn(0);
            Assert.True(signal.TakeLast(testCase.RequiredZerosNumber).All(x => x.Representation == 0));

        }
    }
}