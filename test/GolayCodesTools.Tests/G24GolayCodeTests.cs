namespace AppliedAlgebra.GolayCodesTools.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using CodesResearchTools.NoiseGenerator;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Comparers;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class G24GolayCodeTests
    {
        [UsedImplicitly]
        public static readonly TheoryData<GolayCodeTestCase> DecodeTestsData;

        private readonly G24GolayCode _code;
        private readonly INoiseGenerator _noiseGenerator;

        static G24GolayCodeTests()
        {
            var gf2 = GaloisField.Create(2);
            FieldElement[] InformationWordBuilder(IEnumerable<int> array) => array.Select(x => gf2.CreateElement(x)).ToArray();

            DecodeTestsData
                = new TheoryData<GolayCodeTestCase>
                  {
                      new GolayCodeTestCase {InformationWord = InformationWordBuilder(new[] {1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0})},
                      new GolayCodeTestCase {InformationWord = InformationWordBuilder(new[] {1, 0, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1})},
                      new GolayCodeTestCase {InformationWord = InformationWordBuilder(new[] {1, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 0})},
                      new GolayCodeTestCase {InformationWord = InformationWordBuilder(new[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0})},
                      new GolayCodeTestCase {InformationWord = InformationWordBuilder(new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1})}
                  };
        }

        public G24GolayCodeTests()
        {
            _code = new G24GolayCode();
            _noiseGenerator = new RecursiveGenerator();
        }

        [Theory]
        [MemberData(nameof(DecodeTestsData))]
        public void ShouldTestDecodeMethod(GolayCodeTestCase testCase)
        {
            // Given
            var codeword = _code.Encode(testCase.InformationWord);
            var noisyCodeword =
                codeword.AddNoise(
                    _noiseGenerator.VariatePositionsAndValues(_code.Field, _code.CodewordLength, (_code.CodeDistance - 1) / 2)
                        .Skip(100)
                        .First()
                );

            // When
            var actualInformationWord = _code.Decode(noisyCodeword);

            // Then
            Assert.Equal(testCase.InformationWord, actualInformationWord);
        }

        [Theory]
        [MemberData(nameof(DecodeTestsData))]
        public void ShouldTestDecodeViaListMethod(GolayCodeTestCase testCase)
        {
            // Given
            var codeword = _code.Encode(testCase.InformationWord);
            var noisyCodeword =
                codeword.AddNoise(
                    _noiseGenerator.VariatePositionsAndValues(_code.Field, _code.CodewordLength, (_code.CodeDistance - 1) / 2 + 1)
                        .Skip(200)
                        .First()
                );

            // When
            var actualInformationWords = _code.DecodeViaList(noisyCodeword);

            // Then
            Assert.Contains(testCase.InformationWord, actualInformationWords, new FieldElementsArraysComparer());
        }

        [Fact]
        public void ShouldReturnCorrectStringRepresentation()
        {
            Assert.Equal("G24[24,12,8]", _code.ToString());
        }
    }
}