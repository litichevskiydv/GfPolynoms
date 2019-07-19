namespace AppliedAlgebra.WaveletCodesTools.Tests.Codes
{
    using CodesAbstractions;
    using CodesResearchTools.NoiseGenerator;
    using Decoding.ListDecoderForFixedDistanceCodes;
    using Decoding.StandartDecoderForFixedDistanceCodes;
    using FixedDistanceCodesFactory;
    using GeneratingPolynomialsBuilder;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.ComplementaryFilterBuilder;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.PolynomialsGcdFinder;
    using GfPolynoms.Comparers;
    using JetBrains.Annotations;
    using RsCodesTools.Decoding.ListDecoder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using RsCodesTools.Decoding.StandartDecoder;
    using TestCases;
    using Xunit;

    [UsedImplicitly]
    public class FixedDistanceWaveletCodeTests : WaveletCodeTestsBase
    {
        [UsedImplicitly] public static TheoryData<DecodingTestCase> DecodingTestsData;
        [UsedImplicitly] public static TheoryData<DecodingTestCase> ListDecodingTestsData;
        [UsedImplicitly] public static TheoryData<(ICode code, string stringRepresentation)> ToStringTestsData;

        static FixedDistanceWaveletCodeTests()
        {
            var gaussSolver = new GaussSolver();
            var noiseGenerator = new RecursiveGenerator();
            var codesFactory
                = new StandardCodesFactory(
                    new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()), gaussSolver),
                    new RsBasedDecoder(new BerlekampWelchDecoder(gaussSolver), gaussSolver),
                    new GsBasedDecoder(
                        new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalculator()), new RrFactorizator()),
                        gaussSolver
                    )
                );

            DecodingTestsData
                = new TheoryData<DecodingTestCase>
                  {
                      PrepareDecodingTestCase(codesFactory.CreateN7K3D4(), noiseGenerator),
                      PrepareDecodingTestCase(codesFactory.CreateN7K3D4(), noiseGenerator, informationWord: new[] {1, 1, 1}),
                      PrepareDecodingTestCase(codesFactory.CreateN7K3D4(), noiseGenerator, informationWord: new[] {3, 2, 1}),
                      PrepareDecodingTestCase(codesFactory.CreateN7K3D4(), noiseGenerator, informationWord: new[] {7, 3, 2}),
                      PrepareDecodingTestCase(codesFactory.CreateN8K4D4(), noiseGenerator),
                      PrepareDecodingTestCase(codesFactory.CreateN8K4D4(), noiseGenerator, informationWord: new[] {1, 1, 1, 1}),
                      PrepareDecodingTestCase(codesFactory.CreateN8K4D4(), noiseGenerator, informationWord: new[] {2, 4, 2, 7}),
                      PrepareDecodingTestCase(codesFactory.CreateN8K4D4(), noiseGenerator, informationWord: new[] {8, 4, 2, 0}),
                      PrepareDecodingTestCase(codesFactory.CreateN10K5D5(), noiseGenerator),
                      PrepareDecodingTestCase(codesFactory.CreateN10K5D5(), noiseGenerator, informationWord: new[] {1, 1, 1, 1, 1}),
                      PrepareDecodingTestCase(codesFactory.CreateN10K5D5(), noiseGenerator, informationWord: new[] {2, 4, 2, 5, 10}),
                      PrepareDecodingTestCase(codesFactory.CreateN10K5D5(), noiseGenerator, informationWord: new[] {0, 0, 3, 7, 8}),
                      PrepareDecodingTestCase(codesFactory.CreateN15K7D8(), noiseGenerator),
                      PrepareDecodingTestCase(codesFactory.CreateN15K7D8(), noiseGenerator, informationWord: new[] {1, 1, 1, 1, 1, 1, 1}),
                      PrepareDecodingTestCase(codesFactory.CreateN15K7D8(), noiseGenerator, informationWord: new[] {1, 2, 10, 8, 5, 11, 1}),
                      PrepareDecodingTestCase(codesFactory.CreateN15K7D8(), noiseGenerator, informationWord: new[] {9, 12, 1, 3, 2, 7, 4}),
                      PrepareDecodingTestCase(codesFactory.CreateN26K13D12(), noiseGenerator),
                      PrepareDecodingTestCase(codesFactory.CreateN26K13D12(), noiseGenerator, informationWord: new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}),
                      PrepareDecodingTestCase(codesFactory.CreateN26K13D12(), noiseGenerator, informationWord: new[] {1, 3, 23, 3, 8, 9, 11, 0, 13, 24, 21, 1, 16}),
                      PrepareDecodingTestCase(codesFactory.CreateN26K13D12(), noiseGenerator, informationWord: new[] {0, 9, 7, 4, 6, 22, 12, 17, 8, 13, 12, 25, 2}),
                      PrepareDecodingTestCase(codesFactory.CreateN30K15D13(), noiseGenerator),
                      PrepareDecodingTestCase(codesFactory.CreateN31K15D15(), noiseGenerator),
                      PrepareDecodingTestCase(codesFactory.CreateN80K40D37(), noiseGenerator),
                      PrepareDecodingTestCase(codesFactory.CreateN100K50D49(), noiseGenerator)
                  };
            ListDecodingTestsData
                = new TheoryData<DecodingTestCase>
                  {
                      PrepareListDecodingTestCase(codesFactory.CreateN7K3D4(), noiseGenerator),
                      PrepareListDecodingTestCase(codesFactory.CreateN7K3D4(), noiseGenerator, new[] {1, 1, 1}),
                      PrepareListDecodingTestCase(codesFactory.CreateN7K3D4(), noiseGenerator, new[] {3, 2, 1}),
                      PrepareListDecodingTestCase(codesFactory.CreateN7K3D4(), noiseGenerator, new[] {7, 3, 2}),
                      PrepareListDecodingTestCase(codesFactory.CreateN15K7D8(), noiseGenerator),
                      PrepareListDecodingTestCase(codesFactory.CreateN15K7D8(), noiseGenerator, new[] {1, 1, 1, 1, 1, 1, 1}),
                      PrepareListDecodingTestCase(codesFactory.CreateN15K7D8(), noiseGenerator, new[] {1, 2, 10, 8, 5, 11, 1}),
                      PrepareListDecodingTestCase(codesFactory.CreateN15K7D8(), noiseGenerator, new[] {9, 12, 1, 3, 2, 7, 4}),
                      PrepareListDecodingTestCase(codesFactory.CreateN26K13D12(), noiseGenerator),
                      PrepareListDecodingTestCase(codesFactory.CreateN26K13D12(), noiseGenerator, new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}),
                      PrepareListDecodingTestCase(codesFactory.CreateN26K13D12(), noiseGenerator, new[] {1, 3, 23, 3, 8, 9, 11, 0, 13, 24, 21, 1, 16}),
                      PrepareListDecodingTestCase(codesFactory.CreateN26K13D12(), noiseGenerator, new[] {0, 9, 7, 4, 6, 22, 12, 17, 8, 13, 12, 25, 2}),
                      PrepareListDecodingTestCase(codesFactory.CreateN31K15D15(), noiseGenerator)
                  };
            ToStringTestsData
                = new TheoryData<(ICode code, string stringRepresentation)>
                  {
                      (codesFactory.CreateN7K3D4(), "W[7,3,4]")
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