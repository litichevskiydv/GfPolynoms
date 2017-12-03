namespace AppliedAlgebra.WaveletCodesTools.Tests
{
    using System;
    using System.Collections.Generic;
    using Decoding.ListDecoderForFixedDistanceCodes;
    using Encoding;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using RsCodesTools.ListDecoder;
    using RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using Xunit;

    public class GsBasedDecoderTests
    {
        private readonly GsBasedDecoder _decoder;

        [UsedImplicitly]
        public static readonly IEnumerable<object[]> DecoderTestsData;

        private static Tuple<FieldElement, FieldElement>[] AddRandomNoise(Tuple<FieldElement, FieldElement>[] codeword, int errorsCount)
        {
            var random = new Random();
            var errorsPositions = new HashSet<int>();
            while (errorsPositions.Count < errorsCount)
                errorsPositions.Add(random.Next(codeword.Length));

            var one = codeword[0].Item1.Field.One();
            foreach (var errorPosition in errorsPositions)
                codeword[errorPosition].Item2.Add(one);

            return codeword;
        }

        private static object[] PrepareTestData(IEncoder encoder,
            int n, int k, int d, Polynomial generationPolynomial, Polynomial informationPolynomial,
            int randomErrorsCount)
        {
            return new object[]
                   {
                       n, k, d, generationPolynomial,
                       AddRandomNoise(encoder.Encode(n, generationPolynomial, informationPolynomial), randomErrorsCount),
                       n - randomErrorsCount,
                       informationPolynomial
                   };
        }

        static GsBasedDecoderTests()
        {
            var gf7 = new PrimeOrderField(7);
            var generationPolynomial1 = new Polynomial(gf7, 4, 2, 6, 4, 3, 4)
                                        + new Polynomial(gf7, 1, 2, 1, 5, 2, 1).RightShift(2);

            var gf8 = new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1));
            var generationPolynomial2 = new Polynomial(gf8, 0, 0, 2, 5, 6, 0, 1);

            var gf9 = new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1));
            var generationPolynomial3 = new Polynomial(gf9, 1, 2, 7, 2, 2, 2, 1, 5, 7);

            var gf11 = new PrimeOrderField(11);
            var generationPolynomial4 = new Polynomial(gf11, 0, 0, 7, 3, 4, 1, 8, 1, 8, 2, 7, 5);
            var generationPolynomial5 = new Polynomial(gf11, 0, 0, 2, 0, 10, 9, 3, 9, 3, 10, 2, 2);

            var gf13 = new PrimeOrderField(13);
            var generationPolynomial6 = new Polynomial(gf13, 0, 0, 0, 8, 1, 12, 2, 11, 5, 6, 4, 2, 3, 12, 2, 4);

            var gf16 = new PrimePowerOrderField(16, new Polynomial(new PrimeOrderField(2), 1, 0, 0, 1, 1));
            var generationPolynomial7 = new Polynomial(gf16, 3, 3, 13, 2, 4, 5, 2, 9, 11, 11, 14, 3, 9, 11, 10);

            var gf27 = new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1));
            var generationPolynomial8 = new Polynomial(gf27, 0, 0, 20, 18, 14, 15, 2, 5, 2, 19, 17, 4, 23, 1, 8, 6, 5, 4, 20, 26, 6, 5, 16,
                23, 26, 15, 6, 25, 18, 22, 8, 4, 17, 20, 19, 18, 8, 6, 23, 12, 20, 22, 8, 7, 0, 7, 6, 3, 11);

            var gf32 = new PrimePowerOrderField(32, new Polynomial(new PrimeOrderField(2), 1, 0, 0, 1, 0, 1));
            var generationPolynomial9 = new Polynomial(gf32, 6, 27, 1, 17, 13, 28, 18, 23, 15, 12, 3, 8, 5, 28, 1, 5, 29, 17, 18, 10, 12,
                18, 27, 22, 28, 22, 8, 2, 26, 18, 3);
            var generationPolynomial10 = new Polynomial(gf32, 7, 24, 2, 0, 5, 29, 2, 18, 3, 1, 15, 22, 1, 16, 29, 17, 6, 16, 17, 25, 21, 26,
                10, 30, 18, 6, 24, 4, 31, 14, 15);

            var encoder = new Encoder();
            DecoderTestsData = new[]
                               {
                                   PrepareTestData(encoder, 6, 3, 3, generationPolynomial1, new Polynomial(gf7, 4, 0, 2), 1),
                                   PrepareTestData(encoder, 6, 3, 3, generationPolynomial1, new Polynomial(gf7, 1, 2, 3), 1),
                                   PrepareTestData(encoder, 6, 3, 3, generationPolynomial1, new Polynomial(gf7, 6, 4, 1), 1),
                                   PrepareTestData(encoder, 6, 3, 3, generationPolynomial1, new Polynomial(gf7, 0, 2), 1),
                                   PrepareTestData(encoder, 6, 3, 3, generationPolynomial1, new Polynomial(gf7, 0, 0, 3), 1),
                                   PrepareTestData(encoder, 7, 3, 4, generationPolynomial2, new Polynomial(gf8, 0, 0, 3), 2),
                                   PrepareTestData(encoder, 7, 3, 4, generationPolynomial2, new Polynomial(gf8, 1), 2),
                                   PrepareTestData(encoder, 8, 4, 4, generationPolynomial3, new Polynomial(gf9, 0, 0, 3), 2),
                                   PrepareTestData(encoder, 8, 4, 4, generationPolynomial3, new Polynomial(gf9, 1, 2, 3, 4), 2),
                                   PrepareTestData(encoder, 8, 4, 4, generationPolynomial3, new Polynomial(gf9, 1, 2), 2),
                                   PrepareTestData(encoder, 8, 4, 4, generationPolynomial3, new Polynomial(gf9, 0, 0, 0, 6), 2),
                                   PrepareTestData(encoder, 10, 5, 6, generationPolynomial4, new Polynomial(gf11, 1, 2, 3, 4, 5), 3),
                                   PrepareTestData(encoder, 10, 5, 5, generationPolynomial5, new Polynomial(gf11, 1, 2, 3, 4, 5), 2),
                                   PrepareTestData(encoder, 12, 6, 6, generationPolynomial6, new Polynomial(gf13, 1, 2, 3, 4, 5, 6), 3),
                                   PrepareTestData(encoder, 12, 6, 6, generationPolynomial6, new Polynomial(gf13, 0, 2, 0, 2, 11), 3),
                                   PrepareTestData(encoder, 15, 7, 8, generationPolynomial7, new Polynomial(gf16, 1, 2, 14, 2, 11), 4),
                                   PrepareTestData(encoder, 15, 7, 8, generationPolynomial7, new Polynomial(gf16, 0, 0, 0, 1, 12, 12, 1), 4),
                                   PrepareTestData(encoder, 26, 13, 12, generationPolynomial8, new Polynomial(gf27, 0, 2, 0, 2, 11), 6),
                                   PrepareTestData(encoder, 31, 15, 16, generationPolynomial9, new Polynomial(gf32, 19, 2, 7, 2, 12, 23, 26), 8),
                                   PrepareTestData(encoder, 31, 15, 16, generationPolynomial9, new Polynomial(gf32, 0, 2, 7, 2, 2, 0, 26), 8),
                                   PrepareTestData(encoder, 31, 15, 16, generationPolynomial9, new Polynomial(gf32, 16, 30, 23), 8),
                                   PrepareTestData(encoder, 31, 15, 15, generationPolynomial10, new Polynomial(gf32, 19, 2, 7, 2, 12, 30, 11), 8)
                               };
        }

        public GsBasedDecoderTests()
        {
            _decoder = new GsBasedDecoder(new GsDecoder(
                    new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalcualtor()), 
                    new RrFactorizator()),
                new GaussSolver());
        }

        [Theory]
        [MemberData(nameof(DecoderTestsData))]
        public void ShouldFindOriginalInformationWordAmongPossibleVariants(int n, int k, int d, Polynomial generatingPolynomial,
            Tuple<FieldElement, FieldElement>[] decodedCodeword, int minCorrectValuesCount, Polynomial expectedInformationPolynomial)
        {
            // When
            var possibleVariants = _decoder.Decode(n, k, d, generatingPolynomial, decodedCodeword, minCorrectValuesCount);

            // Then
            Assert.Contains(expectedInformationPolynomial, possibleVariants);
        }
    }
}