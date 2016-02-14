namespace GfPolynoms.Tests
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PolynomialUnderPrimePowerOrderFieldTest
    {
        private readonly PrimePowerOrderField _field;

        public PolynomialUnderPrimePowerOrderFieldTest()
        {
            _field = new PrimePowerOrderField(8, 2, new[] {1, 1, 0, 1});
        }

        [TestCase(new[] {2, 2, 2}, 1, Result = 2)]
        [TestCase(new[] {2, 2, 2}, 2, Result = 7)]
        [TestCase(new[] {2, 2, 2}, 3, Result = 5)]
        [TestCase(new[] {2, 2, 2}, 4, Result = 7)]
        [TestCase(new[] {2, 2, 2}, 5, Result = 1)]
        [TestCase(new[] {2, 2, 2}, 6, Result = 1)]
        [TestCase(new[] {2, 2, 2}, 7, Result = 5)]
        public int ShouldEvaluateValue(int[] coefficients, int parameterValue)
        {
            // Given
            var a = new Polynomial(_field, coefficients);

            // When
            var value = a.Evaluate(parameterValue);

            // Then
            return value;
        }
    }
}