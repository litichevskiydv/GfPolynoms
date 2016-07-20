namespace GfAlgorithms.Tests
{
    using System.Collections.Generic;
    using CombinationsCountCalculator;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public class PascalsTriangleBasedCalcualtorTests
    {
        private static readonly PrimePowerOrderField Gf27;
        private readonly PascalsTriangleBasedCalcualtor _calcualtor;
        
        [UsedImplicitly]
        public static IEnumerable<object[]> CalculatorTestsData;

        static PascalsTriangleBasedCalcualtorTests()
        {
            Gf27 = new PrimePowerOrderField(27, 3, new[] { 2, 2, 0, 1 });

            CalculatorTestsData = new[]
                                  {
                                      new object[] {Gf27, 6, 3, new FieldElement(Gf27, 2)},
                                      new object[] {Gf27, 4, 2, Gf27.Zero()},
                                      new object[] {Gf27, 52, 5, Gf27.Zero()},
                                      new object[] {Gf27, 52, 5, Gf27.Zero()},
                                      new object[] {Gf27, 13, 3, Gf27.One()},
                                      new object[] {Gf27, 13, 10, Gf27.One()},
                                      new object[] {Gf27, 4, 3, Gf27.One() }
                                  };
        }

        public PascalsTriangleBasedCalcualtorTests()
        {
            _calcualtor = new PascalsTriangleBasedCalcualtor();
        }

        [Theory]
        [MemberData(nameof(CalculatorTestsData))]
        public void ShouldCalculateCombinationsCountWithoutCache(GaloisField field, int n, int k, FieldElement expectedResult)
        {
            Assert.Equal(expectedResult, _calcualtor.Calculate(field, n, k));
        }

        [Fact]
        public void ShouldFillCombinationsCacheDuringCalculations()
        {
            // Given
            const int n = 4;
            const int k = 3;
            var combinationsCache = new FieldElement[5][].MakeSquare();

            // When
            _calcualtor.Calculate(Gf27, n, k, combinationsCache);

            // Then
            Assert.Equal(Gf27.One(), combinationsCache[4][3]);
            Assert.Equal(Gf27.One(), combinationsCache[3][3]);
            Assert.Equal(Gf27.Zero(), combinationsCache[3][2]);
            Assert.Equal(Gf27.One(), combinationsCache[2][2]);
            Assert.Equal(new FieldElement(Gf27, 2), combinationsCache[2][1]);
            Assert.Equal(Gf27.One(), combinationsCache[1][1]);
            Assert.Equal(Gf27.One(), combinationsCache[1][0]);
        }
    }
}