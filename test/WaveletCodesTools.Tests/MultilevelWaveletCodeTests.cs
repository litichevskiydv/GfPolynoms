namespace AppliedAlgebra.WaveletCodesTools.Tests
{
    using CodesAbstractions;
    using CodesResearchTools.NoiseGenerator;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms.Comparers;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using TestsDependencies;
    using WaveletCodesTools.Encoding;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector;
    using WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator;
    using Xunit;

    public class MultilevelWaveletCodeTests : WaveletCodeTestsBase
    {
        [UsedImplicitly] 
        public static TheoryData<DecodingTestCase> DecodingTestsData;
        [UsedImplicitly] 
        public static TheoryData<DecodingTestCase> ListDecodingTestsData;
        [UsedImplicitly] 
        public static TheoryData<(ICode code, string stringRepresentation)> ToStringTestsData;

        static MultilevelWaveletCodeTests()
        {
            var gf3 = GaloisField.Create(3);
            var code = new MultilevelWaveletCode(
                new MultilevelEncoder(
                    new ConvolutionBasedCalculator(),
                    new CanonicalGenerator(),
                    new LinearEquationsBasedCorrector(new GaussSolver()),
                    2,
                    (
                        gf3.CreateElementsVector(2, 1, 1, 2, 0, 1, 2, 2, 0, 0, 0, 0),
                        gf3.CreateElementsVector(0, 2, 0, 2, 1, 1, 0, 0, 0, 0, 0, 0)
                    )
                ),
                gf3,
                9,
                5,
                3
            );
            var noiseGenerator = new RecursiveGenerator();

            DecodingTestsData
                = new TheoryData<DecodingTestCase>
                  {
                      PrepareDecodingTestCase(code, noiseGenerator),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {1, 0, 1, 0, 1}),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {2, 1, 1, 0, 2}),
                      PrepareDecodingTestCase(code, noiseGenerator, informationWord: new[] {1, 0, 2, 1, 2})
                  };
            ListDecodingTestsData
                = new TheoryData<DecodingTestCase>
                  {
                      PrepareListDecodingTestCase(code, noiseGenerator),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {1, 0, 1, 0, 1}),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {2, 1, 1, 0, 2}),
                      PrepareListDecodingTestCase(code, noiseGenerator, new[] {1, 0, 2, 1, 2})
                  };
            ToStringTestsData
                = new TheoryData<(ICode code, string stringRepresentation)>
                  {
                      (code, "W[9,5,3]")
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