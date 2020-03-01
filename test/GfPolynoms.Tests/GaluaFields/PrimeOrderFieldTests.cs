namespace AppliedAlgebra.GfPolynoms.Tests.GaluaFields
{
    using System;
    using GaloisFields;
    using Xunit;

    public class PrimeOrderFieldTests
    {
        private readonly PrimeOrderField _gf5;

        public PrimeOrderFieldTests()
        {
            _gf5 = (PrimeOrderField)GaloisField.Create(5);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(6)]
        public void ShouldNotCreateFieldWithNotPrimeOrder(int fieldOrder)
        {
            Assert.Throws<ArgumentException>(() => GaloisField.Create(fieldOrder));
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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(63)]
        [InlineData(-63)]
        public void ShouldGetGeneratingElementPowerGet(int power)
        {
            // When
            var element = _gf5.GetGeneratingElementPower(power);
            var invertedElement = _gf5.GetGeneratingElementPower(-power);

            // Then
            Assert.Equal(1, (element*invertedElement)%_gf5.Order);
        }

        [Theory]
        [InlineData(0, 1, 0)]
        [InlineData(0, 0, 1)]
        [InlineData(1, 15, 1)]
        [InlineData(1, -2, 1)]
        [InlineData(3, 3, 2)]
        [InlineData(3, -3, 3)]
        public void ShouldPowerElementToSpecifiedDegree(int element, int power, int expected)
        {
            // When
            var actual = _gf5.Pow(element, power);

            // Then
            Assert.Equal(expected, actual);
        }
    }
}