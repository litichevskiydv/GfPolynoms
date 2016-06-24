namespace RsCodesTools.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.LinearSystemSolver;
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

        static GsDecoderTests()
        {
            var gf8 = new PrimePowerOrderField(8, 2, new[] { 1, 1, 0, 1 });

            var informationPolynomial1 = new Polynomial(gf8, 1, 2, 3);
            var informationPolynomial2 = new Polynomial(gf8, 7, 4, 1);
            var informationPolynomial3 = new Polynomial(gf8, 0, 2);
            var informationPolynomial4 = new Polynomial(gf8, 0, 0, 3);

            DecoderTestsData = new[]
                               {
                                   new object[]
                                   {
                                       7, 3, 4,
                                       AddNoise(GenerateCodeword(informationPolynomial1), 2, 3, 6),
                                       informationPolynomial1
                                   },
                                   new object[]
                                   {
                                       7, 3, 4,
                                       AddRandomNoise(GenerateCodeword(informationPolynomial1), 3),
                                       informationPolynomial1
                                   },
                                   new object[]
                                   {
                                       7, 3, 4,
                                       AddRandomNoise(GenerateCodeword(informationPolynomial2), 3),
                                       informationPolynomial2
                                   },
                                   new object[]
                                   {
                                       7, 3, 4,
                                       AddRandomNoise(GenerateCodeword(informationPolynomial3), 3),
                                       informationPolynomial3
                                   },
                                   new object[]
                                   {
                                       7, 3, 4,
                                       AddRandomNoise(GenerateCodeword(informationPolynomial4), 3),
                                       informationPolynomial4
                                   }
                               };
        }

        public GsDecoderTests()
        {
            _decoder = new GsDecoder(new SimplePolynomialBuilder(new GaussSolver()), new RrFactorizator());
        }

        [Theory]
        [MemberData(nameof(DecoderTestsData))]
        public void ShouldFindOriginalInformationWordAmongPossibleVariants(int n, int k, int minCorrectValuesCount, Tuple<FieldElement, FieldElement>[] decodedCodeword, Polynomial expectedInformationPolynomial)
        {
            // When
            var possibleVariants = _decoder.Decode(n, k, decodedCodeword, minCorrectValuesCount);

            // Then
            Assert.True(possibleVariants.Contains(expectedInformationPolynomial));
        }
    }
}