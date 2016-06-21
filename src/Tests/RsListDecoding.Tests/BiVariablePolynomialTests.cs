namespace RsListDecoding.Tests
{
    using System;
    using System.Collections.Generic;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaluaFields;
    using JetBrains.Annotations;
    using Xunit;

    public class BiVariablePolynomialTests
    {
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> EvaluateTestsData;

        static BiVariablePolynomialTests()
        {
            var gf5 = new PrimeOrderField(5);
            var polynomial = new BiVariablePolynomial(gf5)
                             {
                                 [new Tuple<int, int>(1, 1)] = new FieldElement(gf5, 2),
                                 [new Tuple<int, int>(0, 1)] = new FieldElement(gf5, 1),
                                 [new Tuple<int, int>(2, 0)] = new FieldElement(gf5, 1),
                                 [new Tuple<int, int>(1, 0)] = new FieldElement(gf5, 1),
                                 [new Tuple<int, int>(0, 0)] = new FieldElement(gf5, 4)
                             };

            EvaluateTestsData = new[]
                                {
                                    new object[] {polynomial, new FieldElement(gf5, 1), new FieldElement(gf5, 3), gf5.Zero()},
                                    new object[] {polynomial, new FieldElement(gf5, 2), new FieldElement(gf5, 4), gf5.Zero()},
                                    new object[] {polynomial, new FieldElement(gf5, 3), new FieldElement(gf5, 2), gf5.Zero()},
                                    new object[] {polynomial, new FieldElement(gf5, 2), new FieldElement(gf5, 3), gf5.Zero()},
                                };
        }

        [Theory]
        [MemberData(nameof(EvaluateTestsData))]
        public void ShouldEvaluateValue(BiVariablePolynomial polynomial, FieldElement xValue, FieldElement yValue, FieldElement actualResult)
        {
            Assert.Equal(polynomial.Evaluate(xValue, yValue), actualResult);
        }
    }
}