namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator
{
    using System;
    using System.Linq;
    using GfAlgorithms.Matrices;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator;
    using Xunit;

    public class CanonicalGeneratorTests
    {
        private readonly CanonicalGenerator _waveletCoefficientsGenerator;

        [UsedImplicitly]
        public static readonly TheoryData<GetInitialApproximationVectorParametersValidationTestCase> GetApproximationVectorParametersValidationTestCases;
        [UsedImplicitly]
        public static readonly TheoryData<GetDetailsVectorParametersValidationTestCase> GetDetailsVectorParametersValidationTestCases;
        [UsedImplicitly]
        public static readonly TheoryData<DetailsVectorGenerationTestCase> DetailsVectorGenerationTestCases;

        static CanonicalGeneratorTests()
        {
            var gf3 = GaloisField.Create(3);
            var informationWord = new[] { gf3.Zero(), gf3.One(), gf3.CreateElement(2) };
            GetApproximationVectorParametersValidationTestCases
                = new TheoryData<GetInitialApproximationVectorParametersValidationTestCase>
                  {
                      new GetInitialApproximationVectorParametersValidationTestCase(),
                      new GetInitialApproximationVectorParametersValidationTestCase
                      {
                          InformationWord = new[] {gf3.One(), gf3.One()},
                          VectorLength = 3
                      }
                  };
            GetDetailsVectorParametersValidationTestCases
                = new TheoryData<GetDetailsVectorParametersValidationTestCase>
                  {
                      new GetDetailsVectorParametersValidationTestCase(),
                      new GetDetailsVectorParametersValidationTestCase {InformationWord = informationWord},
                      new GetDetailsVectorParametersValidationTestCase
                      {
                          InformationWord = informationWord,
                          LevelNumber = 1,
                          ApproximationVector = FieldElementsMatrix.IdentityMatrix(gf3, 2)
                      }
                  };
            DetailsVectorGenerationTestCases
                = new TheoryData<DetailsVectorGenerationTestCase>
                  {
                      new DetailsVectorGenerationTestCase
                      {
                          InformationWord = informationWord,
                          LevelNumber = 1,
                          ApproximationVector = FieldElementsMatrix.ColumnVector(gf3.Zero()),
                          ExpectedDetailsVector = FieldElementsMatrix.ColumnVector(gf3.One()),
                          ExpectedCorrectableComponentsCount = 0
                      },
                      new DetailsVectorGenerationTestCase
                      {
                          InformationWord = informationWord,
                          LevelNumber = 0,
                          ApproximationVector = FieldElementsMatrix.ColumnVector(gf3.Zero(), gf3.One()),
                          ExpectedDetailsVector = FieldElementsMatrix.ColumnVector(gf3.CreateElement(2), gf3.Zero()),
                          ExpectedCorrectableComponentsCount = 1
                      }
                  };
        }

        public CanonicalGeneratorTests()
        {
            _waveletCoefficientsGenerator = new CanonicalGenerator();
        }

        [Theory]
        [MemberData(nameof(GetApproximationVectorParametersValidationTestCases))]
        public void GetApproximationVectorMustValidateParameters(GetInitialApproximationVectorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _waveletCoefficientsGenerator.GetInitialApproximationVector(testCase.InformationWord, testCase.VectorLength)
            );
        }

        [Fact]
        public void MustInitializeApproximationVector()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var informationWord = new[] { gf3.One(), gf3.One(), gf3.Zero(), gf3.CreateElement(2) };
            const int vectorLength = 3;

            // When
            var actualApproximationVector = _waveletCoefficientsGenerator.GetInitialApproximationVector(informationWord, vectorLength);

            // Then
            var expectedApproximationVector = FieldElementsMatrix.ColumnVector(informationWord.Take(3).ToArray());
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

        [Theory]
        [MemberData(nameof(DetailsVectorGenerationTestCases))]
        public void MustGenerateDetailsVector(DetailsVectorGenerationTestCase testCase)
        {
            // When
            var (actualDetailsVector, actualCorrectableComponentsCount) 
                = _waveletCoefficientsGenerator.GetDetailsVector(testCase.InformationWord, testCase.LevelNumber, testCase.ApproximationVector);

            // Then
            Assert.Equal(testCase.ExpectedDetailsVector, actualDetailsVector);
            Assert.Equal(testCase.ExpectedCorrectableComponentsCount, actualCorrectableComponentsCount);
        }
    }
}