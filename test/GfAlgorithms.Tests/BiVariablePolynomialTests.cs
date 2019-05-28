namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using System.Collections.Generic;
    using BiVariablePolynomials;
    using CombinationsCountCalculator;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public class BiVariablePolynomialTests
    {
        public class EvaluationTestCase
        {
            public BiVariablePolynomial Polynomial { get; set; }

            public FieldElement XValue { get; set; }

            public FieldElement YValue { get; set; }

            public FieldElement Expected { get; set; }
        }

        private static readonly GaloisField Gf5;
        private readonly ICombinationsCountCalculator _combinationsCountCalculator;

        [UsedImplicitly]
        public static readonly TheoryData<EvaluationTestCase> EvaluateTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> AddTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> SubtractTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> MultiplyPolynomialsTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> MultiplyByFieldElementTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> SubstitutionTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> DivideByXDegreeTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> EvaluateXTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> EvaluateYTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> CalculateHasseDerivativeTestsData;

        static BiVariablePolynomialTests()
        {
            Gf5 = new PrimeOrderField(5);

            var polynomialForEvoluation = new BiVariablePolynomial(Gf5)
                             {
                                 [new Tuple<int, int>(1, 1)] = Gf5.CreateElement(2),
                                 [new Tuple<int, int>(0, 1)] = Gf5.One(),
                                 [new Tuple<int, int>(2, 0)] = Gf5.One(),
                                 [new Tuple<int, int>(1, 0)] = Gf5.One(),
                                 [new Tuple<int, int>(0, 0)] = Gf5.CreateElement(4)
            };
            EvaluateTestsData
                = new TheoryData<EvaluationTestCase>
                  {
                      new EvaluationTestCase
                      {
                          Polynomial = polynomialForEvoluation,
                          XValue = Gf5.One(),
                          YValue = Gf5.CreateElement(3),
                          Expected = Gf5.Zero()
                      },
                      new EvaluationTestCase
                      {
                          Polynomial = polynomialForEvoluation,
                          XValue = Gf5.CreateElement(2),
                          YValue = new FieldElement(Gf5, 4),
                          Expected = Gf5.Zero()
                      },
                      new EvaluationTestCase
                      {
                          Polynomial = polynomialForEvoluation,
                          XValue = Gf5.CreateElement(3),
                          YValue = Gf5.CreateElement(2),
                          Expected = Gf5.Zero()
                      },
                      new EvaluationTestCase
                      {
                          Polynomial = polynomialForEvoluation,
                          XValue = Gf5.CreateElement(2),
                          YValue = Gf5.CreateElement(3),
                          Expected = Gf5.Zero()
                      },
                  };

            AddTestsData = new[]
                           {
                               new object[]
                               {
                                   new BiVariablePolynomial(Gf5)
                                   {
                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                       [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3)
                                   },
                                   new BiVariablePolynomial(Gf5)
                                   {
                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 3),
                                       [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 4)
                                   },
                                   new BiVariablePolynomial(Gf5)
                                   {
                                       [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3),
                                       [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 4)
                                   }
                               },
                               new object[]
                               {
                                   new BiVariablePolynomial(Gf5)
                                   {
                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                       [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3),
                                       [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 4)
                                   },
                                   new BiVariablePolynomial(Gf5)
                                   {
                                       [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 4),
                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 4),
                                       [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 4)

                                   },
                                   new BiVariablePolynomial(Gf5)
                                   {
                                       [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 2),
                                       [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 3),
                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 1)
                                   }
                               }
                           };

            SubtractTestsData = new[]
                                {
                                    new object[]
                                    {
                                        new BiVariablePolynomial(Gf5)
                                        {
                                            [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                            [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3)
                                        },
                                        new BiVariablePolynomial(Gf5)
                                        {
                                            [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 3),
                                            [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 4)
                                        },
                                        new BiVariablePolynomial(Gf5)
                                        {
                                            [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 4),
                                            [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3),
                                            [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 1)
                                        }
                                    },
                                    new object[]
                                    {
                                        new BiVariablePolynomial(Gf5)
                                        {
                                            [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                            [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3),
                                            [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 4)
                                        },
                                        new BiVariablePolynomial(Gf5)
                                        {
                                            [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 4),
                                            [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 4),
                                            [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 4)

                                        },
                                        new BiVariablePolynomial(Gf5)
                                        {
                                            [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 3),
                                            [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 4)
                                        }
                                    }
                                };

            MultiplyPolynomialsTestsData = new[]
                                           {
                                               new object[]
                                               {
                                                   new BiVariablePolynomial(Gf5)
                                                   {
                                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                                       [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3)
                                                   },
                                                   new BiVariablePolynomial(Gf5)
                                                   {
                                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 3),
                                                       [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 4)
                                                   },
                                                   new BiVariablePolynomial(Gf5)
                                                   {
                                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 1),
                                                       [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 4),
                                                       [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 3),
                                                       [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 2)
                                                   }
                                               },
                                               new object[]
                                               {
                                                   new BiVariablePolynomial(Gf5)
                                                   {
                                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                                       [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3),
                                                       [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 4)
                                                   },
                                                   new BiVariablePolynomial(Gf5)
                                                   {
                                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 4),
                                                       [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 1),
                                                       [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 2)
                                                   },
                                                   new BiVariablePolynomial(Gf5)
                                                   {
                                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 3),
                                                       [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 4),
                                                       [new Tuple<int, int>(2, 0)] = new FieldElement(Gf5, 3),
                                                       [new Tuple<int, int>(0, 2)] = new FieldElement(Gf5, 3)
                                                   }
                                               },
                                               new object[]
                                               {
                                                   new BiVariablePolynomial(Gf5)
                                                   {
                                                       [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                                       [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3),
                                                       [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 4)
                                                   },
                                                   new BiVariablePolynomial(Gf5),
                                                   new BiVariablePolynomial(Gf5)
                                               }
                                           };

            var polynomialForMultiplication = new BiVariablePolynomial(Gf5)
                                              {
                                                  [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 3),
                                                  [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 2),
                                                  [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 4),
                                                  [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 1)
                                              };
            MultiplyByFieldElementTestsData = new[]
                                              {
                                                  new object[] {polynomialForMultiplication, Gf5.Zero(), new BiVariablePolynomial(Gf5)},
                                                  new object[]
                                                  {
                                                      polynomialForMultiplication, new FieldElement(Gf5, 2),
                                                      new BiVariablePolynomial(Gf5)
                                                      {
                                                          [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 1),
                                                          [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 4),
                                                          [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 3),
                                                          [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 2)
                                                      }
                                                  }
                                              };

            SubstitutionTestsData = new[]
                                    {
                                        new object[]
                                        {
                                            new BiVariablePolynomial(Gf5)
                                            {
                                                [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 1),
                                                [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3),
                                                [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 2)
                                            },
                                            new BiVariablePolynomial(Gf5)
                                            {
                                                [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 1)
                                            },
                                            new BiVariablePolynomial(Gf5)
                                            {
                                                [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 3),
                                                [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 1)
                                            },
                                            new BiVariablePolynomial(Gf5)
                                            {
                                                [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                                [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3),
                                                [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 2)
                                            }
                                        },
                                        new object[]
                                        {
                                            new BiVariablePolynomial(Gf5)
                                            {
                                                [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 1),
                                                [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 2),
                                                [new Tuple<int, int>(2, 0)] = new FieldElement(Gf5, 1),
                                                [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 3),
                                                [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 1)
                                            },
                                            new BiVariablePolynomial(Gf5)
                                            {
                                                [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 1),
                                                [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 1)
                                            },
                                            new BiVariablePolynomial(Gf5)
                                            {
                                                [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                                [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 1)
                                            },
                                            new BiVariablePolynomial(Gf5)
                                            {
                                                [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                                [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 1),
                                                [new Tuple<int, int>(2, 0)] = new FieldElement(Gf5, 1),
                                                [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 4),
                                                [new Tuple<int, int>(2, 1)] = new FieldElement(Gf5, 1)
                                            }
                                        },
                                        new object[]
                                        {
                                            new BiVariablePolynomial(Gf5)
                                            {
                                                [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 1),
                                                [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3),
                                                [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 2)
                                            },
                                            new BiVariablePolynomial(Gf5),
                                            new BiVariablePolynomial(Gf5),
                                            new BiVariablePolynomial(Gf5)
                                            {
                                                [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 1)
                                            }
                                        }
                                    };

            DivideByXDegreeTestsData = new[]
                                       {
                                           new object[]
                                           {
                                               new BiVariablePolynomial(Gf5)
                                               {
                                                   [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 2),
                                                   [new Tuple<int, int>(2, 0)] = new FieldElement(Gf5, 1),
                                                   [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 3)
                                               },
                                               new BiVariablePolynomial(Gf5)
                                               {
                                                   [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                                   [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 1),
                                                   [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 3)
                                               }
                                           },
                                           new object[]
                                           {
                                               new BiVariablePolynomial(Gf5)
                                               {
                                                   [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 2),
                                                   [new Tuple<int, int>(2, 0)] = new FieldElement(Gf5, 1),
                                                   [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 3)
                                               },
                                               new BiVariablePolynomial(Gf5)
                                               {
                                                   [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 2),
                                                   [new Tuple<int, int>(2, 0)] = new FieldElement(Gf5, 1),
                                                   [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 3)
                                               }
                                           },
                                           new object[]
                                           {
                                               new BiVariablePolynomial(Gf5),
                                               new BiVariablePolynomial(Gf5) 
                                           } 
                                       };

            EvaluateXTestsData = new[]
                                 {
                                     new object[]
                                     {
                                         new BiVariablePolynomial(Gf5)
                                         {
                                             [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 3),
                                             [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 2),
                                             [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 1)
                                         },
                                         new FieldElement(Gf5, 1),
                                         new Polynomial(Gf5, 0, 1)
                                     },
                                     new object[]
                                     {
                                         new BiVariablePolynomial(Gf5)
                                         {
                                             [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 3),
                                             [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 2),
                                             [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 1)
                                         },
                                         Gf5.Zero(),
                                         new Polynomial(Gf5, 3)
                                     },
                                     new object[]
                                     {
                                         new BiVariablePolynomial(Gf5)
                                         {
                                             [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                             [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3),
                                             [new Tuple<int, int>(2, 0)] = new FieldElement(Gf5, 2),
                                             [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 1),
                                             [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 1),
                                             [new Tuple<int, int>(0, 2)] = new FieldElement(Gf5, 1)
                                         },
                                         new FieldElement(Gf5, 3),
                                         new Polynomial(Gf5, 4, 4, 1)
                                     }
                                 };

            EvaluateYTestsData = new[]
                                 {
                                     new object[]
                                     {
                                         new BiVariablePolynomial(Gf5)
                                         {
                                             [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 3),
                                             [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 1),
                                             [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 2)
                                         },
                                         new FieldElement(Gf5, 1),
                                         new Polynomial(Gf5, 0, 1)
                                     },
                                     new object[]
                                     {
                                         new BiVariablePolynomial(Gf5)
                                         {
                                             [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 3),
                                             [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 2),
                                             [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 1)
                                         },
                                         Gf5.Zero(),
                                         new Polynomial(Gf5, 3)
                                     },
                                     new object[]
                                     {
                                         new BiVariablePolynomial(Gf5)
                                         {
                                             [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                             [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 3),
                                             [new Tuple<int, int>(2, 0)] = new FieldElement(Gf5, 2),
                                             [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 1),
                                             [new Tuple<int, int>(1, 1)] = new FieldElement(Gf5, 1),
                                             [new Tuple<int, int>(0, 2)] = new FieldElement(Gf5, 1)
                                         },
                                         new FieldElement(Gf5, 2),
                                         new Polynomial(Gf5, 3, 0, 2)
                                     }
                                 };

            CalculateHasseDerivativeTestsData = new[]
                                                {
                                                    new object[]
                                                    {
                                                        new BiVariablePolynomial(Gf5)
                                                        {
                                                            [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                                            [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 2),
                                                            [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 2),
                                                            [new Tuple<int, int>(2, 0)] = new FieldElement(Gf5, 1),
                                                            [new Tuple<int, int>(0, 2)] = new FieldElement(Gf5, 1)
                                                        },
                                                        1, 1, Gf5.One(), Gf5.One(),
                                                        Gf5.Zero()
                                                    },
                                                    new object[]
                                                    {
                                                        new BiVariablePolynomial(Gf5)
                                                        {
                                                            [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                                            [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 2),
                                                            [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 2),
                                                            [new Tuple<int, int>(2, 2)] = new FieldElement(Gf5, 1)
                                                        },
                                                        1, 1, Gf5.One(), Gf5.One(),
                                                        new FieldElement(Gf5, 4) 
                                                    },
                                                    new object[]
                                                    {
                                                        new BiVariablePolynomial(Gf5)
                                                        {
                                                            [new Tuple<int, int>(0, 0)] = new FieldElement(Gf5, 2),
                                                            [new Tuple<int, int>(1, 0)] = new FieldElement(Gf5, 2),
                                                            [new Tuple<int, int>(0, 1)] = new FieldElement(Gf5, 2),
                                                            [new Tuple<int, int>(2, 2)] = new FieldElement(Gf5, 1)
                                                        },
                                                        2, 1, Gf5.One(), Gf5.One(),
                                                        new FieldElement(Gf5, 2)
                                                    }
                                                };
        }

        public BiVariablePolynomialTests()
        {
            _combinationsCountCalculator = new PascalsTriangleBasedCalcualtor();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void ShouldSetCoefficients(int coefficientValueRepresentation)
        {
            // Given
            var polynomial = new BiVariablePolynomial(Gf5);
            var monomial = new Tuple<int, int>(1, 1);
            var coefficientValue = new FieldElement(Gf5, coefficientValueRepresentation);

            // When
            polynomial[monomial] = coefficientValue;

            // Then
            Assert.Equal(coefficientValue, polynomial[monomial]);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(0)]
        public void ShouldChangeCoefficients(int coefficientValueRepresentation)
        {
            // Given
            var monomial = new Tuple<int, int>(1, 1);
            var polynomial = new BiVariablePolynomial(Gf5) {[monomial] = Gf5.One()};
            var coefficientValue = new FieldElement(Gf5, coefficientValueRepresentation);

            // When
            polynomial[monomial] = coefficientValue;

            // Then
            Assert.Equal(coefficientValue, polynomial[monomial]);
        }

        [Theory]
        [MemberData(nameof(EvaluateTestsData))]
        public void ShouldEvaluateValue(EvaluationTestCase testCase)
        {
            Assert.Equal(testCase.Expected, testCase.Polynomial.Evaluate(testCase.XValue, testCase.YValue));
        }

        [Theory]
        [MemberData(nameof(AddTestsData))]
        public void ShouldAddTwoPolynomials(BiVariablePolynomial a, BiVariablePolynomial b, BiVariablePolynomial expectedResult)
        {
            Assert.Equal(expectedResult, a + b, EqualityComparer<BiVariablePolynomial>.Default);
        }

        [Theory]
        [MemberData(nameof(SubtractTestsData))]
        public void ShouldSubtractTwoPolynomials(BiVariablePolynomial a, BiVariablePolynomial b, BiVariablePolynomial expectedResult)
        {
            Assert.Equal(expectedResult, a - b, EqualityComparer<BiVariablePolynomial>.Default);
        }

        [Theory]
        [MemberData(nameof(MultiplyPolynomialsTestsData))]
        public void ShouldMultiplyTwoPolynomials(BiVariablePolynomial a, BiVariablePolynomial b, BiVariablePolynomial expectedResult)
        {
            Assert.Equal(expectedResult, a*b, EqualityComparer<BiVariablePolynomial>.Default);
        }

        [Theory]
        [MemberData(nameof(MultiplyByFieldElementTestsData))]
        public void ShouldMultiplyByFieldElement(BiVariablePolynomial a, FieldElement b, BiVariablePolynomial expectedResult)
        {
            Assert.Equal(expectedResult, a * b, EqualityComparer<BiVariablePolynomial>.Default);
        }

        [Theory]
        [MemberData(nameof(SubstitutionTestsData))]
        public void ShouldPerformVariablesSubstitution(BiVariablePolynomial polynomial,
            BiVariablePolynomial xSubstitution, BiVariablePolynomial ySubstitution,
            BiVariablePolynomial expectedResult)
        {
            Assert.Equal(expectedResult, polynomial.PerformVariablesSubstitution(xSubstitution, ySubstitution), EqualityComparer<BiVariablePolynomial>.Default);
        }

        [Theory]
        [MemberData(nameof(DivideByXDegreeTestsData))]
        public void ShouldDivideByMaxPossibleXDegree(BiVariablePolynomial polynomial, BiVariablePolynomial expectedResult)
        {
            Assert.Equal(expectedResult, polynomial.DivideByMaxPossibleXDegree(), EqualityComparer<BiVariablePolynomial>.Default);
        }

        [Theory]
        [MemberData(nameof(EvaluateXTestsData))]
        public void ShouldEvaluateX(BiVariablePolynomial polynomial, FieldElement xValue, Polynomial expectedResult)
        {
            Assert.Equal(expectedResult, polynomial.EvaluateX(xValue));
        }

        [Theory]
        [MemberData(nameof(EvaluateYTestsData))]
        public void ShouldEvaluateY(BiVariablePolynomial polynomial, FieldElement yValue, Polynomial expectedResult)
        {
            Assert.Equal(expectedResult, polynomial.EvaluateY(yValue));
        }

        [Theory]
        [MemberData(nameof(CalculateHasseDerivativeTestsData))]
        public void ShouldCalculateHasseDerivative(BiVariablePolynomial polynomial, int r, int s, FieldElement xValue, FieldElement yValue,
            FieldElement expectedValue)
        {
            Assert.Equal(expectedValue, polynomial.CalculateHasseDerivative(r, s, xValue, yValue, _combinationsCountCalculator));
        }
    }
}