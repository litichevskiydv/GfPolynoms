namespace AppliedAlgebra.RsCodesTools.Tests
{
    using System;
    using System.Collections.Generic;
    using Decoding.ListDecoder;
    using Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using Encoding;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class GsDecoderTests
    {
        private readonly GsDecoder _decoder;

        [UsedImplicitly]
        public static readonly TheoryData<ListDecoderTestCase> DecoderTestsData;

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

        private static Tuple<FieldElement, FieldElement>[] AddNoise(Tuple<FieldElement, FieldElement>[] codeword, params int[] errorsPositions)
        {
            var one = codeword[0].Item1.Field.One();
            foreach (var errorPosition in errorsPositions)
                codeword[errorPosition].Item2.Add(one);

            return codeword;
        }

        private static ListDecoderTestCase PrepareTestsData(int n, int k, IEncoder encoder, Polynomial informationPolynomial, int randomErrorsCount) =>
            new ListDecoderTestCase
            {
                N = n,
                K = k,
                DecodedCodeword = AddRandomNoise(encoder.Encode(n, informationPolynomial), randomErrorsCount),
                MinCorrectValuesCount = n - randomErrorsCount,
                Expected = informationPolynomial
            };

        private static ListDecoderTestCase PrepareTestsData(int n, int k, IEncoder encoder, Polynomial informationPolynomial, params int[] errorsPositions) =>
            new ListDecoderTestCase
            {
                N = n,
                K = k,
                DecodedCodeword = AddNoise(encoder.Encode(n, informationPolynomial), errorsPositions),
                MinCorrectValuesCount = n - errorsPositions.Length,
                Expected = informationPolynomial
            };

        static GsDecoderTests()
        {
            var gf8 = GaloisField.Create(8, new[] {1, 1, 0, 1});
            var gf9 = GaloisField.Create(9, new[] {1, 0, 1});
            var encoder = new Encoder();

            DecoderTestsData
                = new TheoryData<ListDecoderTestCase>
                  {
                      PrepareTestsData(7, 3, encoder, new Polynomial(gf8, 1, 2, 3), 2, 3, 6),
                      PrepareTestsData(7, 3, encoder, new Polynomial(gf8, 1, 2, 3), 3),
                      PrepareTestsData(7, 3, encoder, new Polynomial(gf8, 7, 4, 1), 3),
                      PrepareTestsData(7, 3, encoder, new Polynomial(gf8, 0, 2), 3),
                      PrepareTestsData(7, 3, encoder, new Polynomial(gf8, 0, 0, 3), 3),
                      PrepareTestsData(8, 5, encoder, new Polynomial(gf9, 0, 0, 3, 1, 1), 2)
                  };
        }

        public GsDecoderTests()
        {
            _decoder = new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalculator()), new RrFactorizator());
        }

        [Theory]
        [MemberData(nameof(DecoderTestsData))]
        public void ShouldFindOriginalInformationWordAmongPossibleVariants(ListDecoderTestCase testCase)
        {
            // When
            var possibleVariants = _decoder.Decode(testCase.N, testCase.K, testCase.DecodedCodeword, testCase.MinCorrectValuesCount);

            // Then
            Assert.Contains(testCase.Expected, possibleVariants);
        }
    }
}