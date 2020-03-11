namespace AppliedAlgebra.GfPolynoms.Tests.Polynomials
{
    using System.Linq;
    using Extensions;
    using GaloisFields;
    using Xunit;

    public class PolynomialExtensionsTests
    {
        [Fact]
        public void ShouldGetPolynomialSpectrum()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var polynomial = new Polynomial(gf5, 1, 1, 1);

            // When
            var actualSpectrum = polynomial.GetSpectrum();

            // Then
            var expectedSpectrum = new[] { 3, 2, 1, 3 }.Select(x => gf5.CreateElement(x)).ToArray();
            Assert.Equal(expectedSpectrum, actualSpectrum);
        }
    }
}