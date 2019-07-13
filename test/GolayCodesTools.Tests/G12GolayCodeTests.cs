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

    public class G12GolayCodeTests
    {
        [UsedImplicitly]
        public static readonly TheoryData<GolayCodeTestCase> DecodeTestsData;

        private readonly G12GolayCode _code;
        private readonly INoiseGenerator _noiseGenerator;

        static G12GolayCodeTests()
        {
            var gf3 = new PrimeOrderField(3);
            FieldElement[] InformationWordBuilder(IEnumerable<int> array) => array.Select(x => gf3.CreateElement(x)).ToArray();

            DecodeTestsData
                = new TheoryData<GolayCodeTestCase>
                  {
                      new GolayCodeTestCase {InformationWord = InformationWordBuilder(new[] {1, 2, 0, 0, 1, 2})},
                      new GolayCodeTestCase {InformationWord = InformationWordBuilder(new[] {0, 1, 2, 1, 2, 2})},
                      new GolayCodeTestCase {InformationWord = InformationWordBuilder(new[] {0, 0, 0, 2, 1, 1})},
                      new GolayCodeTestCase {InformationWord = InformationWordBuilder(new[] {0, 0, 0, 0, 0, 0})},
                      new GolayCodeTestCase {InformationWord = InformationWordBuilder(new[] {2, 2, 2, 2, 2, 2})},
                  };
        }

        public G12GolayCodeTests()
        {
            _code = new G12GolayCode();
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
                        .Skip(50)
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
                        .Skip(100)
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
            Assert.Equal("G12[12,6,6]", _code.ToString());
        }
    }
}