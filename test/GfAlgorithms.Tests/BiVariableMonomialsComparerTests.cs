namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using BiVariablePolynomials;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class BiVariableMonomialsComparerTests
    {
        private readonly BiVariableMonomialsComparer _comparer;

        [UsedImplicitly]
        public static readonly TheoryData<BiVariableMonomialsComparisonTestCase> ComparisonTestsData;

        static BiVariableMonomialsComparerTests()
        {
            ComparisonTestsData
                = new TheoryData<BiVariableMonomialsComparisonTestCase>
                  {
                      new BiVariableMonomialsComparisonTestCase
                      {
                          FirstOperand = (1, 1), SecondOperand = (1, 1), ExpectedResult = 0
                      },
                      new BiVariableMonomialsComparisonTestCase
                      {
                          FirstOperand = (0, 1), SecondOperand = (1, 1), ExpectedResult = -1
                      },
                      new BiVariableMonomialsComparisonTestCase
                      {
                          FirstOperand = (1, 1), SecondOperand = (0, 1), ExpectedResult = 1
                      },
                      new BiVariableMonomialsComparisonTestCase
                      {
                          FirstOperand = (0, 2), SecondOperand = (3, 1), ExpectedResult = -1
                      },
                      new BiVariableMonomialsComparisonTestCase
                      {
                          FirstOperand = (3, 1), SecondOperand = (0, 2), ExpectedResult = 1
                      }
                  };
        }

        public BiVariableMonomialsComparerTests()
        {
            _comparer = new BiVariableMonomialsComparer(Tuple.Create(1, 3));
        }

        [Theory]
        [MemberData(nameof(ComparisonTestsData))]
        public void ShouldCompareTwoMonomials(BiVariableMonomialsComparisonTestCase testCase)
        {
            Assert.Equal(Math.Sign(testCase.ExpectedResult), Math.Sign(_comparer.Compare(testCase.FirstOperand, testCase.SecondOperand)));
        }
    }
}