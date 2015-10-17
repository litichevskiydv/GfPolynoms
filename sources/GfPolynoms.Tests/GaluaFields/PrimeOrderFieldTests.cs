using GfPolynoms.GaluaFields;
using NUnit.Framework;

namespace GfPolynoms.Tests.GaluaFields
{
    [TestFixture]
    public class PrimeOrderFieldTests
    {
        private PrimeOrderField _field;

        [SetUp]
        public void Init()
        {
            _field = new PrimeOrderField(5);
        }

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
            const int a = 6;

            // When
            var isFiledMember = _field.IsFieldElement(a);

            // Then
            Assert.AreEqual(false, isFiledMember);
        }
    }
}