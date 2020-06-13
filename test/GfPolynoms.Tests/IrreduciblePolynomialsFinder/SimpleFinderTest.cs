namespace AppliedAlgebra.GfPolynoms.Tests.IrreduciblePolynomialsFinder
{
    using System;
    using System.Linq;
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
        [InlineData(5, 3)]
        public void ShouldFindIrreduciblePolynomials(int fieldOrder, int degree)
        {
            // Given
            var finder = new SimpleFinder();

            // When
            var irreduciblePolynomials = finder.Find(GaloisField.Create(fieldOrder), degree);

            // Then
            Assert.All(irreduciblePolynomials,
                polynomial =>
                {
                    Assert.Equal(degree, polynomial.Degree);
                    Assert.NotNull(
                        GaloisField.Create(
                            (int) Math.Pow(fieldOrder, degree),
                            Enumerable.Range(0, polynomial.Degree + 1).Select(x => polynomial[x]).ToArray()
                        )
                    );
                }
            );
        }
    }
}