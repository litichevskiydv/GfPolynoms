namespace GfPolynoms.Tests.GaluaFields
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PrimeOrderFieldTests
    {
        private readonly PrimeOrderField _gf5;

        public PrimeOrderFieldTests()
        {
            _gf5 = new PrimeOrderField(5);
        }

        [TestCase(2, Result = true)]
        [TestCase(6, Result = false)]
        public bool ShouldApproveFieldMember(int element)
        {
            return _gf5.IsFieldElement(element);
        }

        [TestCase(3, 4, Result = 2)]
        [TestCase(1, 2, Result = 3)]
        [TestCase(3, 3, Result = 1)]
        [TestCase(3, 2, Result = 0)]
        public int ShouldSumTwoFieldElements(int firstItem, int secondItem)
        {
            return _gf5.Add(firstItem, secondItem);
        }

        [TestCase(3, 2, Result = 1)]
        [TestCase(4, 1, Result = 3)]
        [TestCase(1, 4, Result = 2)]
        [TestCase(2, 3, Result = 4)]
        [TestCase(0, 1, Result = 4)]
        public int ShouldSubtractTwoFieldElements(int minuend, int subtrahend)
        {
            return _gf5.Subtract(minuend, subtrahend);
        }

        [TestCase(0, 4, Result = 0)]
        [TestCase(4, 0, Result = 0)]
        [TestCase(2, 4, Result = 3)]
        [TestCase(3, 3, Result = 4)]
        [TestCase(2, 3, Result = 1)]
        [TestCase(4, 1, Result = 4)]
        public int ShouldMultiplyTwoFieldElements(int firstMultiplied, int secondMultiplied)
        {
            return _gf5.Multiply(firstMultiplied, secondMultiplied);
        }

        [TestCase(0, 3, Result = 0)]
        [TestCase(4, 3, Result = 3)]
        [TestCase(3, 4, Result = 2)]
        [TestCase(2, 3, Result = 4)]
        [TestCase(3, 2, Result = 4)]
        public int ShouldDivideTwoFieldElements(int dividend, int divisor)
        {
            return _gf5.Divide(dividend, divisor);
        }

        [TestCase(0, Result = 0)]
        [TestCase(1, Result = 4)]
        [TestCase(2, Result = 3)]
        [TestCase(3, Result = 2)]
        [TestCase(4, Result = 1)]
        public int ShouldInverseElementForAddition(int element)
        {
            return _gf5.InverseForAddition(element);
        }

        [TestCase(1, Result = 1)]
        [TestCase(2, Result = 3)]
        [TestCase(3, Result = 2)]
        [TestCase(4, Result = 4)]
        public int ShouldInverseElementForMultiplication(int element)
        {
            return _gf5.InverseForMultiplication(element);
        }
    }
}