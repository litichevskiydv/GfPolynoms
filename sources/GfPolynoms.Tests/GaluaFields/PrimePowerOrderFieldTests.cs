namespace GfPolynoms.Tests.GaluaFields
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PrimePowerOrderFieldTests
    {
        private PrimePowerOrderField _field;

        [SetUp]
        public void Init()
        {
            _field = new PrimePowerOrderField(8, 2, new[] { 1, 1, 0, 1 });
        }

        [Test]
        public void ShouldApproveFieldMember()
        {
            // Given
            const int a = 2;

            // When
            var isFiledMember = _field.IsFieldElement(a);

            // Then
            Assert.AreEqual(true, isFiledMember);
        }

        [Test]
        public void ShouldNotApproveFieldMember()
        {
            // Given
            const int a = 8;

            // When
            var isFiledMember = _field.IsFieldElement(a);

            // Then
            Assert.AreEqual(false, isFiledMember);
        }

        [Test]
        public void ShouldAddTwoFieldElements1()
        {
            // Given
            const int a = 3;
            const int b = 4;

            // When
            var c = _field.Add(a, b);

            // Then
            Assert.AreEqual(6, c);
        }

        [Test]
        public void ShouldAddTwoFieldElements2()
        {
            // Given
            const int a = 5;
            const int b = 6;

            // When
            var c = _field.Add(a, b);

            // Then
            Assert.AreEqual(1, c);
        }

        [Test]
        public void ShouldAddTwoFieldElements3()
        {
            // Given
            const int a = 5;
            const int b = 5;

            // When
            var c = _field.Add(a, b);

            // Then
            Assert.AreEqual(0, c);
        }

        [Test]
        public void ShouldSubtractTwoFieldElements1()
        {
            // Given
            const int a = 1;
            const int b = 4;

            // When
            var c = _field.Subtract(a, b);

            // Then
            Assert.AreEqual(2, c);
        }

        [Test]
        public void ShouldSubtractTwoFieldElements2()
        {
            // Given
            const int a = 3;
            const int b = 5;

            // When
            var c = _field.Subtract(a, b);

            // Then
            Assert.AreEqual(2, c);
        }

        [Test]
        public void ShouldSubtractTwoFieldElements3()
        {
            // Given
            const int a = 5;
            const int b = 7;

            // When
            var c = _field.Subtract(a, b);

            // Then
            Assert.AreEqual(4, c);
        }

        [Test]
        public void ShouldMultiplyTwoFieldElements1()
        {
            // Given
            const int a = 2;
            const int b = 4;

            // When
            var c = _field.Multiply(a, b);

            // Then
            Assert.AreEqual(5, c);
        }

        [Test]
        public void ShouldMultiplyTwoFieldElements2()
        {
            // Given
            const int a = 6;
            const int b = 7;

            // When
            var c = _field.Multiply(a, b);

            // Then
            Assert.AreEqual(5, c);
        }

        [Test]
        public void ShouldMultiplyTwoFieldElements3()
        {
            // Given
            const int a = 2;
            const int b = 7;

            // When
            var c = _field.Multiply(a, b);

            // Then
            Assert.AreEqual(1, c);
        }

        [Test]
        public void ShouldDivideZero()
        {
            // Given
            const int a = 0;
            const int b = 3;

            // When
            var c = _field.Divide(a, b);

            // Then
            Assert.AreEqual(0, c);
        }

        [Test]
        public void ShouldDivideTwoFieldElements1()
        {
            // Given
            const int a = 4;
            const int b = 3;

            // When
            var c = _field.Divide(a, b);

            // Then
            Assert.AreEqual(_field[a], (_field[b]*_field[c])%_field.IrreduciblePolynomial);
        }

        [Test]
        public void ShouldDivideTwoFieldElements2()
        {
            // Given
            const int a = 7;
            const int b = 2;

            // When
            var c = _field.Divide(a, b);

            // Then
            Assert.AreEqual(_field[a], (_field[b] * _field[c]) % _field.IrreduciblePolynomial);
        }

        [Test]
        public void ShouldDivideTwoFieldElements3()
        {
            // Given
            const int a = 2;
            const int b = 7;

            // When
            var c = _field.Divide(a, b);

            // Then
            Assert.AreEqual(_field[a], (_field[b] * _field[c]) % _field.IrreduciblePolynomial);
        }
    }
}