namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using System.Linq;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using Xunit;

    public class PolynomialExtensionsTests
    {
        private readonly GaloisField _field;

        public PolynomialExtensionsTests()
        {
            _field = GaloisField.Create(7);
        }

        [Theory]
        [InlineData(new[] {1, 2, 3, 4, 5, 6}, new[] {1, 3, 5}, new[] {2, 4, 6})]
        [InlineData(new int[0], new int[0], new int[0])]
        [InlineData(new[] {1}, new[] {1}, new int[0])]
        [InlineData(new[] {0, 1}, new int[0], new[] {1})]
        public void ShouldGetPolyhaseComponentsForPolynomial(int[] polynomialCoefficients, int[] expectedEvenComponentCoefficients, int[] expectedOddComponenetCoefficients)
        {
            // Given
            var polynomial = new Polynomial(_field, polynomialCoefficients);
            var expectedEvenComponent = new Polynomial(_field, expectedEvenComponentCoefficients);
            var expectedOddComponent = new Polynomial(_field, expectedOddComponenetCoefficients);

            // When
            var (actualEvenComponent, actualOddComponent) = polynomial.GetPolyphaseComponents();

            // Then
            Assert.Equal(expectedEvenComponent, actualEvenComponent);
            Assert.Equal(expectedOddComponent, actualOddComponent);
        }

        [Theory]
        [InlineData(new[] {1, 3, 5}, new[] {2, 4, 6}, new[] {1, 2, 3, 4, 5, 6})]
        [InlineData(new int[0], new int[0], new int[0])]
        [InlineData(new[] {1}, new int[0], new[] {1})]
        [InlineData(new int[0], new[] {1}, new[] {0, 1})]
        public void ShouldCreatePolynomialFromPolyhaseComponents(int[] evenComponentCoefficients, int[] oddComponenetCoefficients, int[] expectedPolynomialCoefficients)
        {
            // Given
            var evenComponent = new Polynomial(_field, evenComponentCoefficients);
            var oddComponent = new Polynomial(_field, oddComponenetCoefficients);
            var expectedPolynomial = new Polynomial(_field, expectedPolynomialCoefficients);

            // When
            var actualPolynomial = Extensions.PolynomialExtensions.CreateFormPolyphaseComponents(evenComponent, oddComponent);

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

        [Theory]
        [InlineData(5, new[] {1, 1, 1}, 3, new[] {3, 2, 1, 3})]
        [InlineData(16, new[] {3, 3, 4, 4, 11, 9, 2, 14, 4, 0, 2, 5}, 14, new[] {13, 13, 0, 5, 2, 7, 13, 8, 1, 14, 15, 15, 3, 3, 9})]
        public void MustGetPolynomialSpectrum(int fieldOrder, int[] coefficients, int expectedDegree, int[] expectedSpectrum)
        {
            // Given
            var field = GaloisField.Create(fieldOrder);
            var polynomial = new Polynomial(field, coefficients);

            // When
            var actualSpectrum = polynomial.GetSpectrum(expectedDegree);

            // Then
            Assert.Equal(expectedSpectrum.Select(x => field.CreateElement(x)).ToArray(), actualSpectrum);
        }

        [Theory]
        [InlineData(3, null, 3, new[] {1, 1})]
        [InlineData(3, new[] {1, 2, 2}, 3, null)]
        [InlineData(3, new[] {1, 2, 2}, 5, new[] {1, 1})]
        public void PerformVariableSubstitutionMustValidateParameters(
            int polynomialFieldOrder,
            int[] polynomialCoefficients,
            int substitutionFieldOrder,
            int[] substitutionCoefficients
        )
        {
            // Given
            var polynomialField = GaloisField.Create(polynomialFieldOrder);
            var substitutionField = GaloisField.Create(substitutionFieldOrder);
            var polynomial = polynomialCoefficients != null ? new Polynomial(polynomialField, polynomialCoefficients) : null;
            var substitution = substitutionCoefficients != null ? new Polynomial(substitutionField, substitutionCoefficients) : null;

            // When, Then
            Assert.ThrowsAny<ArgumentException>(() => polynomial.PerformVariableSubstitution(substitution));
        }

        [Theory]
        [InlineData(3, new[] {1, 2, 2}, new[] {1, 1}, new[] {2, 0, 2})]
        [InlineData(5, new[] {3, 4, 1}, new[] {1, 0, 2}, new[] {3, 0, 2, 0, 4})]
        public void MustPerformVariableSubstitution(
            int fieldOrder,
            int[] polynomialCoefficients,
            int[] substitutionCoefficients,
            int[] expectedCoefficients
        )
        {
            // Given
            var field = GaloisField.Create(fieldOrder);
            var polynomial = new Polynomial(field, polynomialCoefficients);
            var substitution = new Polynomial(field, substitutionCoefficients);

            // When
            var actual = polynomial.PerformVariableSubstitution(substitution);

            // Then
            var expected = new Polynomial(field, expectedCoefficients);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(3, null, 3, new[] {1, 1})]
        [InlineData(3, new[] {1, 2, 2}, 3, null)]
        [InlineData(3, new[] {1, 2, 2}, 5, new[] {1, 1})]
        [InlineData(3, new[] {1, 2, 2}, 3, new[] {1})]
        public void ChangeVariableSubstitutionMustValidateParameters(
            int polynomialFieldOrder,
            int[] polynomialCoefficients,
            int newVariableFieldOrder,
            int[] newVariableCoefficients
        )
        {
            // Given
            var polynomialField = GaloisField.Create(polynomialFieldOrder);
            var newVariableField = GaloisField.Create(newVariableFieldOrder);
            var polynomial = polynomialCoefficients != null ? new Polynomial(polynomialField, polynomialCoefficients) : null;
            var newVariable = newVariableCoefficients != null ? new Polynomial(newVariableField, newVariableCoefficients) : null;

            // When, Then
            Assert.ThrowsAny<ArgumentException>(() => polynomial.ChangeVariable(newVariable));
        }

        [Theory]
        [InlineData(3, new[] { 2, 0, 2 }, new[] { 1, 1 }, new[] { 1, 2, 2 })]
        [InlineData(5, new[] { 3, 0, 2, 0, 4 }, new[] { 1, 0, 2 }, new[] { 3, 4, 1 })]
        public void MustChangeVariable(
            int fieldOrder,
            int[] polynomialCoefficients,
            int[] newVariableCoefficients,
            int[] expectedCoefficients
        )
        {
            // Given
            var field = GaloisField.Create(fieldOrder);
            var polynomial = new Polynomial(field, polynomialCoefficients);
            var newVariable = new Polynomial(field, newVariableCoefficients);

            // When
            var actual = polynomial.ChangeVariable(newVariable);

            // Then
            var expected = new Polynomial(field, expectedCoefficients);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MustNotChangeVariable()
        {
            // Given
            var field = GaloisField.Create(5);
            var polynomial = new Polynomial(field, 3, 1, 2, 0, 4);
            var newVariable = new Polynomial(field, 1, 0, 2);

            // When, Then
            Assert.Throws<InvalidOperationException>(() => polynomial.ChangeVariable(newVariable));
        }
    }
}