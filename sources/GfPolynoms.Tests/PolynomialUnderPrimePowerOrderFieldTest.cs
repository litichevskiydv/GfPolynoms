namespace GfPolynoms.Tests
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PolynomialUnderPrimePowerOrderFieldTest
    {
        private readonly PrimePowerOrderField _gf8;

        public PolynomialUnderPrimePowerOrderFieldTest()
        {
            _gf8 = new PrimePowerOrderField(8, 2, new[] {1, 1, 0, 1});
        }

        [TestCase(new[] {2, 2, 2}, 0, Result = 2)]
        [TestCase(new[] {2, 2, 2}, 1, Result = 2)]
        [TestCase(new[] {2, 2, 2}, 2, Result = 5)]
        [TestCase(new[] {2, 2, 2}, 3, Result = 5)]
        [TestCase(new[] {2, 2, 2}, 4, Result = 6)]
        [TestCase(new[] {2, 2, 2}, 5, Result = 6)]
        [TestCase(new[] {2, 2, 2}, 6, Result = 1)]
        [TestCase(new[] {2, 2, 2}, 7, Result = 1)]
        public int ShouldEvaluateValue(int[] coefficients, int parameterValue)
        {
            // Given
            var a = new Polynomial(_gf8, coefficients);

            // When
            var value = a.Evaluate(parameterValue);

            // Then
            return value;
        }
    }
}