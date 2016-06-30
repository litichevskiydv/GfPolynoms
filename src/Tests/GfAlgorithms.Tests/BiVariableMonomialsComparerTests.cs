namespace GfAlgorithms.Tests
{
    using System;
    using System.Collections.Generic;
    using BiVariablePolynomials;
    using JetBrains.Annotations;
    using Xunit;

    public class BiVariableMonomialsComparerTests
    {
        private readonly BiVariableMonomialsComparer _comparer;

        [UsedImplicitly]
        public static readonly IEnumerable<object[]> ComparsionTestsData;

        static BiVariableMonomialsComparerTests()
        {
            ComparsionTestsData = new[]
                                  {
                                      new object[] {new Tuple<int, int>(1, 1), new Tuple<int, int>(1, 1), 0},
                                      new object[] {new Tuple<int, int>(0, 1), new Tuple<int, int>(1, 1), -1},
                                      new object[] {new Tuple<int, int>(1, 1), new Tuple<int, int>(0, 1), 1},
                                      new object[] {new Tuple<int, int>(0, 2), new Tuple<int, int>(3, 1), -1},
                                      new object[] {new Tuple<int, int>(3, 1), new Tuple<int, int>(0, 2), 1}
                                  };
        }

        public BiVariableMonomialsComparerTests()
        {
            _comparer = new BiVariableMonomialsComparer(new Tuple<int, int>(1, 3));
        }

        [Theory]
        [MemberData(nameof(ComparsionTestsData))]
        public void ShoulCompareTwoMonomials(Tuple<int, int> a, Tuple<int, int> b, int expectedComparsionResult)
        {
            Assert.Equal(Math.Sign(expectedComparsionResult), Math.Sign(_comparer.Compare(a, b)));
        }
    }
}