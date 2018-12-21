namespace AppliedAlgebra.GolayCodesTools.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using CodesResearchTools.NoiseGenerator;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public class G24GolayCodeTests
    {
        [UsedImplicitly] public static readonly IEnumerable<object[]> DecodeTestsData;

        private readonly G24GolayCode _code;
        private readonly INoiseGenerator _noiseGenerator;

        static G24GolayCodeTests()
        {
            var gf2 = new PrimeOrderField(2);
            FieldElement[] InformationWordBuilder(IEnumerable<int> array) => array.Select(x => gf2.CreateElement(x)).ToArray();

            DecodeTestsData
                = new[]
                  {
                      new object[] {InformationWordBuilder(new[] {1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0})},
                      new object[] {InformationWordBuilder(new[] {1, 0, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1})},
                      new object[] {InformationWordBuilder(new[] {1, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 0})},
                      new object[] {InformationWordBuilder(new[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0})},
                      new object[] {InformationWordBuilder(new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1})}
                  };
        }

        public G24GolayCodeTests()
        {
            _code = new G24GolayCode();
            _noiseGenerator = new RecursiveGenerator();
        }

        [Theory]
        [MemberData(nameof(DecodeTestsData))]
        public void ShouldTestDecodeMethod(FieldElement[] expectedInformationWord)
        {
            // Given
            var codeword = _code.Encode(expectedInformationWord);
            var noisyCodeword =
                codeword.AddNoise(
                    _noiseGenerator.VariatePositionsAndValues(_code.Field, _code.CodewordLength, (_code.CodeDistance - 1) / 2)
                        .Skip(100)
                        .First()
                );

            // When
            var actualInformationWord = _code.Decode(noisyCodeword);

            // Then
            Assert.Equal(expectedInformationWord, actualInformationWord);
        }

        [Theory]
        [MemberData(nameof(DecodeTestsData))]
        public void ShouldTestDecodeViaListMethod(FieldElement[] expectedInformationWord)
        {
            // Given
            var codeword = _code.Encode(expectedInformationWord);
            var noisyCodeword =
                codeword.AddNoise(
                    _noiseGenerator.VariatePositionsAndValues(_code.Field, _code.CodewordLength, (_code.CodeDistance - 1) / 2 + 1)
                        .Skip(200)
                        .First()
                );

            // When
            var actualInformationWords = _code.DecodeViaList(noisyCodeword);

            // Then
            Assert.Contains(expectedInformationWord, actualInformationWords);
        }

        [Fact]
        public void ShouldReturnCorrectStringRepresentation()
        {
            Assert.Equal("G24[24,12,8]", _code.ToString());
        }
    }
}