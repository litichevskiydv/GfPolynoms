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

    public class G11GolayCodeTests
    {
        [UsedImplicitly] public static readonly IEnumerable<object[]> DecodeTestsData;

        private readonly G11GolayCode _code;
        private readonly INoiseGenerator _noiseGenerator;

        static G11GolayCodeTests()
        {
            var gf3 = new PrimeOrderField(3);
            FieldElement[] InformationWordBuilder(IEnumerable<int> array) => array.Select(x => gf3.CreateElement(x)).ToArray();

            DecodeTestsData
                = new[]
                  {
                      new object[] {InformationWordBuilder(new[] {1, 2, 0, 0, 1, 2})},
                      new object[] {InformationWordBuilder(new[] {0, 1, 2, 1, 2, 2})},
                      new object[] {InformationWordBuilder(new[] {0, 0, 0, 2, 1, 1})},
                      new object[] {InformationWordBuilder(new[] {0, 0, 0, 0, 0, 0})},
                      new object[] {InformationWordBuilder(new[] {2, 2, 2, 2, 2, 2})},
                  };
        }

        public G11GolayCodeTests()
        {
            _code = new G11GolayCode();
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
                        .Skip(50)
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
                        .Skip(100)
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
            Assert.Equal("G11[11,6,5]", _code.ToString());
        }
    }
}