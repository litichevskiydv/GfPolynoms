namespace GfPolynoms.Tests.GaluaFields
{
    using System;
    using System.Collections.Generic;
    using GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public class PrimePowerOrderFieldTests
    {
        private static readonly PrimePowerOrderField Gf8;
        private static readonly PrimePowerOrderField Gf27;

        [UsedImplicitly]
        public static readonly IEnumerable<object[]> IncorrectFieldCreationTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> SumTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> SubtractTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> MultiplyTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> DivideTestsData;

        static PrimePowerOrderFieldTests()
        {
            Gf8 = new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1));
            Gf27 = new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1));

            IncorrectFieldCreationTestsData = new[]
                                              {
                                                  new object[] {8, null},
                                                  new object[] {8, new Polynomial(new PrimeOrderField(3), 1)},
                                                  new object[] {8, new Polynomial(new PrimeOrderField(2), 1)},
                                                  new object[] {8, new Polynomial(new PrimeOrderField(2), 0, 0, 0, 1)}
                                              };
            SumTestsData = new[]
                            {
                                new object[] {Gf8, 3, 4, 7},
                                new object[] {Gf8, 1, 3, 2},
                                new object[] {Gf8, 1, 6, 7},
                                new object[] {Gf8, 5, 6, 3},
                                new object[] {Gf8, 4, 7, 3},
                                new object[] {Gf8, 5, 5, 0},
                                new object[] {Gf27, 3, 4, 7},
                                new object[] {Gf27, 10, 12, 22},
                                new object[] {Gf27, 11, 10, 18},
                                new object[] {Gf27, 18, 9, 0},
                                new object[] {Gf27, 20, 17, 7},
                                new object[] {Gf27, 9, 21, 3}
                            };
            SubtractTestsData = new[]
                                 {
                                     new object[] {Gf8, 3, 4, 7},
                                     new object[] {Gf8, 1, 3, 2},
                                     new object[] {Gf8, 1, 6, 7},
                                     new object[] {Gf8, 5, 6, 3},
                                     new object[] {Gf8, 4, 7, 3},
                                     new object[] {Gf8, 5, 5, 0},
                                     new object[] {Gf27, 3, 4, 2},
                                     new object[] {Gf27, 10, 12, 7},
                                     new object[] {Gf27, 11, 14, 6},
                                     new object[] {Gf27, 18, 9, 9},
                                     new object[] {Gf27, 20, 17, 12},
                                     new object[] {Gf27, 9, 21, 24}
                                 };
            MultiplyTestsData = new[]
                                 {
                                     new object[] {Gf8, 0, 4, 0},
                                     new object[] {Gf8, 2, 4, 3},
                                     new object[] {Gf8, 3, 4, 7},
                                     new object[] {Gf8, 2, 3, 6},
                                     new object[] {Gf8, 6, 7, 4},
                                     new object[] {Gf8, 2, 7, 5},
                                     new object[] {Gf27, 0, 4, 0},
                                     new object[] {Gf27, 12, 5, 1},
                                     new object[] {Gf27, 3, 4, 12},
                                     new object[] {Gf27, 21, 3, 17},
                                     new object[] {Gf27, 20, 21, 26},
                                     new object[] {Gf27, 2, 7, 5}
                                 };
            DivideTestsData = new[]
                               {
                                   new object[] {Gf8, 0, 3, 0},
                                   new object[] {Gf8, 4, 3, 5},
                                   new object[] {Gf8, 7, 2, 6},
                                   new object[] {Gf8, 2, 7, 3},
                                   new object[] {Gf27, 0, 3, 0},
                                   new object[] {Gf27, 15, 7, 6},
                                   new object[] {Gf27, 11, 26, 14},
                                   new object[] {Gf27, 10, 17, 15},
                                   new object[] {Gf27, 2, 23, 25}
                               };
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(6)]
        public void ShouldNotCreateFieldWithNotPrimePowerOrder(int fieldOrder)
        {
            Assert.Throws<ArgumentException>(() => new PrimePowerOrderField(fieldOrder));
        }

        [Theory]
        [MemberData(nameof(IncorrectFieldCreationTestsData))]
        public void ShouldNotCreateFieldWithIncorrectIrreduciblePolynomial(int fieldOrder, Polynomial irreduciblePolynomial)
        {
            Assert.ThrowsAny<ArgumentException>(() => new PrimePowerOrderField(fieldOrder, irreduciblePolynomial));
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(8, false)]
        public void ShouldApproveFieldMemberUnderGf8(int element, bool expected)
        {
            // When
            var actual = Gf8.IsFieldElement(element);

            // Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(SumTestsData))]
        public void ShouldSumTwoFieldElements(PrimePowerOrderField field, int firstItem, int secondItem, int expected)
        {
            // When
            var actual =  field.Add(firstItem, secondItem);

            //Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(SubtractTestsData))]
        public void ShouldSubtractTwoFieldElements(PrimePowerOrderField field, int minuend, int subtrahend, int expected)
        {
            // When
            var actual = field.Subtract(minuend, subtrahend);

            //Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(MultiplyTestsData))]
        public void ShouldMultiplyTwoFieldElements(PrimePowerOrderField field, int firstMultiplied, int secondMultiplied, int expected)
        {
            // When
            var actual = field.Multiply(firstMultiplied, secondMultiplied);

            // Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(DivideTestsData))]
        public void ShouldDivideTwoFieldElements(PrimePowerOrderField field, int dividend, int divisor, int expected)
        {
            // When
            var actual = field.Divide(dividend, divisor);

            // Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(3, 6)]
        [InlineData(4, 8)]
        [InlineData(5, 7)]
        [InlineData(6, 3)]
        [InlineData(7, 5)]
        [InlineData(8, 4)]
        [InlineData(9, 18)]
        [InlineData(10, 20)]
        [InlineData(11, 19)]
        [InlineData(12, 24)]
        [InlineData(13, 26)]
        [InlineData(14, 25)]
        [InlineData(15, 21)]
        [InlineData(16, 23)]
        [InlineData(17, 22)]
        [InlineData(18, 9)]
        [InlineData(19, 11)]
        [InlineData(20, 10)]
        [InlineData(21, 15)]
        [InlineData(22, 17)]
        [InlineData(23, 16)]
        [InlineData(24, 12)]
        [InlineData(25, 14)]
        [InlineData(26, 13)]
        public void ShouldInverseElementForAdditionUnderGf27(int element, int expected)
        {
            // When
            var actual = Gf27.InverseForAddition(element);

            // Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 11)]
        [InlineData(4, 15)]
        [InlineData(5, 12)]
        [InlineData(6, 19)]
        [InlineData(7, 24)]
        [InlineData(8, 21)]
        [InlineData(9, 22)]
        [InlineData(10, 26)]
        [InlineData(11, 3)]
        [InlineData(12, 5)]
        [InlineData(13, 20)]
        [InlineData(14, 23)]
        [InlineData(15, 4)]
        [InlineData(16, 25)]
        [InlineData(17, 18)]
        [InlineData(18, 17)]
        [InlineData(19, 6)]
        [InlineData(20, 13)]
        [InlineData(21, 8)]
        [InlineData(22, 9)]
        [InlineData(23, 14)]
        [InlineData(24, 7)]
        [InlineData(25, 16)]
        [InlineData(26, 10)]
        public void ShouldInverseForMultiplicationUnderGf27(int element, int expected)
        {
            // When
            var actual = Gf27.InverseForMultiplication(element);

            // Then
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(17)]
        [InlineData(18)]
        [InlineData(19)]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(22)]
        [InlineData(23)]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(26)]
        [InlineData(63)]
        [InlineData(-63)]
        public void ShouldGetGeneratingElementPowerGet(int power)
        {
            // When
            var element = Gf27.GetGeneratingElementPower(power);
            var invertedElement = Gf27.GetGeneratingElementPower(-power);
            var one = new Polynomial(Gf27.IrreduciblePolynomial.Field, 1);

            // Then
            Assert.Equal(one, (Gf27[element] * Gf27[invertedElement])%Gf27.IrreduciblePolynomial);
        }

        [Theory]
        [InlineData(0, 1, 0)]
        [InlineData(0, 0, 1)]
        [InlineData(1, 15, 1)]
        [InlineData(1, -2, 1)]
        [InlineData(3, 3, 4)]
        [InlineData(3, -3, 15)]
        public void ShouldPowerElementToSpecifiedDegree(int element, int power, int expected)
        {
            // When
            var actual = Gf27.Pow(element, power);

            // Then
            Assert.Equal(expected, actual);
        }
    }
}