namespace GfPolynoms.Tests.IrreduciblePolynomialsFinder
{
    using System.Linq;
    using GfPolynoms.IrreduciblePolynomialsFinder;
    using Xunit;

    public class SimpleFinderTest
    {
        [Theory]
        [InlineData(2, 2)]
        [InlineData(2, 3)]
        [InlineData(2, 15)]
        [InlineData(3, 2)]
        [InlineData(3, 3)]
        [InlineData(5, 7)]
        public void ShouldFindIrreduciblePolynomialsFinder(int fieldOrder, int degree)
        {
            // Given
            var finder = new SimpleFinder();

            // When
            var polynomial = finder.Find(fieldOrder, degree);

            // Then
            Assert.Equal(degree, polynomial.Degree);
            Assert.True(Enumerable.Range(0, fieldOrder).All(x => polynomial.Evaluate(x) != 0));
        }
    }
}