namespace WaveletCodesTools.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using ListDecoderForFixedDistanceCodes;
    using RsCodesTools.ListDecoder;
    using RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder;
    using RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator;
    using Xunit;

    public class GsBasedDecoderTests
    {
        private readonly GsBasedDecoder _decoder;

        [UsedImplicitly]
        public static readonly IEnumerable<object[]> DecoderTestsData;

        private static Tuple<FieldElement, FieldElement>[] GenerateCodeword(int n, FieldElement generationElement, Polynomial generationPolynomial, Polynomial informationPolynomial)
        {
            var field = informationPolynomial.Field;

            var m = new Polynomial(field, 1).RightShift(n);
            m[0] = field.InverseForAddition(1);
            var c = (informationPolynomial.RaiseVariableDegree(2) * generationPolynomial) % m;

            var i = 0;
            var generationElementPower = field.One();
            var codeword = new Tuple<FieldElement, FieldElement>[n];
            for (; i <= c.Degree; i++, generationElementPower *= generationElement)
                codeword[i] = new Tuple<FieldElement, FieldElement>(generationElementPower, new FieldElement(field, c[i]));
            for (; i < n; i++, generationElementPower *= generationElement)
                codeword[i] = new Tuple<FieldElement, FieldElement>(generationElementPower, field.Zero());

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

        static GsBasedDecoderTests()
        {
            var gf7 = new PrimeOrderField(7);
            var generationElement = new FieldElement(gf7, 3);
            var generationPolynomial = new Polynomial(gf7, 4, 2, 6, 4, 3, 4)
                                        + new Polynomial(gf7, 1, 2, 1, 5, 2, 1).RightShift(2);

            var informationPolynomial1 = new Polynomial(gf7, 4, 0, 2);
            var informationPolynomial2 = new Polynomial(gf7, 1, 2, 3);
            var informationPolynomial3 = new Polynomial(gf7, 6, 4, 1);
            var informationPolynomial4 = new Polynomial(gf7, 0, 2);
            var informationPolynomial5 = new Polynomial(gf7, 0, 0, 3);

            DecoderTestsData = new[]
                               {
                                   new object[]
                                   {
                                       6, 3, 3, generationPolynomial,
                                       AddRandomNoise(GenerateCodeword(6, generationElement, generationPolynomial, informationPolynomial1), 1), 5,
                                       informationPolynomial1
                                   },
                                   new object[]
                                   {
                                       6, 3, 3, generationPolynomial,
                                       AddRandomNoise(GenerateCodeword(6, generationElement, generationPolynomial, informationPolynomial2), 1), 5,
                                       informationPolynomial2
                                   },
                                   new object[]
                                   {
                                       6, 3, 3, generationPolynomial,
                                       AddRandomNoise(GenerateCodeword(6, generationElement, generationPolynomial, informationPolynomial3), 1), 5,
                                       informationPolynomial3
                                   },
                                   new object[]
                                   {
                                       6, 3, 3, generationPolynomial,
                                       AddRandomNoise(GenerateCodeword(6, generationElement, generationPolynomial, informationPolynomial4), 1), 5,
                                       informationPolynomial4
                                   },
                                   new object[]
                                   {
                                       6, 3, 3, generationPolynomial,
                                       AddRandomNoise(GenerateCodeword(6, generationElement, generationPolynomial, informationPolynomial5), 1), 5,
                                       informationPolynomial5
                                   }
                               };
        }

        public GsBasedDecoderTests()
        {
            var linearSystemsSolver = new GaussSolver();
            _decoder = new GsBasedDecoder(new GsDecoder(
                    new SimplePolynomialBuilder(linearSystemsSolver),
                    new RrFactorizator()),
                linearSystemsSolver);
        }

        [Theory]
        [MemberData(nameof(DecoderTestsData))]
        public void ShouldFindOriginalInformationWordAmongPossibleVariants(int n, int k, int d, Polynomial generatingPolynomial,
            Tuple<FieldElement, FieldElement>[] decodedCodeword, int minCorrectValuesCount, Polynomial expectedInformationPolynomial)
        {
            // When
            var possibleVariants = _decoder.Decode(n, k, d, generatingPolynomial, decodedCodeword, minCorrectValuesCount);

            // Then
            Assert.True(possibleVariants.Contains(expectedInformationPolynomial));
        }
    }
}