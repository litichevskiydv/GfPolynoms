namespace AppliedAlgebra.GfPolynoms.Tests
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    using GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public class PolynomialUnderPrimeOrderFieldTest
    {
        private static readonly PrimeOrderField Gf2;
        private static readonly PrimeOrderField Gf3;

        [UsedImplicitly]
        public static readonly IEnumerable<object[]> DevideTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> ModuloTestsData;

        static PolynomialUnderPrimeOrderFieldTest()
        {
            Gf2 = new PrimeOrderField(2);
            Gf3 = new PrimeOrderField(3);


            DevideTestsData = new[]
                               {
                                   new object[] {Gf2, new[] {1}, new[] {1, 1, 0, 1}, new[] {0}},
                                   new object[] {Gf2, new[] {0, 1}, new[] {1, 1, 0, 1}, new[] {0}},
                                   new object[] {Gf2, new[] {0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1}},
                                   new object[] {Gf2, new[] {0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {0, 1}},
                                   new object[] {Gf2, new[] {0, 0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1, 0, 1}},
                                   new object[] {Gf2, new[] {0, 0, 0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1, 1, 0, 1}},
                                   new object[] {Gf2, new[] {0, 0, 0, 0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1, 1, 1, 0, 1 } },
                                   new object[] {Gf3, new[] {0, 0, 0, 1}, new[] {1, 1}, new[] {1, 2, 1}}
                               };
            ModuloTestsData = new[]
                               {
                                   new object[] {Gf2, new[] {1}, new[] {1, 1, 0, 1}, new[] {1}},
                                   new object[] {Gf2, new[] {0, 1}, new[] {1, 1, 0, 1}, new[] {0, 1}},
                                   new object[] {Gf2, new[] {0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1, 1}},
                                   new object[] {Gf2, new[] {0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {0, 1, 1}},
                                   new object[] {Gf2, new[] {0, 0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1, 1, 1}},
                                   new object[] {Gf2, new[] {0, 0, 0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1, 0, 1}},
                                   new object[] {Gf2, new[] {0, 0, 0, 0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1}},
                                   new object[] {Gf3, new[] {0, 0, 0, 1}, new[] {1, 1}, new[] {2}}
                               };

        }

        [Fact]
        public void ShouldCreatePolynomFromArray1()
        {
            // Given
            var a = new Polynomial(Gf2, 0, 1, 1, 0);

            //Then
            Assert.Equal(2, a.Degree);
            Assert.Equal(0, a[0]);
            Assert.Equal(1, a[1]);
            Assert.Equal(1, a[2]);
        }

        [Fact]
        public void ShouldCreatePolynomFromArray2()
        {
            // Given
            var a = new Polynomial(Gf2, 0, 0, 0, 0);

            //Then
            Assert.Equal(0, a.Degree);
        }

        [Theory]
        [InlineData(new[] {1, 1}, new[] {0, 1, 1}, new[] {1, 0, 1})]
        [InlineData(new[] {1, 1, 1}, new[] {1, 0, 1}, new[] {0, 1})]
        public void ShouldSumTwoPolynomsUnderGf2(int[] firstItemCoefficients, int[] secondItemCoefficients, int[] sumCoefficients)
        {
            // Given
            var a = new Polynomial(Gf2, firstItemCoefficients);
            var b = new Polynomial(Gf2, secondItemCoefficients);

            // When
            var c = a + b;

            // Then
            Assert.Equal(new Polynomial(Gf2, sumCoefficients), c);
        }

        [Theory]
        [InlineData(new[] {0, 1, 1}, new[] {1, 0, 1}, new[] {1, 1})]
        [InlineData(new[] {0, 0, 1}, new[] {1, 1}, new[] {1, 1, 1})]
        [InlineData(new[] {1, 1}, new[] {1, 1}, new[] {0})]
        public void ShouldSubtractTwoPolynomsUnderGf2(int[] minuendCoefficients, int[] subtrahendCoefficients, int[] differenceCoefficients)
        {
            // Given
            var a = new Polynomial(Gf2, minuendCoefficients);
            var b = new Polynomial(Gf2, subtrahendCoefficients);

            // When
            var c = a - b;

            // Then
            Assert.Equal(new Polynomial(Gf2, differenceCoefficients), c);
        }

        [Theory]
        [InlineData(new[] {1, 1}, new[] {1, 1}, new[] {1, 0, 1})]
        [InlineData(new[] {0, 1}, new[] {1, 1}, new[] {0, 1, 1})]
        [InlineData(new[] {0, 1, 1}, new[] {1, 1}, new[] {0, 1, 0, 1})]
        public void ShouldMultiplyTwoPolynomsUnderGf2(int[] firstMultipliedCoefficients, int[] secondMultipliedCoefficients,
            int[] productCoefficients)
        {
            // Given
            var a = new Polynomial(Gf2, firstMultipliedCoefficients);
            var b = new Polynomial(Gf2, secondMultipliedCoefficients);

            // When
            var c = a*b;

            // Then
            Assert.Equal(new Polynomial(Gf2, productCoefficients), c);
        }

        [Fact]
        public void ShouldThrowArgumentExceptionWhenDevideByZero()
        {
            // Given
            var a = new Polynomial(Gf2, 1);
            var b = new Polynomial(Gf2);

            // Then
            Assert.Throws<ArgumentException>(() => a/b);
        }

        [Theory]
        [MemberData(nameof(DevideTestsData))]
        public void ShouldDevide(PrimeOrderField field, int[] dividendCoefficients, int[] divisorCoefficients, int[] quotientCoefficients)
        {
            // Given
            var a = new Polynomial(field, dividendCoefficients);
            var b = new Polynomial(field, divisorCoefficients);

            // When
            var c = a/b;

            // Then
            Assert.Equal(new Polynomial(field, quotientCoefficients), c);
        }

        [Theory]
        [MemberData(nameof(ModuloTestsData))]
        public void ShouldCalculateModulo(PrimeOrderField field, int[] dividendCoefficients, int[] divisorCoefficients, int[] remainderCoefficients)
        {
            // Given
            var a = new Polynomial(field, dividendCoefficients);
            var b = new Polynomial(field, divisorCoefficients);

            // When
            var c = a%b;

            // Then
            Assert.Equal(new Polynomial(field, remainderCoefficients), c);
        }

        [Theory]
        [InlineData(new[] {1, 1}, 2, new[] {0, 0, 1, 1})]
        [InlineData(new[] {1}, 4, new[] {0, 0, 0, 0, 1})]
        public void ShouldPerformRightShift(int[] initialCoefficients, int shift, int[] resultCoefficients)
        {
            // Given
            var a = new Polynomial(Gf2, initialCoefficients);

            // When
            var c = a >> shift;

            // Then
            Assert.Equal(new Polynomial(Gf2, resultCoefficients), c);
        }

        [Theory]
        [InlineData(new[] {1, 2, 1}, 2, new[] {1, 0, 2, 0, 1})]
        [InlineData(new[] {1, 1, 1}, 1, new[] { 1, 1, 1 })]
        public void ShouldRaiseVariableDegre(int[] initialCoefficients, int newDegree, int[] resultCoefficients)
        {
            // Given
            var a = new Polynomial(Gf3, initialCoefficients);

            // When
            var c = a.RaiseVariableDegree(newDegree);

            // Then
            Assert.Equal(new Polynomial(Gf3, resultCoefficients), c);
        }

        [Theory]
        [InlineData(new[] {1, 2, 1}, 2, 0)]
        [InlineData(new[] { 1, 2, 2 }, 2, 1)]
        public void ShouldEvaluateValueUnderGf3(int[] coefficients, int parameterValue, int expected)
        {
            // Given
            var a = new Polynomial(Gf3, coefficients);

            // When
            var actual = a.Evaluate(parameterValue);

            // Then
            Assert.Equal(expected, actual);
        }
    }
}