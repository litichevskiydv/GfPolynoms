namespace GfPolynoms.Tests
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PolynomUnderPrimeOrderFieldTest
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
            var a = new Polynom(_field, new[] { 0, 1, 1, 0 });

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
            var a = new Polynom(_field, new[] { 0, 0, 0, 0 });

            //Then
            Assert.AreEqual(0, a.Degree);
        }

        [Test]
        public void ShouldAddTwoPolynoms1()
        {
            // Given
            var a = new Polynom(_field, new[] {1, 1});
            var b = new Polynom(_field, new[] {0, 1, 1});

            // When
            var c = a + b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] {1, 0, 1}), c);
        }

        [Test]
        public void ShouldAddTwoPolynoms2()
        {
            // Given
            var a = new Polynom(_field, new[] { 1, 1, 1 });
            var b = new Polynom(_field, new[] { 1, 0, 1 });

            // When
            var c = a + b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 0, 1 }), c);
        }

        [Test]
        public void ShouldSubtractTwoPolynoms1()
        {
            // Given
            var a = new Polynom(_field, new[] { 0, 1, 1 });
            var b = new Polynom(_field, new[] { 1, 0, 1 });

            // When
            var c = a - b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 1, 1 }), c);
        }

        [Test]
        public void ShouldSubtractTwoPolynoms2()
        {
            // Given
            var a = new Polynom(_field, new[] { 0, 0, 1 });
            var b = new Polynom(_field, new[] { 1, 1 });

            // When
            var c = a - b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 1, 1, 1 }), c);
        }

        [Test]
        public void ShouldSubtractTwoPolynoms3()
        {
            // Given
            var a = new Polynom(_field, new[] { 1, 1 });
            var b = new Polynom(_field, new[] { 1, 1 });

            // When
            var c = a - b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 0 }), c);
        }

        [Test]
        public void ShouldMultiplyTwoPolynoms1()
        {
            // Given
            var a = new Polynom(_field, new[] { 1, 1 });
            var b = new Polynom(_field, new[] { 1, 1 });

            // When
            var c = a * b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 1, 0, 1 }), c);
        }

        [Test]
        public void ShouldMultiplyTwoPolynoms2()
        {
            // Given
            var a = new Polynom(_field, new[] { 0, 1 });
            var b = new Polynom(_field, new[] { 1, 1 });

            // When
            var c = a * b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 0, 1, 1 }), c);
        }

        [Test]
        public void ShouldMultiplyTwoPolynoms3()
        {
            // Given
            var a = new Polynom(_field, new[] {0, 1, 1});
            var b = new Polynom(_field, new[] { 1, 1 });

            // When
            var c = a * b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] {0, 1, 0, 1}), c);
        }

        [Test]
        public void ShouldPerformRightShift()
        {
            // Given
            var a = new Polynom(_field, new[] { 1, 1 });

            // When
            var c = a >> 2;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 0, 0, 1, 1 }), c);
        }

        [Test]
        public void ShouldPerformEnlarge()
        {
            // Given
            var a = new Polynom(_field);

            // When
            var c = a.Enlarge(4);
            c[4] = 1;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 0, 0, 0, 0, 1 }), c);
        }

        [Test]
        public void ShouldCalculateModulo1()
        {
            // Given
            var a = new Polynom(_field, new[] {1});
            var b = new Polynom(_field, new[] {1, 1, 0, 1});

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(a, c);
        }

        [Test]
        public void ShouldCalculateModulo2()
        {
            // Given
            var a = new Polynom(_field, new[] {1}).RightShift(1);
            var b = new Polynom(_field, new[] { 1, 1, 0, 1 });

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(a, c);
        }

        [Test]
        public void ShouldCalculateModulo3()
        {
            // Given
            var a = new Polynom(_field, new[] { 1 }).RightShift(3);
            var b = new Polynom(_field, new[] { 1, 1, 0, 1 });

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] {1, 1}), c);
        }

        [Test]
        public void ShouldCalculateModulo4()
        {
            // Given
            var a = new Polynom(_field, new[] { 1 }).RightShift(4);
            var b = new Polynom(_field, new[] { 1, 1, 0, 1 });

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 0, 1, 1 }), c);
        }

        [Test]
        public void ShouldCalculateModulo5()
        {
            // Given
            var a = new Polynom(_field, new[] { 1 }).RightShift(5);
            var b = new Polynom(_field, new[] { 1, 1, 0, 1 });

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 1, 1, 1 }), c);
        }

        [Test]
        public void ShouldCalculateModulo6()
        {
            // Given
            var a = new Polynom(_field, new[] { 1 }).RightShift(6);
            var b = new Polynom(_field, new[] { 1, 1, 0, 1 });

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 1, 0, 1 }), c);
        }

        [Test]
        public void ShouldCalculateModulo7()
        {
            // Given
            var a = new Polynom(_field, new[] { 1 }).RightShift(7);
            var b = new Polynom(_field, new[] { 1, 1, 0, 1 });

            // When
            var c = a % b;

            // Then
            Assert.AreEqual(new Polynom(_field, new[] { 1 }), c);
        }
    }
}