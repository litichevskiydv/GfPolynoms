namespace AppliedAlgebra.RsCodesTools.Tests.Codes
{
    using CodesAbstractions;
    using CodesResearchTools.NoiseGenerator;
    using GfAlgorithms.Extensions;
    using GfPolynoms.Comparers;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class ReedSolomonCodeTests : ReedSolomonCodeTestsBase
    {
        [UsedImplicitly] public static TheoryData<CodewordDecodingTestCase> DecodingTestsData;
        [UsedImplicitly] public static TheoryData<CodewordDecodingTestCase> ListDecodingTestsData;
        [UsedImplicitly] public static TheoryData<(ICode code, string stringRepresentation)> ToStringTestsData;

        static ReedSolomonCodeTests()
        {
            var code = new ReedSolomonCode(new PrimePowerOrderField(9), 8, 4);
            var noiseGenerator = new RecursiveGenerator();

            DecodingTestsData
                = new TheoryData<CodewordDecodingTestCase>
                  {
                      PrepareDecodingTestCase(code, noiseGenerator),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {1, 1, 1, 1}),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {4, 3, 2, 1}),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {7, 3, 8, 2})
                  };
            ListDecodingTestsData
                = new TheoryData<CodewordDecodingTestCase>
                  {
                      PrepareListDecodingTestCase(code, noiseGenerator),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {1, 1, 1, 1}),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {4, 3, 2, 1}),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {7, 3, 8, 2})
                  };
            ToStringTestsData
                = new TheoryData<(ICode code, string stringRepresentation)>
                  {
                      (code, "RS[8,4]")
                  };
        }

        [Theory]
        [MemberData(nameof(DecodingTestsData))]
        public void ShouldTestDecoding(CodewordDecodingTestCase testCase)
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
        public void ShouldTestListDecoding(CodewordDecodingTestCase testCase)
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