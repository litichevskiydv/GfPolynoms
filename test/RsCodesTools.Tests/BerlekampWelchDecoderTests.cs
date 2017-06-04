namespace AppliedAlgebra.RsCodesTools.Tests
{
    using System;
    using System.Collections.Generic;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using StandartDecoder;
    using Xunit;

    public class BerlekampWelchDecoderTests
    {
        private readonly BerlekampWelchDecoder _decoder;

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

        private static object[] PrepareTestsWithErrors(int n, int k, Polynomial informationPolynomial, int randomErrorsCount)
        {
            return new object[]
                   {
                       n, k,
                       AddRandomNoise(GenerateCodeword(informationPolynomial), randomErrorsCount), randomErrorsCount,
                       informationPolynomial
                   };
        }

        private static object[] PrepareTestsDataWithoutErrors(int n, int k, Polynomial informationPolynomial, int errorsCount)
        {
            return new object[]
                   {
                       n, k,
                       GenerateCodeword(informationPolynomial), errorsCount,
                       informationPolynomial
                   };
        }

        static BerlekampWelchDecoderTests()
        {
            var gf8 = new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1));
            var gf9 = new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1));

            DecoderTestsData = new[]
                               {
                                   PrepareTestsWithErrors(7, 3, new Polynomial(gf8, 1, 2, 3), 2),
                                   PrepareTestsWithErrors(7, 3, new Polynomial(gf8, 7, 4, 1), 2),
                                   PrepareTestsWithErrors(7, 3, new Polynomial(gf8, 0, 2), 1),
                                   PrepareTestsWithErrors(7, 3, new Polynomial(gf8, 0, 0, 3), 1),
                                   PrepareTestsWithErrors(8, 5, new Polynomial(gf9, 0, 0, 3, 1, 1), 1),
                                   PrepareTestsDataWithoutErrors(8, 5, new Polynomial(gf9, 0, 0, 3, 1, 1), 1)
                               };
        }

        public BerlekampWelchDecoderTests()
        {
            _decoder = new BerlekampWelchDecoder(new GaussSolver());
        }

        [Theory]
        [MemberData(nameof(DecoderTestsData))]
        public void ShouldPerformDecodeReceivedCodevord(int n, int k, Tuple<FieldElement, FieldElement>[] decodedCodeword, int errorsCount, Polynomial expectedInformationPolynomial)
        {
            // When
            var actualInformationPolynomial = _decoder.Decode(n, k, decodedCodeword, errorsCount);

            // Then
            Assert.Equal(expectedInformationPolynomial, actualInformationPolynomial);
        }

        [Fact]
        public void ShouldNotPerformDecodeReceivedCodevord()
        {
            // Given
            var gf9 = new PrimePowerOrderField(9, new Polynomial(new PrimeOrderField(3), 1, 0, 1));
            const int n = 8;
            const int k = 5;
            const int errorsCount = 1;
            var informationPolynomial = new Polynomial(gf9);
            var decodedCodeword = AddNoise(GenerateCodeword(informationPolynomial), 1, 2);

            // When, Then
            Assert.Throws<InformationPolynomialWasNotFoundException>(() => _decoder.Decode(n, k, decodedCodeword, errorsCount));
        }
    }
}