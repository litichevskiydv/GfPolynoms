namespace GfPolynoms.Tests.GaluaFields
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PrimeOrderFieldTests
    {
        private readonly PrimeOrderField _field;

        public PrimeOrderFieldTests()
        {
            _field = new PrimeOrderField(5);
        }

        [TestCase(2, Result = true)]
        [TestCase(6, Result = false)]
        public bool ShouldApproveFieldMember(int element)
        {
            return _field.IsFieldElement(element);
        }

        [TestCase(3, 4, Result = 2)]
        public int ShouldSumTwoFieldElements(int firstItem, int secondItem)
        {
            return _field.Add(firstItem, secondItem);
        }

        [TestCase(1, 4, Result = 2)]
        public int ShouldSubtractTwoFieldElements(int minuend, int subtrahend)
        {
            return _field.Subtract(minuend, subtrahend);
        }

        [TestCase(2, 4, Result = 3)]
        public int ShouldMultiplyTwoFieldElements(int firstMultiplied, int secondMultiplied)
        {
            return _field.Multiply(firstMultiplied, secondMultiplied);
        }

        [TestCase(0, 3, Result = 0)]
        [TestCase(4, 3, Result = 3)]
        public int ShouldDivideTwoFieldElements(int dividend, int divisor)
        {
            return _field.Divide(dividend, divisor);
        }
    }
}