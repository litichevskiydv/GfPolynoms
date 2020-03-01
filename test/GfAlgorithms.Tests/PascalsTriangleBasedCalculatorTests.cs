namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using CombinationsCountCalculator;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases;
    using Xunit;

    public class PascalsTriangleBasedCalculatorTests
    {
        private static readonly GaloisField Gf27;
        private readonly PascalsTriangleBasedCalculator _calculator;
        
        [UsedImplicitly]
        public static TheoryData<CombinationsCountCalculatorTestCase> CalculatorTestsData;

        static PascalsTriangleBasedCalculatorTests()
        {
            Gf27 = GaloisField.Create(27, new[] {2, 2, 0, 1});

            CalculatorTestsData
                = new TheoryData<CombinationsCountCalculatorTestCase>
                  {
                      new CombinationsCountCalculatorTestCase {Field = Gf27, N = 6, K = 3, Expected = Gf27.CreateElement(2)},
                      new CombinationsCountCalculatorTestCase {Field = Gf27, N = 4, K = 2, Expected = Gf27.Zero()},
                      new CombinationsCountCalculatorTestCase {Field = Gf27, N = 52, K = 5, Expected = Gf27.Zero()},
                      new CombinationsCountCalculatorTestCase {Field = Gf27, N = 52, K = 5, Expected = Gf27.Zero()},
                      new CombinationsCountCalculatorTestCase {Field = Gf27, N = 13, K = 3, Expected = Gf27.One()},
                      new CombinationsCountCalculatorTestCase {Field = Gf27, N = 13, K = 10, Expected = Gf27.One()},
                      new CombinationsCountCalculatorTestCase {Field = Gf27, N = 4, K = 3, Expected = Gf27.One()}
                  };
        }

        public PascalsTriangleBasedCalculatorTests()
        {
            _calculator = new PascalsTriangleBasedCalculator();
        }

        [Theory]
        [MemberData(nameof(CalculatorTestsData))]
        public void ShouldCalculateCombinationsCountWithoutCache(CombinationsCountCalculatorTestCase testCase)
        {
            Assert.Equal(testCase.Expected, _calculator.Calculate(testCase.Field, testCase.N, testCase.K));
        }

        [Fact]
        public void ShouldFillCombinationsCacheDuringCalculations()
        {
            // Given
            const int n = 4;
            const int k = 3;
            var combinationsCache = new FieldElement[5][].MakeSquare();

            // When
            _calculator.Calculate(Gf27, n, k, combinationsCache);

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