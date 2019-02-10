namespace AppliedAlgebra.WaveletCodesTools.Tests.Codes
{
    using CodesAbstractions;
    using CodesResearchTools.NoiseGenerator;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Comparers;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public class WaveletCodeTests : WaveletCodeTestsBase
    {
        [UsedImplicitly] public static TheoryData<DecodingTestCase> DecodingTestsData;
        [UsedImplicitly] public static TheoryData<DecodingTestCase> ListDecodingTestsData;
        [UsedImplicitly] public static TheoryData<(ICode code, string stringRepresentation)> ToStringTestsData;

        static WaveletCodeTests()
        {
            var code = new WaveletCode(7, 3, 4,
                new Polynomial(
                    new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1)),
                    0, 0, 2, 5, 6, 0, 1
                )
            );
            var noiseGenerator = new RecursiveGenerator();

            DecodingTestsData
                = new TheoryData<DecodingTestCase>
                  {
                      PrepareDecodingTestCase(code, noiseGenerator),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {1, 1, 1}),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {3, 2, 1}),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {7, 3, 2})
                  };
            ListDecodingTestsData
                = new TheoryData<DecodingTestCase>
                  {
                      PrepareListDecodingTestCase(code, noiseGenerator),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {1, 1, 1}),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {3, 2, 1}),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {7, 3, 2})
                  };
            ToStringTestsData
                = new TheoryData<(ICode code, string stringRepresentation)>
                  {
                      (code, "W[7,3,4]")
                  };
        }

        [Theory]
        [MemberData(nameof(DecodingTestsData))]
        public void ShouldTestDecoding(DecodingTestCase testCase)
        {
            // Given
            var noisyCodeword = testCase.Code.Encode(testCase.InformationWord).AddNoise(testCase.AdditiveNoise);

            // When
            var actualInformationWord = testCase.Code.Decode(noisyCodeword);

            // Then
            Assert.Equal(testCase.InformationWord, actualInformationWord);
        }

        [Theory]
        [MemberData(nameof(ListDecodingTestsData))]
        public void ShouldTestListDecoding(DecodingTestCase testCase)
        {
            // Given
            var noisyCodeword = testCase.Code.Encode(testCase.InformationWord).AddNoise(testCase.AdditiveNoise);

            // When
            var actualInformationWords = testCase.Code.DecodeViaList(noisyCodeword);

            // Then
            Assert.Contains(testCase.InformationWord, actualInformationWords, new FieldElementsArraysComparer());
        }

        [Theory]
        [MemberData(nameof(ToStringTestsData))]
        public void ShouldReturnCorrectStringRepresentation((ICode code, string stringRepresentation) testCase)
        {
            // Given
            var (code, stringRepresentation) = testCase;

            // When, Then
            Assert.Equal(stringRepresentation, code.ToString());
        }
    }
}