namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.MultilevelEncoderDependencies.DetailsVectorsGenerator
{
    using System;
    using GfAlgorithms.Matrices;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorsGenerator;
    using Xunit;

    public class NaiveGeneratorTests
    {
        private readonly NaiveGenerator _detailsVectorsGenerator;

        [UsedImplicitly]
        public static readonly TheoryData<GetDetailsVectorParametersValidationTestCase> GetDetailsVectorParametersValidationTestCases;

        static NaiveGeneratorTests()
        {
            GetDetailsVectorParametersValidationTestCases
                = new TheoryData<GetDetailsVectorParametersValidationTestCase>
                  {
                      new GetDetailsVectorParametersValidationTestCase {LevelNumber = -1},
                      new GetDetailsVectorParametersValidationTestCase {LevelNumber = 1}
                  };
        }

        public NaiveGeneratorTests()
        {
            var gf3 = GaloisField.Create(3);
            _detailsVectorsGenerator =
                new NaiveGenerator(
                    gf3.CreateElement(2) * FieldElementsMatrix.CirculantMatrix(gf3.Zero(), gf3.Zero(), gf3.One())
                );
        }

        [Theory]
        [MemberData(nameof(GetDetailsVectorParametersValidationTestCases))]
        public void GetApproximationVectorMustValidateParameters(GetDetailsVectorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _detailsVectorsGenerator.GetDetailsVector(testCase.InformationWord, testCase.LevelNumber, testCase.ApproximationVector)
            );
        }

        [Fact]
        public void MustGenerateDetailsVector()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var informationWord = new[] {gf3.Zero(), gf3.One(), gf3.CreateElement(2)};
            const int levelNumber = 0;

            // When
            var actualDetailsVector = _detailsVectorsGenerator.GetDetailsVector(informationWord, levelNumber, informationWord);

            // Then
            var expectedDetailsVector = new[] {gf3.One(), gf3.Zero(), gf3.CreateElement(2)};
            Assert.Equal(expectedDetailsVector, actualDetailsVector);
        }
    }
}