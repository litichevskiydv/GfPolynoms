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
    using TestCases.BiVariablePolynomial;
    using Xunit;

    public class BiVariablePolynomialTests
    {

        private static readonly GaloisField Gf5;
        private readonly ICombinationsCountCalculator _combinationsCountCalculator;

        [UsedImplicitly]
        public static readonly TheoryData<EvaluationTestCase> EvaluateTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<BinaryOperationTestCase> AddTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<BinaryOperationTestCase> SubtractTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<BinaryOperationTestCase> MultiplyPolynomialsTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<MultiplicationByFieldElementTestCase> MultiplyByFieldElementTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<SubstitutionTestCase> SubstitutionTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<DivideByXDegreeTestCase> DivideByXDegreeTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<PartialEvaluationTestCase> EvaluateXTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<PartialEvaluationTestCase> EvaluateYTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<HasseDerivativeCalculationTestCase> HasseDerivativeCalculationTestsData;

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

            AddTestsData
                = new TheoryData<BinaryOperationTestCase>
                  {
                      new BinaryOperationTestCase
                      {
                          FirstOperand = new BiVariablePolynomial(Gf5)
                                         {
                                             [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                                             [Tuple.Create(1, 0)] = Gf5.CreateElement(3)
                                         },
                          SecondOperand = new BiVariablePolynomial(Gf5)
                                          {
                                              [Tuple.Create(0, 0)] = Gf5.CreateElement(3),
                                              [Tuple.Create(0, 1)] = Gf5.CreateElement(4)
                                          },
                          Expected = new BiVariablePolynomial(Gf5)
                                     {
                                         [Tuple.Create(1, 0)] = Gf5.CreateElement(3),
                                         [Tuple.Create(0, 1)] = Gf5.CreateElement(4)
                                     }
                      },
                      new BinaryOperationTestCase
                      {
                          FirstOperand = new BiVariablePolynomial(Gf5)
                                         {
                                             [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                                             [Tuple.Create(1, 0)] = Gf5.CreateElement(3),
                                             [Tuple.Create(0, 1)] = Gf5.CreateElement(4)
                                         },
                          SecondOperand = new BiVariablePolynomial(Gf5)
                                          {
                                              [Tuple.Create(0, 1)] = Gf5.CreateElement(4),
                                              [Tuple.Create(0, 0)] = Gf5.CreateElement(4),
                                              [Tuple.Create(1, 0)] = Gf5.CreateElement(4)

                                          },
                          Expected = new BiVariablePolynomial(Gf5)
                                     {
                                         [Tuple.Create(1, 0)] = Gf5.CreateElement(2),
                                         [Tuple.Create(0, 1)] = Gf5.CreateElement(3),
                                         [Tuple.Create(0, 0)] = Gf5.One()
                                     }
                      }
                  };

            SubtractTestsData
                = new TheoryData<BinaryOperationTestCase>
                  {
                      new BinaryOperationTestCase
                      {
                          FirstOperand = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(3)
                          },
                          SecondOperand = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(4)
                          },
                          Expected = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(4),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(0, 1)] = Gf5.One()
                          }
                      },
                      new BinaryOperationTestCase
                      {
                          FirstOperand = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(4)
                          },
                          SecondOperand = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(4),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(4),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(4)

                          },
                          Expected = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(4)
                          }
                      }
                  };

            MultiplyPolynomialsTestsData
                = new TheoryData<BinaryOperationTestCase>
                  {
                      new BinaryOperationTestCase
                      {
                          FirstOperand = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(3)
                          },
                          SecondOperand = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(4)
                          },
                          Expected = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.One(),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(4),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(3),
                              [Tuple.Create(1, 1)] = Gf5.CreateElement(2)
                          }
                      },
                      new BinaryOperationTestCase
                      {
                          FirstOperand = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(4)
                          },
                          SecondOperand = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(4),
                              [Tuple.Create(1, 0)] = Gf5.One(),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(2)
                          },
                          Expected = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(4),
                              [Tuple.Create(2, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(0, 2)] = Gf5.CreateElement(3)
                          }
                      },
                      new BinaryOperationTestCase
                      {
                          FirstOperand = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(4)
                          },
                          SecondOperand = new BiVariablePolynomial(Gf5),
                          Expected = new BiVariablePolynomial(Gf5)
                      }
                  };

            var polynomialForMultiplication
                = new BiVariablePolynomial(Gf5)
                  {
                      [Tuple.Create(0, 0)] = Gf5.CreateElement(3),
                      [Tuple.Create(1, 0)] = Gf5.CreateElement(2),
                      [Tuple.Create(0, 1)] = Gf5.CreateElement(4),
                      [Tuple.Create(1, 1)] = Gf5.One()
                  };
            MultiplyByFieldElementTestsData
                = new TheoryData<MultiplicationByFieldElementTestCase>
                  {
                      new MultiplicationByFieldElementTestCase
                      {
                          Polynomial = polynomialForMultiplication,
                          Multiplier = Gf5.Zero(),
                          Expected = new BiVariablePolynomial(Gf5)
                      },
                      new MultiplicationByFieldElementTestCase
                      {
                          Polynomial = polynomialForMultiplication,
                          Multiplier = Gf5.CreateElement(2),
                          Expected = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.One(),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(4),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(3),
                              [Tuple.Create(1, 1)] = Gf5.CreateElement(2)
                          }
                      }
                  };

            SubstitutionTestsData
                = new TheoryData<SubstitutionTestCase>
                  {
                      new SubstitutionTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                                       {
                                           [Tuple.Create(0, 0)] = Gf5.One(),
                                           [Tuple.Create(1, 0)] = Gf5.CreateElement(3),
                                           [Tuple.Create(0, 1)] = Gf5.CreateElement(2)
                                       },
                          XSubstitution = new BiVariablePolynomial(Gf5)
                                          {
                                              [Tuple.Create(1, 0)] = Gf5.One()
                                          },
                          YSubstitution = new BiVariablePolynomial(Gf5)
                                          {
                                              [Tuple.Create(0, 0)] = Gf5.CreateElement(3),
                                              [Tuple.Create(1, 1)] = Gf5.One()
                                          },
                          Expected = new BiVariablePolynomial(Gf5)
                                     {
                                         [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                                         [Tuple.Create(1, 0)] = Gf5.CreateElement(3),
                                         [Tuple.Create(1, 1)] = Gf5.CreateElement(2)
                                     }
                      },
                      new SubstitutionTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                                       {
                                           [Tuple.Create(0, 0)] = Gf5.One(),
                                           [Tuple.Create(1, 0)] = Gf5.CreateElement(2),
                                           [Tuple.Create(2, 0)] = Gf5.One(),
                                           [Tuple.Create(0, 1)] = Gf5.CreateElement(3),
                                           [Tuple.Create(1, 1)] = Gf5.One()
                                       },
                          XSubstitution = new BiVariablePolynomial(Gf5)
                                          {
                                              [Tuple.Create(0, 0)] = Gf5.One(),
                                              [Tuple.Create(1, 0)] = Gf5.One()
                                          },
                          YSubstitution = new BiVariablePolynomial(Gf5)
                                          {
                                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                                              [Tuple.Create(1, 1)] = Gf5.One()
                                          },
                          Expected = new BiVariablePolynomial(Gf5)
                                     {
                                         [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                                         [Tuple.Create(1, 0)] = Gf5.One(),
                                         [Tuple.Create(2, 0)] = Gf5.One(),
                                         [Tuple.Create(1, 1)] = Gf5.CreateElement(4),
                                         [Tuple.Create(2, 1)] = Gf5.One()
                                     }
                      },
                      new SubstitutionTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                                       {
                                           [Tuple.Create(0, 0)] = Gf5.One(),
                                           [Tuple.Create(1, 0)] = Gf5.CreateElement(3),
                                           [Tuple.Create(0, 1)] = Gf5.CreateElement(2)
                                       },
                          XSubstitution = new BiVariablePolynomial(Gf5),
                          YSubstitution = new BiVariablePolynomial(Gf5),
                          Expected = new BiVariablePolynomial(Gf5)
                                     {
                                         [Tuple.Create(0, 0)] = Gf5.One()
                                     }
                      }
                  };

            DivideByXDegreeTestsData
                = new TheoryData<DivideByXDegreeTestCase>
                  {
                      new DivideByXDegreeTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(2, 0)] = Gf5.One(),
                              [Tuple.Create(1, 1)] = Gf5.CreateElement(3)
                          },
                          Expected = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 0)] = Gf5.One(),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(3)
                          }
                      },
                      new DivideByXDegreeTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(2, 0)] = Gf5.One(),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(3)
                          },
                          Expected = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(2, 0)] = Gf5.One(),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(3)
                          }
                      },
                      new DivideByXDegreeTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5),
                          Expected = new BiVariablePolynomial(Gf5)
                      }
                  };

            EvaluateXTestsData
                = new TheoryData<PartialEvaluationTestCase>
                  {
                      new PartialEvaluationTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(0, 1)] = Gf5.One()
                          },
                          VariableValue = Gf5.One(),
                          Expected = new Polynomial(Gf5, 0, 1)
                      },
                      new PartialEvaluationTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 1)] = Gf5.One()
                          },
                          VariableValue = Gf5.Zero(),
                          Expected = new Polynomial(Gf5, 3)
                      },
                      new PartialEvaluationTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(2, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(0, 1)] = Gf5.One(),
                              [Tuple.Create(1, 1)] = Gf5.One(),
                              [Tuple.Create(0, 2)] = Gf5.One()
                          },
                          VariableValue = Gf5.CreateElement(3),
                          Expected = new Polynomial(Gf5, 4, 4, 1)
                      }
                  };

            EvaluateYTestsData
                = new TheoryData<PartialEvaluationTestCase>
                  {
                      new PartialEvaluationTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(1, 0)] = Gf5.One(),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(2)
                          },
                          VariableValue = Gf5.One(),
                          Expected = new Polynomial(Gf5, 0, 1)
                      },
                      new PartialEvaluationTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 1)] = Gf5.One()
                          },
                          VariableValue = Gf5.Zero(),
                          Expected = new Polynomial(Gf5, 3)
                      },
                      new PartialEvaluationTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(3),
                              [Tuple.Create(2, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(0, 1)] = Gf5.One(),
                              [Tuple.Create(1, 1)] = Gf5.One(),
                              [Tuple.Create(0, 2)] = Gf5.One()
                          },
                          VariableValue = Gf5.CreateElement(2),
                          Expected = new Polynomial(Gf5, 3, 0, 2)
                      }
                  };

            HasseDerivativeCalculationTestsData
                = new TheoryData<HasseDerivativeCalculationTestCase>
                  {
                      new HasseDerivativeCalculationTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(2),
                              [Tuple.Create(2, 0)] = Gf5.One(),
                              [Tuple.Create(0, 2)] = Gf5.One()
                          },
                          R = 1,
                          S = 1,
                          XValue = Gf5.One(),
                          YValue = Gf5.One(),
                          Expected = Gf5.Zero()
                      },
                      new HasseDerivativeCalculationTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(2),
                              [Tuple.Create(2, 2)] = Gf5.One()
                          },
                          R = 1,
                          S = 1,
                          XValue = Gf5.One(),
                          YValue = Gf5.One(),
                          Expected = new FieldElement(Gf5, 4)
                      },
                      new HasseDerivativeCalculationTestCase
                      {
                          Polynomial = new BiVariablePolynomial(Gf5)
                          {
                              [Tuple.Create(0, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(1, 0)] = Gf5.CreateElement(2),
                              [Tuple.Create(0, 1)] = Gf5.CreateElement(2),
                              [Tuple.Create(2, 2)] = Gf5.One()
                          },
                          R = 2,
                          S = 1,
                          XValue = Gf5.One(),
                          YValue = Gf5.One(),
                          Expected = Gf5.CreateElement(2)
                      }
                  };
        }

        public BiVariablePolynomialTests()
        {
            _combinationsCountCalculator = new PascalsTriangleBasedCalculator();
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
        public void ShouldAddTwoPolynomials(BinaryOperationTestCase testCase)
        {
            Assert.Equal(testCase.Expected, testCase.FirstOperand + testCase.SecondOperand, EqualityComparer<BiVariablePolynomial>.Default);
        }

        [Theory]
        [MemberData(nameof(SubtractTestsData))]
        public void ShouldSubtractTwoPolynomials(BinaryOperationTestCase testCase)
        {
            Assert.Equal(testCase.Expected, testCase.FirstOperand - testCase.SecondOperand, EqualityComparer<BiVariablePolynomial>.Default);
        }

        [Theory]
        [MemberData(nameof(MultiplyPolynomialsTestsData))]
        public void ShouldMultiplyTwoPolynomials(BinaryOperationTestCase testCase)
        {
            Assert.Equal(testCase.Expected, testCase.FirstOperand * testCase.SecondOperand, EqualityComparer<BiVariablePolynomial>.Default);
        }

        [Theory]
        [MemberData(nameof(MultiplyByFieldElementTestsData))]
        public void ShouldMultiplyByFieldElement(MultiplicationByFieldElementTestCase testCase)
        {
            Assert.Equal(testCase.Expected, testCase.Polynomial * testCase.Multiplier, EqualityComparer<BiVariablePolynomial>.Default);
        }

        [Theory]
        [MemberData(nameof(SubstitutionTestsData))]
        public void ShouldPerformVariablesSubstitution(SubstitutionTestCase testCase)
        {
            Assert.Equal(
                testCase.Expected,
                testCase.Polynomial.PerformVariablesSubstitution(testCase.XSubstitution, testCase.YSubstitution),
                EqualityComparer<BiVariablePolynomial>.Default
            );
        }

        [Theory]
        [MemberData(nameof(DivideByXDegreeTestsData))]
        public void ShouldDivideByMaxPossibleXDegree(DivideByXDegreeTestCase testCase)
        {
            Assert.Equal(testCase.Expected, testCase.Polynomial.DivideByMaxPossibleXDegree(), EqualityComparer<BiVariablePolynomial>.Default);
        }

        [Theory]
        [MemberData(nameof(EvaluateXTestsData))]
        public void ShouldEvaluateX(PartialEvaluationTestCase testCase)
        {
            Assert.Equal(testCase.Expected, testCase.Polynomial.EvaluateX(testCase.VariableValue));
        }

        [Theory]
        [MemberData(nameof(EvaluateYTestsData))]
        public void ShouldEvaluateY(PartialEvaluationTestCase testCase)
        {
            Assert.Equal(testCase.Expected, testCase.Polynomial.EvaluateY(testCase.VariableValue));
        }

        [Theory]
        [MemberData(nameof(HasseDerivativeCalculationTestsData))]
        public void ShouldCalculateHasseDerivative(HasseDerivativeCalculationTestCase testCase)
        {
            Assert.Equal(
                testCase.Expected,
                testCase.Polynomial.CalculateHasseDerivative(
                    testCase.R,
                    testCase.S,
                    testCase.XValue,
                    testCase.YValue,
                    _combinationsCountCalculator
                )
            );
        }
    }
}