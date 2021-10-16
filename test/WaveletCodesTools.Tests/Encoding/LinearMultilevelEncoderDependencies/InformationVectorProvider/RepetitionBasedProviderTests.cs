namespace AppliedAlgebra.WaveletCodesTools.Tests.Encoding.LinearMultilevelEncoderDependencies.InformationVectorProvider
{
    using System;
    using GfAlgorithms.Matrices;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using WaveletCodesTools.Encoding.LinearMultilevelEncoderDependencies.InformationVectorProvider;
    using Xunit;

    public class RepetitionBasedProviderTests
    {
        private readonly RepetitionBasedProvider _informationVectorProvider;

        [UsedImplicitly]
        public static TheoryData<InformationVectorGenerationParametersValidationTestCase> InformationVectorGenerationParametersValidationTestCases;

        static RepetitionBasedProviderTests()
        {
            var gf3 = GaloisField.Create(3);
            InformationVectorGenerationParametersValidationTestCases
                = new TheoryData<InformationVectorGenerationParametersValidationTestCase>
                  {
                      new InformationVectorGenerationParametersValidationTestCase(),
                      new InformationVectorGenerationParametersValidationTestCase
                      {
                          InformationWord = gf3.CreateElementsVector(0, 1, 2),
                          RequiredLength = 2
                      }
                  };
        }

        public RepetitionBasedProviderTests()
        {
            _informationVectorProvider = new RepetitionBasedProvider();
        }

        [Theory]
        [MemberData(nameof(InformationVectorGenerationParametersValidationTestCases))]
        public void GetInformationVectorShouldValidateParameters(InformationVectorGenerationParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => _informationVectorProvider.GetInformationVector(testCase.InformationWord, testCase.RequiredLength)
            );
        }

        [Fact]
        public void ShouldProvideInformationVector()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var informationWord = gf3.CreateElementsVector(0, 1, 2);
            const int requiredLength = 7;

            // When
            var actual = _informationVectorProvider.GetInformationVector(informationWord, requiredLength);

            // Then
            var expected = FieldElementsMatrix.ColumnVector(gf3.CreateElementsVector(0, 1, 2, 2, 0, 1, 1));
            Assert.Equal(expected, actual);
        }
    }
}