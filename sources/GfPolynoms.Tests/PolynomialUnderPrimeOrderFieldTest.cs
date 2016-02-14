namespace GfPolynoms.Tests
{
    using GfPolynoms.GaluaFields;
    using NUnit.Framework;

    [TestFixture]
    public class PolynomialUnderPrimeOrderFieldTest
    {
        private readonly PrimeOrderField _fieldOrder2;
        private readonly PrimeOrderField _fieldOrder3;

        public PolynomialUnderPrimeOrderFieldTest()
        {
            _fieldOrder2 = new PrimeOrderField(2);
            _fieldOrder3 = new PrimeOrderField(3);
        }

        [Test]
        public void ShouldCreatePolynomFromArray1()
        {
            // Given
            var a = new Polynomial(_fieldOrder2, 0, 1, 1, 0);

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
            var a = new Polynomial(_fieldOrder2, 0, 0, 0, 0);

            //Then
            Assert.AreEqual(0, a.Degree);
        }

        [TestCase(new[] {1, 1}, new[] {0, 1, 1}, new[] {1, 0, 1})]
        [TestCase(new[] {1, 1, 1}, new[] {1, 0, 1}, new[] {0, 1})]
        public void ShouldSumTwoPolynomsUnderFieldOrder2(int[] firstItemCoefficients, int[] secondItemCoefficients, int[] sumCoefficients)
        {
            // Given
            var a = new Polynomial(_fieldOrder2, firstItemCoefficients);
            var b = new Polynomial(_fieldOrder2, secondItemCoefficients);

            // When
            var c = a + b;

            // Then
            Assert.AreEqual(new Polynomial(_fieldOrder2, sumCoefficients), c);
        }

        [TestCase(new[] {0, 1, 1}, new[] {1, 0, 1}, new[] {1, 1})]
        [TestCase(new[] {0, 0, 1}, new[] {1, 1}, new[] {1, 1, 1})]
        [TestCase(new[] {1, 1}, new[] {1, 1}, new[] {0})]
        public void ShouldSubtractTwoPolynomsUnderFieldOrder2(int[] minuendCoefficients, int[] subtrahendCoefficients, int[] differenceCoefficients)
        {
            // Given
            var a = new Polynomial(_fieldOrder2, minuendCoefficients);
            var b = new Polynomial(_fieldOrder2, subtrahendCoefficients);

            // When
            var c = a - b;

            // Then
            Assert.AreEqual(new Polynomial(_fieldOrder2, differenceCoefficients), c);
        }

        [TestCase(new[] {1, 1}, new[] {1, 1}, new[] {1, 0, 1})]
        [TestCase(new[] {0, 1}, new[] {1, 1}, new[] {0, 1, 1})]
        [TestCase(new[] {0, 1, 1}, new[] {1, 1}, new[] {0, 1, 0, 1})]
        public void ShouldMultiplyTwoPolynomsUnderFieldOrder2(int[] firstMultipliedCoefficients, int[] secondMultipliedCoefficients,
            int[] productCoefficients)
        {
            // Given
            var a = new Polynomial(_fieldOrder2, firstMultipliedCoefficients);
            var b = new Polynomial(_fieldOrder2, secondMultipliedCoefficients);

            // When
            var c = a*b;

            // Then
            Assert.AreEqual(new Polynomial(_fieldOrder2, productCoefficients), c);
        }

        [TestCase(new[] {1}, new[] {1, 1, 0, 1}, new[] {1})]
        [TestCase(new[] {0, 1}, new[] {1, 1, 0, 1}, new[] {0, 1})]
        [TestCase(new[] {0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1, 1})]
        [TestCase(new[] {0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {0, 1, 1})]
        [TestCase(new[] {0, 0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1, 1, 1})]
        [TestCase(new[] {0, 0, 0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1, 0, 1})]
        [TestCase(new[] {0, 0, 0, 0, 0, 0, 0, 1}, new[] {1, 1, 0, 1}, new[] {1})]
        public void ShouldCalculateModuloUnderFieldOrder2(int[] dividendCoefficients, int[] divisorCoefficients, int[] remainderCoefficients)
        {
            // Given
            var a = new Polynomial(_fieldOrder2, dividendCoefficients);
            var b = new Polynomial(_fieldOrder2, divisorCoefficients);

            // When
            var c = a%b;

            // Then
            Assert.AreEqual(new Polynomial(_fieldOrder2, remainderCoefficients), c);
        }

        [TestCase(new[] {0, 0, 0, 1}, new[] {1, 1}, new[] {2})]
        public void ShouldCalculateModuloUnderFieldOrder3(int[] dividendCoefficients, int[] divisorCoefficients, int[] remainderCoefficients)
        {
            // Given
            var a = new Polynomial(_fieldOrder3, dividendCoefficients);
            var b = new Polynomial(_fieldOrder3, divisorCoefficients);

            // When
            var c = a%b;

            // Then
            Assert.AreEqual(new Polynomial(_fieldOrder3, remainderCoefficients), c);
        }


        [TestCase(new[] {1, 1}, 2, new[] {0, 0, 1, 1})]
        [TestCase(new[] {1}, 4, new[] {0, 0, 0, 0, 1})]
        public void ShouldPerformRightShift(int[] initialCoefficients, int shift, int[] resultCoefficients)
        {
            // Given
            var a = new Polynomial(_fieldOrder2, initialCoefficients);

            // When
            var c = a >> shift;

            // Then
            Assert.AreEqual(new Polynomial(_fieldOrder2, resultCoefficients), c);
        }

        [TestCase(new[] {1, 1, 1}, 2, new[] {1, 0, 1, 0, 1})]
        [TestCase(new[] {1, 1, 1}, 1, new[] { 1, 1, 1 })]
        public void ShouldRaiseVariableDegre(int[] initialCoefficients, int newDegree, int[] resultCoefficients)
        {
            // Given
            var a = new Polynomial(_fieldOrder3, initialCoefficients);

            // When
            a.RaiseVariableDegree(newDegree);

            // Then
            Assert.AreEqual(new Polynomial(_fieldOrder3, resultCoefficients), a);
        }

        [TestCase(new[] {1, 2, 1}, 2, Result = 0)]
        [TestCase(new[] { 1, 2, 2 }, 2, Result = 1)]
        public int ShouldEvaluateValueUnderFieldOrder3(int[] coefficients, int parameterValue)
        {
            // Given
            var a = new Polynomial(_fieldOrder3, coefficients);

            // When
            var value = a.Evaluate(parameterValue);

            // Then
            return value;
        }
    }
}