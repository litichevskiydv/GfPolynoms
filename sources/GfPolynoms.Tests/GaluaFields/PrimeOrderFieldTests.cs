namespace GfPolynoms.Tests.GaluaFields
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PrimeOrderFieldTests
    {
        [SetUp]
        public void Init()
        {
            _field = new PrimeOrderField(5);
        }

        private PrimeOrderField _field;

        [Test]
        public void ShouldAddTwoFieldElements()
        {
            // Given
            const int a = 3;
            const int b = 4;

            // When
            var c = _field.Add(a, b);

            // Then
            Assert.AreEqual(2, c);
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
        public void ShouldMultiplyTwoFieldElements()
        {
            // Given
            const int a = 2;
            const int b = 4;

            // When
            var c = _field.Multiply(a, b);

            // Then
            Assert.AreEqual(3, c);
        }

        [Test]
        public void ShouldNotApproveFieldMember()
        {
            // Given
            const int a = 6;

            // When
            var isFiledMember = _field.IsFieldElement(a);

            // Then
            Assert.AreEqual(false, isFiledMember);
        }

        [Test]
        public void ShouldSubtractTwoFieldElements()
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
        public void ShouldDivideTwoFieldElements()
        {
            // Given
            const int a = 4;
            const int b = 3;

            // When
            var c = _field.Divide(a, b);

            // Then
            Assert.AreEqual(3, c);
        }
    }
}