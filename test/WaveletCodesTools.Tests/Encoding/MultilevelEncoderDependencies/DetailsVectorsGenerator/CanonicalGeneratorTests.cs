namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.MultilevelEncoderDependencies.DetailsVectorsGenerator
{
    using System;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorsGenerator;
    using Xunit;

    public class CanonicalGeneratorTests
    {
        private readonly CanonicalGenerator _detailsVectorsGenerator;

        [UsedImplicitly]
        public static readonly TheoryData<GetDetailsVectorParametersValidationTestCase> GetDetailsVectorParametersValidationTestCases;
        [UsedImplicitly]
        public static readonly TheoryData<DetailsVectorGenerationTestCase> DetailsVectorGenerationTestCases;

        static CanonicalGeneratorTests()
        {
            var gf3 = GaloisField.Create(3);
            var informationWord = new[] {gf3.Zero(), gf3.One(), gf3.CreateElement(2)};
            GetDetailsVectorParametersValidationTestCases
                = new TheoryData<GetDetailsVectorParametersValidationTestCase>
                  {
                      new GetDetailsVectorParametersValidationTestCase(),
                      new GetDetailsVectorParametersValidationTestCase {InformationWord = informationWord},
                      new GetDetailsVectorParametersValidationTestCase
                      {
                          InformationWord = informationWord,
                          ApproximationVector = new FieldElement[0]
                      }
                  };
            DetailsVectorGenerationTestCases
                = new TheoryData<DetailsVectorGenerationTestCase>
                  {
                      new DetailsVectorGenerationTestCase
                      {
                          InformationWord = informationWord,
                          LevelNumber = 1,
                          ApproximationVector = new[] {gf3.Zero()},
                          ExpectedDetailsVector = new[] {gf3.One()}
                      },
                      new DetailsVectorGenerationTestCase
                      {
                          InformationWord = informationWord,
                          LevelNumber = 0,
                          ApproximationVector = new[] {gf3.Zero(), gf3.One()},
                          ExpectedDetailsVector = new[] {gf3.CreateElement(2), gf3.Zero()}
                      }
                  };
        }

        public CanonicalGeneratorTests()
        {
            _detailsVectorsGenerator = new CanonicalGenerator();
        }

        [Theory]
        [MemberData(nameof(GetDetailsVectorParametersValidationTestCases))]
        public void GetApproximationVectorMustValidateParameters(GetDetailsVectorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _detailsVectorsGenerator.GetDetailsVector(testCase.InformationWord, testCase.LevelNumber, testCase.ApproximationVector)
            );
        }

        [Theory]
        [MemberData(nameof(DetailsVectorGenerationTestCases))]
        public void MustGenerateDetailsVector(DetailsVectorGenerationTestCase testCase)
        {
            // When
            var actualDetailsVector = _detailsVectorsGenerator.GetDetailsVector(testCase.InformationWord, testCase.LevelNumber, testCase.ApproximationVector);

            // Then
            Assert.Equal(testCase.ExpectedDetailsVector, actualDetailsVector);
        }
    }
}