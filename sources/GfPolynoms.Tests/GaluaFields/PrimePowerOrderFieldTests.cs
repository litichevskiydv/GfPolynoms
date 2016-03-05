namespace GfPolynoms.Tests.GaluaFields
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PrimePowerOrderFieldTests
    {
        private readonly PrimePowerOrderField _gf8;
        private readonly PrimePowerOrderField _gf27;

        public PrimePowerOrderFieldTests()
        {
            _gf8 = new PrimePowerOrderField(8, 2, new[] { 1, 1, 0, 1 });
            _gf27 = new PrimePowerOrderField(27, 3, new[] { 2, 2, 0, 1 });
        }

        [TestCase(2, Result = true)]
        [TestCase(8, Result = false)]
        public bool ShouldApproveFieldMemberUnderGf8(int element)
        {
            return _gf8.IsFieldElement(element);
        }

        [TestCase(3, 4, Result = 7)]
        [TestCase(1, 3, Result = 2)]
        [TestCase(1, 6, Result = 7)]
        [TestCase(5, 6, Result = 3)]
        [TestCase(4, 7, Result = 3)]
        [TestCase(5, 5, Result = 0)]
        public int ShouldSumTwoFieldElementsUnderGf8(int firstItem, int secondItem)
        {
            return _gf8.Add(firstItem, secondItem);
        }

        [TestCase(3, 4, Result = 7)]
        [TestCase(10, 12, Result = 22)]
        [TestCase(11, 10, Result = 18)]
        [TestCase(18, 9, Result = 0)]
        [TestCase(20, 17, Result = 7)]
        [TestCase(9, 21, Result = 3)]
        public int ShouldSumTwoFieldElementsUnderGf27(int firstItem, int secondItem)
        {
            return _gf27.Add(firstItem, secondItem);
        }

        [TestCase(3, 4, Result = 7)]
        [TestCase(1, 3, Result = 2)]
        [TestCase(1, 6, Result = 7)]
        [TestCase(5, 6, Result = 3)]
        [TestCase(4, 7, Result = 3)]
        [TestCase(5, 5, Result = 0)]
        public int ShouldSubtractTwoFieldElementsUnderGf8(int minuend, int subtrahend)
        {
            return _gf8.Subtract(minuend, subtrahend);
        }

        [TestCase(3, 4, Result = 2)]
        [TestCase(10, 12, Result = 7)]
        [TestCase(11, 14, Result = 6)]
        [TestCase(18, 9, Result = 9)]
        [TestCase(20, 17, Result = 12)]
        [TestCase(9, 21, Result = 24)]
        public int ShouldSubtractTwoFieldElementsUnderGf27(int minuend, int subtrahend)
        {
            return _gf27.Subtract(minuend, subtrahend);
        }

        [TestCase(0, 4, Result = 0)]
        [TestCase(2, 4, Result = 3)]
        [TestCase(3, 4, Result = 7)]
        [TestCase(2, 3, Result = 6)]
        [TestCase(6, 7, Result = 4)]
        [TestCase(2, 7, Result = 5)]
        public int ShouldMultiplyTwoFieldElementsUnderGf8(int firstMultiplied, int secondMultiplied)
        {
            return _gf8.Multiply(firstMultiplied, secondMultiplied);
        }

        [TestCase(0, 4, Result = 0)]
        [TestCase(12, 5, Result = 1)]
        [TestCase(3, 4, Result = 12)]
        [TestCase(21, 3, Result = 17)]
        [TestCase(20, 21, Result = 26)]
        [TestCase(2, 7, Result = 5)]
        public int ShouldMultiplyTwoFieldElementsUnderGf27(int firstMultiplied, int secondMultiplied)
        {
            return _gf27.Multiply(firstMultiplied, secondMultiplied);
        }

        [TestCase(0, 3, Result = 0)]
        [TestCase(4, 3, Result = 5)]
        [TestCase(7, 2, Result = 6)]
        [TestCase(2, 7, Result = 3)]
        public int ShouldDivideTwoFieldElementsUnderGf8(int dividend, int divisor)
        {
            return _gf8.Divide(dividend, divisor);
        }

        [TestCase(0, 3, Result = 0)]
        [TestCase(15, 7, Result = 6)]
        [TestCase(11, 26, Result = 14)]
        [TestCase(10, 17, Result = 15)]
        [TestCase(2, 23, Result = 25)]
        public int ShouldDivideTwoFieldElementsUnderGf27(int dividend, int divisor)
        {
            return _gf27.Divide(dividend, divisor);
        }

        [TestCase(0, Result = 0)]
        [TestCase(1, Result = 2)]
        [TestCase(2, Result = 1)]
        [TestCase(3, Result = 6)]
        [TestCase(4, Result = 8)]
        [TestCase(5, Result = 7)]
        [TestCase(6, Result = 3)]
        [TestCase(7, Result = 5)]
        [TestCase(8, Result = 4)]
        [TestCase(9, Result = 18)]
        [TestCase(10, Result = 20)]
        [TestCase(11, Result = 19)]
        [TestCase(12, Result = 24)]
        [TestCase(13, Result = 26)]
        [TestCase(14, Result = 25)]
        [TestCase(15, Result = 21)]
        [TestCase(16, Result = 23)]
        [TestCase(17, Result = 22)]
        [TestCase(18, Result = 9)]
        [TestCase(19, Result = 11)]
        [TestCase(20, Result = 10)]
        [TestCase(21, Result = 15)]
        [TestCase(22, Result = 17)]
        [TestCase(23, Result = 16)]
        [TestCase(24, Result = 12)]
        [TestCase(25, Result = 14)]
        [TestCase(26, Result = 13)]
        public int ShouldInverseElementForAdditionUnderGf27(int element)
        {
            return _gf27.InverseForAddition(element);
        }

        [TestCase(1, Result = 1)]
        [TestCase(2, Result = 2)]
        [TestCase(3, Result = 11)]
        [TestCase(4, Result = 15)]
        [TestCase(5, Result = 12)]
        [TestCase(6, Result = 19)]
        [TestCase(7, Result = 24)]
        [TestCase(8, Result = 21)]
        [TestCase(9, Result = 22)]
        [TestCase(10, Result = 26)]
        [TestCase(11, Result = 3)]
        [TestCase(12, Result = 5)]
        [TestCase(13, Result = 20)]
        [TestCase(14, Result = 23)]
        [TestCase(15, Result = 4)]
        [TestCase(16, Result = 25)]
        [TestCase(17, Result = 18)]
        [TestCase(18, Result = 17)]
        [TestCase(19, Result = 6)]
        [TestCase(20, Result = 13)]
        [TestCase(21, Result = 8)]
        [TestCase(22, Result = 9)]
        [TestCase(23, Result = 14)]
        [TestCase(24, Result = 7)]
        [TestCase(25, Result = 16)]
        [TestCase(26, Result = 10)]
        public int ShouldInverseForMultiplicationUnderGf27(int element)
        {
            return _gf27.InverseForMultiplication(element);
        }
    }
}