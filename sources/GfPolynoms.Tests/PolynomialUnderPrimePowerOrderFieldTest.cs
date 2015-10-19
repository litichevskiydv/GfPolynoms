namespace GfPolynoms.Tests
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PolynomialUnderPrimePowerOrderFieldTest
    {
        private PrimePowerOrderField _field;

        [SetUp]
        public void Init()
        {
            _field = new PrimePowerOrderField(8, 2, new[] {1, 1, 0, 1});
        }

        [Test]
        public void ShouldEvaluateValue1()
        {
            // Given
            var a = new Polynomial(_field, new[] { 2, 2, 2 });

            // When
            var value = a.Evaluate(1);

            // Then
            Assert.AreEqual(2, value);
        }

        [Test]
        public void ShouldEvaluateValue2()
        {
            // Given
            var a = new Polynomial(_field, new[] { 2, 2, 2 });

            // When
            var value = a.Evaluate(2);

            // Then
            Assert.AreEqual(7, value);
        }

        [Test]
        public void ShouldEvaluateValue3()
        {
            // Given
            var a = new Polynomial(_field, new[] { 2, 2, 2 });

            // When
            var value = a.Evaluate(3);

            // Then
            Assert.AreEqual(5, value);
        }

        [Test]
        public void ShouldEvaluateValue4()
        {
            // Given
            var a = new Polynomial(_field, new[] { 2, 2, 2 });

            // When
            var value = a.Evaluate(4);

            // Then
            Assert.AreEqual(7, value);
        }

        [Test]
        public void ShouldEvaluateValue5()
        {
            // Given
            var a = new Polynomial(_field, new[] { 2, 2, 2 });

            // When
            var value = a.Evaluate(5);

            // Then
            Assert.AreEqual(1, value);
        }

        [Test]
        public void ShouldEvaluateValue6()
        {
            // Given
            var a = new Polynomial(_field, new[] { 2, 2, 2 });

            // When
            var value = a.Evaluate(6);

            // Then
            Assert.AreEqual(1, value);
        }

        [Test]
        public void ShouldEvaluateValue7()
        {
            // Given
            var a = new Polynomial(_field, new[] { 2, 2, 2 });

            // When
            var value = a.Evaluate(7);

            // Then
            Assert.AreEqual(5, value);
        }
    }
}