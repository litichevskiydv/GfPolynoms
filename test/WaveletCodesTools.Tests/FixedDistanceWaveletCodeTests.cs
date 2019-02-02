namespace AppliedAlgebra.WaveletCodesTools.Tests
{
    using System.Collections.Generic;
    using System.Linq;
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
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using JetBrains.Annotations;
    using RsCodesTools.Decoding.ListDecoder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using RsCodesTools.Decoding.StandartDecoder;
    using Xunit;

    public class FixedDistanceWaveletCodeTests
    {
        public class DecodingTestCase
        {
            public ICode Code { get; set; }

            public FieldElement[] InformationWord { get; set; }

            public FieldElement[] AdditiveNoise { get; set; }
        }

        [UsedImplicitly]
        public static readonly TheoryData<DecodingTestCase> DecodingTestsData;

        private static DecodingTestCase PrepareDecodingTestCase(ICode code, INoiseGenerator noiseGenerator, int? errorsCount = null, IEnumerable<int> informationWord = null) =>
            new DecodingTestCase
            {
                Code = code,
                InformationWord = informationWord?.Select(x => code.Field.CreateElement(x)).ToArray()
                                  ?? Enumerable.Repeat(code.Field.Zero(), code.InformationWordLength).ToArray(),
                AdditiveNoise = noiseGenerator
                    .VariatePositionsAndValues(code.Field, code.CodewordLength, errorsCount ?? (code.CodeDistance - 1) / 2)
                    .Skip(30)
                    .First()
            };

        static FixedDistanceWaveletCodeTests()
        {
            var gaussSolver = new GaussSolver();
            var noiseGenerator = new RecursiveGenerator();
            var codesFactory
                = new StandardCodesFactory(
                    new LiftingSchemeBasedBuilder(new GcdBasedBuilder(new RecursiveGcdFinder()), gaussSolver),
                    new RsBasedDecoder(new BerlekampWelchDecoder(gaussSolver), gaussSolver),
                    new GsBasedDecoder(
                        new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalcualtor()), new RrFactorizator()),
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
    }
}