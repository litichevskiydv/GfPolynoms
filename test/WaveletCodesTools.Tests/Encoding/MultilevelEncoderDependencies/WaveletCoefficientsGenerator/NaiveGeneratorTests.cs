namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator
{
    using System;
    using GfAlgorithms.Matrices;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator;
    using Xunit;

    public class NaiveGeneratorTests
    {
        private readonly NaiveGenerator _waveletCoefficientsGenerator;

        [UsedImplicitly]
        public static readonly TheoryData<GetApproximationVectorParametersValidationTestCase> GetApproximationVectorParametersValidationTestCases;
        [UsedImplicitly]
        public static readonly TheoryData<GetDetailsVectorParametersValidationTestCase> GetDetailsVectorParametersValidationTestCases;

        static NaiveGeneratorTests()
        {
            var gf3 = GaloisField.Create(3);

            GetApproximationVectorParametersValidationTestCases
                = new TheoryData<GetApproximationVectorParametersValidationTestCase>
                  {
                      new GetApproximationVectorParametersValidationTestCase(),
                      new GetApproximationVectorParametersValidationTestCase
                      {
                          InformationWord = new[] {gf3.One(), gf3.One()},
                          SignalLength = 5,
                          LevelNumber = 0
                      }
                  };
            GetDetailsVectorParametersValidationTestCases
                = new TheoryData<GetDetailsVectorParametersValidationTestCase>
                  {
                      new GetDetailsVectorParametersValidationTestCase {LevelNumber = -1},
                      new GetDetailsVectorParametersValidationTestCase {LevelNumber = 1},
                      new GetDetailsVectorParametersValidationTestCase {LevelNumber = 0},
                      new GetDetailsVectorParametersValidationTestCase
                      {
                          LevelNumber = 0,
                          ApproximationVector = FieldElementsMatrix.IdentityMatrix(gf3, 2)
                      }
                  };
        }

        public NaiveGeneratorTests()
        {
            var gf3 = GaloisField.Create(3);
            _waveletCoefficientsGenerator = new NaiveGenerator(
                gf3.CreateElement(2) * FieldElementsMatrix.CirculantMatrix(gf3.Zero(), gf3.Zero(), gf3.One())
            );
        }

        [Theory]
        [MemberData(nameof(GetApproximationVectorParametersValidationTestCases))]
        public void GetApproximationVectorMustValidateParameters(GetApproximationVectorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _waveletCoefficientsGenerator.GetApproximationVector(testCase.InformationWord, testCase.SignalLength, testCase.LevelNumber)
            );
        }

        [Fact]
        public void MustInitializeApproximationVector()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var informationWord = new[] { gf3.One(), gf3.Zero(), gf3.CreateElement(2) };
            const int signalLength = 24;
            const int levelNumber = 2;

            // When
            var actualApproximationVector = _waveletCoefficientsGenerator.GetApproximationVector(informationWord, signalLength, levelNumber);

            // Then
            var expectedApproximationVector = FieldElementsMatrix.ColumnVector(informationWord);
            Assert.Equal(expectedApproximationVector, actualApproximationVector);
        }

        [Theory]
        [MemberData(nameof(GetDetailsVectorParametersValidationTestCases))]
        public void GetDetailsVectorMustValidateParameters(GetDetailsVectorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _waveletCoefficientsGenerator.GetDetailsVector(testCase.InformationWord, testCase.LevelNumber, testCase.ApproximationVector)
            );
        }

        [Fact]
        public void MustGenerateDetailsVector()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var informationWord = new[] { gf3.Zero(), gf3.One(), gf3.CreateElement(2) };
            const int levelNumber = 0;
            var approximationVector = FieldElementsMatrix.ColumnVector(informationWord);

            // When
            var actualDetailsVector = _waveletCoefficientsGenerator.GetDetailsVector(informationWord, levelNumber, approximationVector);

            // Then
            var expectedDetailsVector = FieldElementsMatrix.ColumnVector(gf3.One(), gf3.Zero(), gf3.CreateElement(2));
            Assert.Equal(expectedDetailsVector, actualDetailsVector);
        }
    }
}