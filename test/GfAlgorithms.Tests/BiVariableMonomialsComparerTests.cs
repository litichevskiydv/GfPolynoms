namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using BiVariablePolynomials;
    using JetBrains.Annotations;
    using Xunit;

    public class BiVariableMonomialsComparerTests
    {
        public class MonomialsComparsionTestCase
        {
            public Tuple<int, int> FirstOperand { get; set; }

            public Tuple<int, int> SecondOperand { get; set; }

            public int ExpectedResult { get; set; }
        }

        private readonly BiVariableMonomialsComparer _comparer;

        [UsedImplicitly]
        public static readonly TheoryData<MonomialsComparsionTestCase> ComparsionTestsData;

        static BiVariableMonomialsComparerTests()
        {
            ComparsionTestsData
                = new TheoryData<MonomialsComparsionTestCase>
                  {
                      new MonomialsComparsionTestCase
                      {
                          FirstOperand = Tuple.Create(1, 1), SecondOperand = Tuple.Create(1, 1), ExpectedResult = 0
                      },
                      new MonomialsComparsionTestCase
                      {
                          FirstOperand = Tuple.Create(0, 1), SecondOperand = Tuple.Create(1, 1), ExpectedResult = -1
                      },
                      new MonomialsComparsionTestCase
                      {
                          FirstOperand = Tuple.Create(1, 1), SecondOperand = Tuple.Create(0, 1), ExpectedResult = 1
                      },
                      new MonomialsComparsionTestCase
                      {
                          FirstOperand = Tuple.Create(0, 2), SecondOperand = Tuple.Create(3, 1), ExpectedResult = -1
                      },
                      new MonomialsComparsionTestCase
                      {
                          FirstOperand = Tuple.Create(3, 1), SecondOperand = Tuple.Create(0, 2), ExpectedResult = 1
                      }
                  };
        }

        public BiVariableMonomialsComparerTests()
        {
            _comparer = new BiVariableMonomialsComparer(Tuple.Create(1, 3));
        }

        [Theory]
        [MemberData(nameof(ComparsionTestsData))]
        public void ShoulCompareTwoMonomials(MonomialsComparsionTestCase testCase)
        {
            Assert.Equal(Math.Sign(testCase.ExpectedResult), Math.Sign(_comparer.Compare(testCase.FirstOperand, testCase.SecondOperand)));
        }
    }
}