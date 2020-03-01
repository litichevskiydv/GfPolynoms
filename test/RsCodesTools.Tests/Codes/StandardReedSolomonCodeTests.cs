namespace AppliedAlgebra.RsCodesTools.Tests.Codes
{
    using CodesAbstractions;
    using CodesFactory;
    using CodesResearchTools.NoiseGenerator;
    using Decoding.ListDecoder;
    using Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using Decoding.StandartDecoder;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms.Comparers;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class StandardReedSolomonCodeTests : ReedSolomonCodeTestsBase
    {
        [UsedImplicitly] public static TheoryData<CodewordDecodingTestCase> DecodingTestsData;
        [UsedImplicitly] public static TheoryData<CodewordDecodingTestCase> ListDecodingTestsData;
        [UsedImplicitly] public static TheoryData<(ICode code, string stringRepresentation)> ToStringTestsData;

        static StandardReedSolomonCodeTests()
        {
            var gaussSolver = new GaussSolver();
            var noiseGenerator = new RecursiveGenerator();
            var codesFactory = new StandardCodesFactory(
                new BerlekampWelchDecoder(gaussSolver),
                new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalculator()), new RrFactorizator())
            );

            var code = codesFactory.Create(GaloisField.Create(8), 7, 4);
            DecodingTestsData
                = new TheoryData<CodewordDecodingTestCase>
                  {
                      PrepareDecodingTestCase(code, noiseGenerator),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {1, 1, 1, 1}),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {4, 3, 2, 1}),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {7, 3, 2, 5})
                  };
            ListDecodingTestsData
                = new TheoryData<CodewordDecodingTestCase>
                  {
                      PrepareListDecodingTestCase(code, noiseGenerator),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {1, 1, 1, 1}),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {4, 3, 2, 1}),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {7, 3, 2, 5})
                  };
            ToStringTestsData
                = new TheoryData<(ICode code, string stringRepresentation)>
                  {
                      (code, "RS[7,4]")
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