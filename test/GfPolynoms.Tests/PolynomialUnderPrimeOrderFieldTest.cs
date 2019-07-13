namespace AppliedAlgebra.GfPolynoms.Tests
{
    using System;
    using Extensions;
    using GaloisFields;
    using JetBrains.Annotations;
    using TestCases.PolynomialUnderPrimeOrderField;
    using Xunit;

    public class PolynomialUnderPrimeOrderFieldTest
    {
        private static readonly PrimeOrderField Gf2;
        private static readonly PrimeOrderField Gf3;

        [UsedImplicitly]
        public static readonly TheoryData<BinaryOperationTestCase> DevideTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<BinaryOperationTestCase> ModuloTestsData;

        static PolynomialUnderPrimeOrderFieldTest()
        {
            Gf2 = new PrimeOrderField(2);
            Gf3 = new PrimeOrderField(3);

            DevideTestsData
                = new TheoryData<BinaryOperationTestCase>
                  {
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {0}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {0}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {0, 1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 0, 0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {1, 0, 1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 0, 0, 0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {1, 1, 0, 1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 0, 0, 0, 0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {1, 1, 1, 0, 1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf3,
                          FirstOperandCoefficients = new[] {0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1},
                          ExpectedResultCoefficients = new[] {1, 2, 1}
                      }
                  };

            ModuloTestsData
                = new TheoryData<BinaryOperationTestCase>
                  {
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {0, 1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {1, 1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {0, 1, 1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 0, 0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {1, 1, 1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 0, 0, 0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {1, 0, 1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf2,
                          FirstOperandCoefficients = new[] {0, 0, 0, 0, 0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1, 0, 1},
                          ExpectedResultCoefficients = new[] {1}
                      },
                      new BinaryOperationTestCase
                      {
                          Field = Gf3,
                          FirstOperandCoefficients = new[] {0, 0, 0, 1},
                          SecondOperandCoefficients = new[] {1, 1},
                          ExpectedResultCoefficients = new[] {2}
                      }
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
        public void ShouldDevide(BinaryOperationTestCase testCase)
        {
            // Given
            var a = new Polynomial(testCase.Field, testCase.FirstOperandCoefficients);
            var b = new Polynomial(testCase.Field, testCase.SecondOperandCoefficients);

            // When
            var c = a/b;

            // Then
            Assert.Equal(new Polynomial(testCase.Field, testCase.ExpectedResultCoefficients), c);
        }

        [Theory]
        [MemberData(nameof(ModuloTestsData))]
        public void ShouldCalculateModulo(BinaryOperationTestCase testCase)
        {
            // Given
            var a = new Polynomial(testCase.Field, testCase.FirstOperandCoefficients);
            var b = new Polynomial(testCase.Field, testCase.SecondOperandCoefficients);

            // When
            var c = a%b;

            // Then
            Assert.Equal(new Polynomial(testCase.Field, testCase.ExpectedResultCoefficients), c);
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