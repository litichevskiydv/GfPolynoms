namespace GfPolynoms.Tests.IrreduciblePolynomialsFinder
{
    using System;
    using GaloisFields;
    using GfPolynoms.IrreduciblePolynomialsFinder;
    using Xunit;

    public class SimpleFinderTest
    {
        [Theory]
        [InlineData(2, 2)]
        [InlineData(2, 3)]
        [InlineData(3, 2)]
        [InlineData(3, 3)]
        [InlineData(3, 4)]
        public void ShouldFindIrreduciblePolynomialsFinder(int fieldOrder, int degree)
        {
            // Given
            var finder = new SimpleFinder();

            // When
            var polynomial = finder.Find(fieldOrder, degree);

            // Then
            Assert.Equal(degree, polynomial.Degree);
            Assert.NotNull(new PrimePowerOrderField((int)Math.Pow(fieldOrder, degree), polynomial));
        }
    }
}