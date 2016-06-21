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
        public static readonly IEnumerable<object[]> EvaluateMethodTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> AddMethodTestsData;

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
            EvaluateMethodTestsData = new[]
                                {
                                    new object[] {polynomial, new FieldElement(gf5, 1), new FieldElement(gf5, 3), gf5.Zero()},
                                    new object[] {polynomial, new FieldElement(gf5, 2), new FieldElement(gf5, 4), gf5.Zero()},
                                    new object[] {polynomial, new FieldElement(gf5, 3), new FieldElement(gf5, 2), gf5.Zero()},
                                    new object[] {polynomial, new FieldElement(gf5, 2), new FieldElement(gf5, 3), gf5.Zero()},
                                };

            AddMethodTestsData = new[]
                                 {
                                     new object[]
                                     {
                                         new BiVariablePolynomial(gf5)
                                         {
                                             [new Tuple<int, int>(0,0)] = new FieldElement(gf5, 2),
                                             [new Tuple<int, int>(1,0)] = new FieldElement(gf5, 3)
                                         },
                                         new BiVariablePolynomial(gf5)
                                         {
                                             [new Tuple<int, int>(0,0)] = new FieldElement(gf5, 3),
                                             [new Tuple<int, int>(0,1)] = new FieldElement(gf5, 4)
                                         },
                                         new BiVariablePolynomial(gf5)
                                         {
                                             [new Tuple<int, int>(1,0)] = new FieldElement(gf5, 3),
                                             [new Tuple<int, int>(0,1)] = new FieldElement(gf5, 4)
                                         }
                                     },
                                     new object[]
                                     {
                                         new BiVariablePolynomial(gf5)
                                         {
                                             [new Tuple<int, int>(0,0)] = new FieldElement(gf5, 2),
                                             [new Tuple<int, int>(1,0)] = new FieldElement(gf5, 3),
                                             [new Tuple<int, int>(0,1)] = new FieldElement(gf5, 4)
                                         },
                                         new BiVariablePolynomial(gf5)
                                         {
                                             [new Tuple<int, int>(0,1)] = new FieldElement(gf5, 4),
                                             [new Tuple<int, int>(0,0)] = new FieldElement(gf5, 4),
                                             [new Tuple<int, int>(1,0)] = new FieldElement(gf5, 4)
                                             
                                         },
                                         new BiVariablePolynomial(gf5)
                                         {
                                             [new Tuple<int, int>(1,0)] = new FieldElement(gf5, 2),
                                             [new Tuple<int, int>(0,1)] = new FieldElement(gf5, 3),
                                             [new Tuple<int, int>(0,0)] = new FieldElement(gf5, 1) 
                                         }
                                     },
                                 };
        }

        [Theory]
        [MemberData(nameof(EvaluateMethodTestsData))]
        public void ShouldEvaluateValue(BiVariablePolynomial polynomial, FieldElement xValue, FieldElement yValue, FieldElement expectedResult)
        {
            Assert.Equal(expectedResult, polynomial.Evaluate(xValue, yValue));
        }

        [Theory]
        [MemberData(nameof(AddMethodTestsData))]
        public void ShouldAddPolynomials(BiVariablePolynomial a, BiVariablePolynomial b, BiVariablePolynomial expectedResult)
        {
            Assert.Equal(expectedResult, a + b);
        }
    }
}