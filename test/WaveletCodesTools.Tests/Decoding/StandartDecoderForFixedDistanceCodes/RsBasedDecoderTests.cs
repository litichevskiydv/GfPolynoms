﻿namespace AppliedAlgebra.WaveletCodesTools.Tests.Decoding.StandartDecoderForFixedDistanceCodes
{
    using System;
    using System.Collections.Generic;
    using GfAlgorithms.LinearSystemSolver;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using RsCodesTools.Decoding.StandartDecoder;
    using TestCases;
    using WaveletCodesTools.Decoding.StandartDecoderForFixedDistanceCodes;
    using WaveletCodesTools.Encoding;
    using Xunit;
    using InformationPolynomialWasNotFoundException = WaveletCodesTools.Decoding.StandartDecoderForFixedDistanceCodes.InformationPolynomialWasNotFoundException;

    public class RsBasedDecoderTests
    {
        private readonly RsBasedDecoder _decoder;

        [UsedImplicitly]
        public static readonly TheoryData<DecoderTestCase> DecoderTestsData;

        private static (FieldElement xValue, FieldElement yValue)[] AddRandomNoise(
            (FieldElement xValue, FieldElement yValue)[] codeword,
            int errorsCount
        )
        {
            var random = new Random();
            var errorsPositions = new HashSet<int>();
            while (errorsPositions.Count < errorsCount)
                errorsPositions.Add(random.Next(codeword.Length));

            var one = codeword[0].xValue.Field.One();
            foreach (var errorPosition in errorsPositions)
                codeword[errorPosition].yValue.Add(one);

            return codeword;
        }

        private static DecoderTestCase PrepareDataWithErrors(int n, int k, int d, Polynomial generationPolynomial,
            IEncoder encoder, Polynomial informationPolynomial, int randomErrorsCount) =>
            new DecoderTestCase
            {
                N = n,
                K = k,
                D = d,
                GeneratingPolynomial = generationPolynomial,
                DecodedCodeword = AddRandomNoise(encoder.Encode(n, generationPolynomial, informationPolynomial), randomErrorsCount),
                MinCorrectValuesCount = randomErrorsCount,
                Expected = informationPolynomial
            };

        private static DecoderTestCase PrepareDataWithoutErrors(int n, int k, int d, Polynomial generationPolynomial,
            IEncoder encoder, Polynomial informationPolynomial, int errorsCount) =>
            new DecoderTestCase
            {
                N = n,
                K = k,
                D = d,
                GeneratingPolynomial = generationPolynomial,
                DecodedCodeword = encoder.Encode(n, generationPolynomial, informationPolynomial),
                MinCorrectValuesCount = errorsCount,
                Expected = informationPolynomial
            };

        static RsBasedDecoderTests()
        {
            var gf7 = GaloisField.Create(7);
            var generationPolynomial1 = new Polynomial(gf7, 4, 2, 6, 4, 3, 4)
                                        + new Polynomial(gf7, 1, 2, 1, 5, 2, 1).RightShift(2);

            var gf8 = GaloisField.Create(8, new[] {1, 1, 0, 1});
            var generationPolynomial2 = new Polynomial(gf8, 6, 4, 2, 7, 4, 7, 4);

            var gf9 = GaloisField.Create(9, new[] {1, 0, 1});
            var generationPolynomial3 = new Polynomial(gf9, 1, 2, 7, 2, 2, 2, 1, 5, 7);

            var gf11 = GaloisField.Create(11);
            var generationPolynomial4 = new Polynomial(gf11, 0, 0, 7, 3, 4, 1, 8, 1, 8, 2, 7, 5);
            var generationPolynomial5 = new Polynomial(gf11, 0, 0, 2, 0, 10, 9, 3, 9, 3, 10, 2, 2);

            var gf13 = GaloisField.Create(13);
            var generationPolynomial6 = new Polynomial(gf13, 0, 0, 0, 8, 1, 12, 2, 11, 5, 6, 4, 2, 3, 12, 2, 4);

            var gf27 = GaloisField.Create(27, new[] {2, 2, 0, 1});
            var generationPolynomial7 = new Polynomial(gf27, 0, 0, 20, 18, 14, 15, 2, 5, 2, 19, 17, 4, 23, 1, 8, 6, 5, 4, 20, 26, 6, 5, 16,
                23, 26, 15, 6, 25, 18, 22, 8, 4, 17, 20, 19, 18, 8, 6, 23, 12, 20, 22, 8, 7, 0, 7, 6, 3, 11);

            var encoder = new Encoder();
            DecoderTestsData
                = new TheoryData<DecoderTestCase>
                  {
                      PrepareDataWithErrors(6, 3, 3, generationPolynomial1, encoder, new Polynomial(gf7, 4, 0, 2), 1),
                      PrepareDataWithErrors(6, 3, 3, generationPolynomial1, encoder, new Polynomial(gf7, 1, 2, 3), 1),
                      PrepareDataWithErrors(6, 3, 3, generationPolynomial1, encoder, new Polynomial(gf7, 6, 4, 1), 1),
                      PrepareDataWithErrors(6, 3, 3, generationPolynomial1, encoder, new Polynomial(gf7, 0, 2), 1),
                      PrepareDataWithErrors(6, 3, 3, generationPolynomial1, encoder, new Polynomial(gf7, 0, 0, 3), 1),
                      PrepareDataWithErrors(7, 3, 3, generationPolynomial2, encoder, new Polynomial(gf8, 0, 0, 3), 1),
                      PrepareDataWithErrors(7, 3, 3, generationPolynomial2, encoder, new Polynomial(gf8, 1), 1),
                      PrepareDataWithErrors(7, 3, 3, generationPolynomial2, encoder, new Polynomial(gf8, 0, 0, 1), 1),
                      PrepareDataWithErrors(8, 4, 4, generationPolynomial3, encoder, new Polynomial(gf9, 0, 0, 3), 1),
                      PrepareDataWithErrors(8, 4, 4, generationPolynomial3, encoder, new Polynomial(gf9, 1, 2, 3, 4), 1),
                      PrepareDataWithErrors(8, 4, 4, generationPolynomial3, encoder, new Polynomial(gf9, 1, 2), 1),
                      PrepareDataWithErrors(8, 4, 4, generationPolynomial3, encoder, new Polynomial(gf9, 0, 0, 0, 6), 1),
                      PrepareDataWithErrors(10, 5, 6, generationPolynomial4, encoder, new Polynomial(gf11, 1, 2, 3, 4, 5), 2),
                      PrepareDataWithErrors(10, 5, 5, generationPolynomial5, encoder, new Polynomial(gf11, 1, 2, 3, 4, 5), 2),
                      PrepareDataWithErrors(12, 6, 6, generationPolynomial6, encoder, new Polynomial(gf13, 1, 2, 3, 4, 5, 6), 2),
                      PrepareDataWithErrors(12, 6, 6, generationPolynomial6, encoder, new Polynomial(gf13, 0, 2, 0, 2, 11), 2),
                      PrepareDataWithErrors(26, 13, 12, generationPolynomial7, encoder, new Polynomial(gf27, 0, 2, 0, 2, 11), 5),
                      PrepareDataWithoutErrors(26, 13, 12, generationPolynomial7, encoder, new Polynomial(gf27, 0, 2, 0, 2, 11), 5)
                  };
        }

        public RsBasedDecoderTests()
        {
            var linearSystemSolver = new GaussSolver();
            _decoder = new RsBasedDecoder(new BerlekampWelchDecoder(linearSystemSolver), linearSystemSolver);
        }

        [Theory]
        [MemberData(nameof(DecoderTestsData))]
        public void ShouldFindOriginalInformationWordAmongPossibleVariants(DecoderTestCase testCase)
        {
            // When
            var actualInformationPolynomial = _decoder.Decode(
                testCase.N,
                testCase.K,
                testCase.D,
                testCase.GeneratingPolynomial,
                testCase.DecodedCodeword,
                testCase.MinCorrectValuesCount
            );

            // Then
            Assert.Equal(testCase.Expected, actualInformationPolynomial);
        }

        [Fact]
        public void ShouldNotPerformDecodeReceivedCodeword()
        {
            // Given
            var gf27 = GaloisField.Create(27, new[] {2, 2, 0, 1});
            var generationPolynomial = new Polynomial(gf27, 0, 0, 20, 18, 14, 15, 2, 5, 2, 19, 17, 4, 23, 1, 8, 6, 5, 4, 20, 26, 6, 5, 16,
                23, 26, 15, 6, 25, 18, 22, 8, 4, 17, 20, 19, 18, 8, 6, 23, 12, 20, 22, 8, 7, 0, 7, 6, 3, 11);
            const int n = 26;
            const int k = 13;
            const int d = 12;
            const int errorsCount = 5;
            var informationPolynomial = new Polynomial(gf27, 0, 2, 0, 2, 11);
            var decodedCodeword = AddRandomNoise(new Encoder().Encode(n, generationPolynomial, informationPolynomial), errorsCount + 1);

            // When, Then
            Assert.Throws<InformationPolynomialWasNotFoundException>(() => _decoder.Decode(n, k, d, generationPolynomial, decodedCodeword, errorsCount));
        }
    }
}