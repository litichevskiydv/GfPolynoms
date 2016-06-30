namespace RsCodesTools.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.CombinationsCountCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using ListDecoder;
    using ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using Xunit;

    public class GsDecoderTests
    {
        private readonly GsDecoder _decoder;

        [UsedImplicitly]
        public static readonly IEnumerable<object[]> DecoderTestsData;

        private static Tuple<FieldElement, FieldElement>[] GenerateCodeword(Polynomial informationPolynomial)
        {
            var field = informationPolynomial.Field;
            var codeword = new Tuple<FieldElement, FieldElement>[field.Order - 1];
            for (var i = 1; i < field.Order; i++)
                codeword[i - 1] = new Tuple<FieldElement, FieldElement>(new FieldElement(field, i),
                    new FieldElement(field, informationPolynomial.Evaluate(i)));

            return codeword;
        }

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

        private static object[] PrepareTestsData(int n, int k, Polynomial informationPolynomial, int randomErrorsCount)
        {
            return new object[]
                   {
                       n, k,
                       AddRandomNoise(GenerateCodeword(informationPolynomial), randomErrorsCount), n - randomErrorsCount,
                       informationPolynomial
                   };
        }

        private static object[] PrepareTestsData(int n, int k, Polynomial informationPolynomial, params int[] errorsPositions)
        {
            return new object[]
                   {
                       n, k,
                       AddNoise(GenerateCodeword(informationPolynomial), errorsPositions), n - errorsPositions.Length,
                       informationPolynomial
                   };
        }

        static GsDecoderTests()
        {
            var gf8 = new PrimePowerOrderField(8, 2, new[] { 1, 1, 0, 1 });
            var gf9 = new PrimePowerOrderField(9, 3, new[] { 1, 0, 1 });

            DecoderTestsData = new[]
                               {
                                   PrepareTestsData(7, 3, new Polynomial(gf8, 1, 2, 3), 2, 3, 6),
                                   PrepareTestsData(7, 3, new Polynomial(gf8, 1, 2, 3), 3),
                                   PrepareTestsData(7, 3, new Polynomial(gf8, 7, 4, 1), 3),
                                   PrepareTestsData(7, 3, new Polynomial(gf8, 0, 2), 3),
                                   PrepareTestsData(7, 3, new Polynomial(gf8, 0, 0, 3), 3),
                                   PrepareTestsData(8, 5, new Polynomial(gf9, 0, 0, 3, 1, 1), 2)
                               };
        }

        public GsDecoderTests()
        {
            _decoder = new GsDecoder(new KotterAlgorithmBasedBuilder(new PascalsTriangleBasedCalcualtor()), new RrFactorizator());
        }

        [Theory]
        [MemberData(nameof(DecoderTestsData))]
        public void ShouldFindOriginalInformationWordAmongPossibleVariants(int n, int k, Tuple<FieldElement, FieldElement>[] decodedCodeword, int minCorrectValuesCount, Polynomial expectedInformationPolynomial)
        {
            // When
            var possibleVariants = _decoder.Decode(n, k, decodedCodeword, minCorrectValuesCount);

            // Then
            Assert.True(possibleVariants.Contains(expectedInformationPolynomial));
        }
    }
}