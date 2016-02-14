namespace GfPolynoms.Tests.GaluaFields
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PrimePowerOrderFieldTests
    {
        private readonly PrimePowerOrderField _field;

        public PrimePowerOrderFieldTests()
        {
            _field = new PrimePowerOrderField(8, 2, new[] { 1, 1, 0, 1 });
        }

        [TestCase(2, Result = true)]
        [TestCase(8, Result = false)]
        public bool ShouldApproveFieldMember(int element)
        {
            return _field.IsFieldElement(element);
        }

        [TestCase(3, 4, Result = 6)]
        [TestCase(5, 6, Result = 1)]
        [TestCase(5, 5, Result = 0)]
        public int ShouldSumTwoFieldElements(int firstItem, int secondItem)
        {
            return _field.Add(firstItem, secondItem);
        }

        [TestCase(1, 4, Result = 2)]
        [TestCase(3, 5, Result = 2)]
        [TestCase(5, 7, Result = 4)]
        public int ShouldSubtractTwoFieldElements(int minuend, int subtrahend)
        {
            return _field.Subtract(minuend, subtrahend);
        }

        [TestCase(2, 4, Result = 5)]
        [TestCase(6, 7, Result = 5)]
        [TestCase(2, 7, Result = 1)]
        public int ShouldMultiplyTwoFieldElements(int firstMultiplied, int secondMultiplied)
        {
            return _field.Multiply(firstMultiplied, secondMultiplied);
        }

        [TestCase(0, 3)]
        [TestCase(4, 3)]
        [TestCase(7, 2)]
        [TestCase(2, 7)]
        public void ShouldDivideTwoFieldElements(int dividend, int divisor)
        {
            // When
            var c = _field.Divide(dividend, divisor);

            // Then
            Assert.AreEqual(_field[dividend], (_field[divisor]*_field[c])%_field.IrreduciblePolynomial);
        }
    }
}