namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using Xunit;

    public class PolynomialExtensionsTests
    {
        private readonly GaloisField _field;

        public PolynomialExtensionsTests()
        {
            _field = new PrimeOrderField(7);
        }

        [Theory]
        [InlineData(new[] {1, 2, 3, 4, 5, 6}, new[] {1, 3, 5}, new[] {2, 4, 6})]
        [InlineData(new int[0], new int[0], new int[0])]
        [InlineData(new[] {1}, new[] {1}, new int[0])]
        [InlineData(new[] {0, 1}, new int[0], new[] {1})]
        public void ShouldGetPolyhaseComponentsForPolynomial(int[] polynomialCoefficients,
            int[] expectedEvenComponentCoefficients, int[] expectedOddComponenetCoefficients)
        {
            // Given
            var polynomial = new Polynomial(_field, polynomialCoefficients);
            var expectedEvenComponent = new Polynomial(_field, expectedEvenComponentCoefficients);
            var expectedOddComponent = new Polynomial(_field, expectedOddComponenetCoefficients);

            // When
            var polyphaseComponents = polynomial.GetPolyphaseComponents();

            // Then
            Assert.Equal(expectedEvenComponent, polyphaseComponents.Item1);
            Assert.Equal(expectedOddComponent, polyphaseComponents.Item2);
        }

        [Theory]
        [InlineData(new[] {1, 3, 5}, new[] {2, 4, 6}, new[] {1, 2, 3, 4, 5, 6})]
        [InlineData(new int[0], new int[0], new int[0])]
        [InlineData(new[] {1}, new int[0], new[] {1})]
        [InlineData(new int[0], new[] {1}, new[] {0, 1})]
        public void ShouldCreatePolynomialFromPolyhaseComponents(int[] evenComponentCoefficients, int[] oddComponenetCoefficients, 
            int[] expectedPolynomialCoefficients)
        {
            // Given
            var evenComponent = new Polynomial(_field, evenComponentCoefficients);
            var oddComponent = new Polynomial(_field, oddComponenetCoefficients);
            var expectedPolynomial = new Polynomial(_field, expectedPolynomialCoefficients);

            // When
            var actualPolynomial = PolynomialsAlgorithmsExtensions.CreateFormPolyphaseComponents(evenComponent, oddComponent);

            // Then
            Assert.Equal(expectedPolynomial, actualPolynomial);
        }

        [Theory]
        [InlineData(new[] {1, 2, 3}, false)]
        [InlineData(new[] {0, 0, 1}, true)]
        [InlineData(new[] {1}, true)]
        [InlineData(new int[0], true)]
        public void ShouldDetermineWhatPolynomialHasOnlyOneNotZeroCoefficient(int[] coefficients, bool expected)
        {
            // Given
            var polynomial = new Polynomial(_field, coefficients);

            // When
            var isMonomial = polynomial.IsMonomial();

            // Then
            Assert.Equal(expected, isMonomial);
        }
    }
}