namespace GfPolynoms.Tests
{
    using GaloisFields;
    using Xunit;

    public class PolynomialUnderPrimePowerOrderFieldTest
    {
        private readonly PrimePowerOrderField _gf8;

        public PolynomialUnderPrimePowerOrderFieldTest()
        {
            _gf8 = new PrimePowerOrderField(8, 2, new[] {1, 1, 0, 1});
        }

        [Theory]
        [InlineData(new[] {2, 2, 2}, 0, 2)]
        [InlineData(new[] {2, 2, 2}, 1, 2)]
        [InlineData(new[] {2, 2, 2}, 2, 5)]
        [InlineData(new[] {2, 2, 2}, 3, 5)]
        [InlineData(new[] {2, 2, 2}, 4, 6)]
        [InlineData(new[] {2, 2, 2}, 5, 6)]
        [InlineData(new[] {2, 2, 2}, 6, 1)]
        [InlineData(new[] {2, 2, 2}, 7, 1)]
        public void ShouldEvaluateValue(int[] coefficients, int parameterValue, int expected)
        {
            // Given
            var a = new Polynomial(_gf8, coefficients);

            // When
            var actual = a.Evaluate(parameterValue);

            // Then
            Assert.Equal(expected, actual);
        }
    }
}