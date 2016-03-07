namespace GfPolynoms.Tests.GaluaFields
{
    using GfPolynoms.GaluaFields;
    using Xunit;

    public class PrimeOrderFieldTests
    {
        private readonly PrimeOrderField _gf5;

        public PrimeOrderFieldTests()
        {
            _gf5 = new PrimeOrderField(5);
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(6, false)]
        public void ShouldApproveFieldMember(int element, bool expected)
        {
            // When
            var actual = _gf5.IsFieldElement(element);

            // Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(3, 4, 2)]
        [InlineData(1, 2, 3)]
        [InlineData(3, 3, 1)]
        [InlineData(3, 2, 0)]
        public void ShouldSumTwoFieldElements(int firstItem, int secondItem, int expected)
        {
            // When
            var actual = _gf5.Add(firstItem, secondItem);

            // Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(3, 2, 1)]
        [InlineData(4, 1, 3)]
        [InlineData(1, 4, 2)]
        [InlineData(2, 3, 4)]
        [InlineData(0, 1, 4)]
        public void ShouldSubtractTwoFieldElements(int minuend, int subtrahend, int expected)
        {
            // When
            var actual = _gf5.Subtract(minuend, subtrahend);

            // Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 4, 0)]
        [InlineData(4, 0, 0)]
        [InlineData(2, 4, 3)]
        [InlineData(3, 3, 4)]
        [InlineData(2, 3, 1)]
        [InlineData(4, 1, 4)]
        public void ShouldMultiplyTwoFieldElements(int firstMultiplied, int secondMultiplied, int expected)
        {
            // When
            var actual = _gf5.Multiply(firstMultiplied, secondMultiplied);

            // Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 3, 0)]
        [InlineData(4, 3, 3)]
        [InlineData(3, 4, 2)]
        [InlineData(2, 3, 4)]
        [InlineData(3, 2, 4)]
        public void ShouldDivideTwoFieldElements(int dividend, int divisor, int expected)
        {
            // When
            var actual = _gf5.Divide(dividend, divisor);

            // Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 4)]
        [InlineData(2, 3)]
        [InlineData(3, 2)]
        [InlineData(4, 1)]
        public void ShouldInverseElementForAddition(int element, int expected)
        {
            // When
            var actual = _gf5.InverseForAddition(element);

            // Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 3)]
        [InlineData(3, 2)]
        [InlineData(4, 4)]
        public void ShouldInverseElementForMultiplication(int element, int expected)
        {
            // When
            var actual = _gf5.InverseForMultiplication(element);

            // Then
            Assert.Equal(expected, actual);
        }
    }
}