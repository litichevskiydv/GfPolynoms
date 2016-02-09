namespace GfPolynoms.Tests
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PolynomialUnderPrimeOrderFieldTest
    {
        private PrimeOrderField _field;

        [SetUp]
        public void Init()
        {
            _field = new PrimeOrderField(2);
        }

        [Test]
        public void ShouldCreatePolynomFromArray1()
        {
            // Given
            var a = new Polynomial(_field, 0, 1, 1, 0);

            //Then
            Assert.AreEqual(2, a.Degree);
            Assert.AreEqual(0, a[0]);
            Assert.AreEqual(1, a[1]);
            Assert.AreEqual(1, a[2]);
        }

        [Test]
        public void ShouldCreatePolynomFromArray2()
        {
            // Given
            var a = new Polynomial(_field, 0, 0, 0, 0);

            //Then
            Assert.AreEqual(0, a.Degree);
        }

        [Test]
        public void ShouldAddTwoPolynoms1()
        {
            // Given
            var a = new Polynomial(_field, 1, 1);
            var b = new Polynomial(_field, 0, 1, 1);

            // When
            var c = a + b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 1, 0, 1), c);
        }

        [Test]
        public void ShouldAddTwoPolynoms2()
        {
            // Given
            var a = new Polynomial(_field, 1, 1, 1);
            var b = new Polynomial(_field, 1, 0, 1);

            // When
            var c = a + b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 0, 1), c);
        }

        [Test]
        public void ShouldSubtractTwoPolynoms1()
        {
            // Given
            var a = new Polynomial(_field, 0, 1, 1);
            var b = new Polynomial(_field, 1, 0, 1);

            // When
            var c = a - b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 1, 1), c);
        }

        [Test]
        public void ShouldSubtractTwoPolynoms2()
        {
            // Given
            var a = new Polynomial(_field, 0, 0, 1);
            var b = new Polynomial(_field, 1, 1);

            // When
            var c = a - b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 1, 1, 1), c);
        }

        [Test]
        public void ShouldSubtractTwoPolynoms3()
        {
            // Given
            var a = new Polynomial(_field, 1, 1);
            var b = new Polynomial(_field, 1, 1);

            // When
            var c = a - b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 0), c);
        }

        [Test]
        public void ShouldMultiplyTwoPolynoms1()
        {
            // Given
            var a = new Polynomial(_field, 1, 1);
            var b = new Polynomial(_field, 1, 1);

            // When
            var c = a * b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 1, 0, 1), c);
        }

        [Test]
        public void ShouldMultiplyTwoPolynoms2()
        {
            // Given
            var a = new Polynomial(_field, 0, 1);
            var b = new Polynomial(_field, 1, 1);

            // When
            var c = a * b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 0, 1, 1), c);
        }

        [Test]
        public void ShouldMultiplyTwoPolynoms3()
        {
            // Given
            var a = new Polynomial(_field, 0, 1, 1);
            var b = new Polynomial(_field, 1, 1);

            // When
            var c = a * b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 0, 1, 0, 1), c);
        }

        [Test]
        public void ShouldPerformRightShift()
        {
            // Given
            var a = new Polynomial(_field, 1, 1);

            // When
            var c = a >> 2;

            // Then
            Assert.AreEqual(new Polynomial(_field, 0, 0, 1, 1), c);
        }

        [Test]
        public void ShouldPerformEnlarge()
        {
            // Given
            var a = new Polynomial(_field, 1);

            // When
            a.RightShift(4);

            // Then
            Assert.AreEqual(new Polynomial(_field, 0, 0, 0, 0, 1), a);
        }

        [Test]
        public void ShouldCalculateModulo1()
        {
            // Given
            var a = new Polynomial(_field, 1);
            var b = new Polynomial(_field, 1, 1, 0, 1);

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(a, c);
        }

        [Test]
        public void ShouldCalculateModulo2()
        {
            // Given
            var a = new Polynomial(_field, 1).RightShift(1);
            var b = new Polynomial(_field, 1, 1, 0, 1);

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(a, c);
        }

        [Test]
        public void ShouldCalculateModulo3()
        {
            // Given
            var a = new Polynomial(_field, 1).RightShift(3);
            var b = new Polynomial(_field, 1, 1, 0, 1);

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 1, 1), c);
        }

        [Test]
        public void ShouldCalculateModulo4()
        {
            // Given
            var a = new Polynomial(_field, 1).RightShift(4);
            var b = new Polynomial(_field, 1, 1, 0, 1);

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 0, 1, 1), c);
        }

        [Test]
        public void ShouldCalculateModulo5()
        {
            // Given
            var a = new Polynomial(_field, 1).RightShift(5);
            var b = new Polynomial(_field, 1, 1, 0, 1);

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 1, 1, 1), c);
        }

        [Test]
        public void ShouldCalculateModulo6()
        {
            // Given
            var a = new Polynomial(_field, 1).RightShift(6);
            var b = new Polynomial(_field, 1, 1, 0, 1);

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 1, 0, 1), c);
        }

        [Test]
        public void ShouldCalculateModulo7()
        {
            // Given
            var a = new Polynomial(_field, 1).RightShift(7);
            var b = new Polynomial(_field, 1, 1, 0, 1);

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(new Polynomial(_field, 1), c);
        }

        [Test]
        public void ShouldCalculateModulo8()
        {
            // Given
            var field = new PrimeOrderField(3);
            var a = new Polynomial(field, 1).RightShift(3);
            var b = new Polynomial(field, 1, 1);

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(new Polynomial(field, 2), c);
        }

        [Test]
        public void ShouldRaiseVariableDegre1()
        {
            // Given
            var field = new PrimeOrderField(3);
            var a = new Polynomial(field, 1, 1, 1);

            // When
           a.RaiseVariableDegre(2);

            // Then
            Assert.AreEqual(new Polynomial(field, 1, 0, 1, 0, 1), a);
        }

        [Test]
        public void ShouldRaiseVariableDegre2()
        {
            // Given
            var field = new PrimeOrderField(3);
            var a = new Polynomial(field, 1, 1, 1);

            // When
            a.RaiseVariableDegre(1);

            // Then
            Assert.AreEqual(new Polynomial(field, 1, 1, 1), a);
        }

        [Test]
        public void ShouldEvaluateValue1()
        {
            // Given
            var field = new PrimeOrderField(3);
            var a = new Polynomial(field, 1, 2, 1);

            // When
            var value = a.Evaluate(2);

            // Then
            Assert.AreEqual(0, value);
        }

        [Test]
        public void ShouldEvaluateValue2()
        {
            // Given
            var field = new PrimeOrderField(3);
            var a = new Polynomial(field, 1, 2, 2);

            // When
            var value = a.Evaluate(2);

            // Then
            Assert.AreEqual(1, value);
        }
    }
}