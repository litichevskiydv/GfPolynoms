namespace AppliedAlgebra.GfPolynoms.Tests.GaluaFields
{
    using System;
    using System.Collections.Generic;
    using GaloisFields;
    using JetBrains.Annotations;
    using Xunit;

    public class PrimePowerOrderFieldTests
    {
        public class IncorrectFieldCreationTestCase
        {
            public int FieldOrder { get; set; }

            public Polynomial IrreduciblePolynomial { get; set; }
        }

        public class BinaryOperationTestCase
        {
            public PrimePowerOrderField Field { get; set; }

            public int FirstOperand { get; set; }

            public int SecondOperand { get; set; }

            public int Expected { get; set; }
        }

        private static readonly PrimePowerOrderField Gf8;
        private static readonly PrimePowerOrderField Gf27;

        [UsedImplicitly]
        public static readonly TheoryData<IncorrectFieldCreationTestCase> IncorrectFieldCreationTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<BinaryOperationTestCase> SumTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<BinaryOperationTestCase> SubtractTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<BinaryOperationTestCase> MultiplyTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<BinaryOperationTestCase> DivideTestsData;

        static PrimePowerOrderFieldTests()
        {
            Gf8 = new PrimePowerOrderField(8, new Polynomial(new PrimeOrderField(2), 1, 1, 0, 1));
            Gf27 = new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1));

            IncorrectFieldCreationTestsData
                = new TheoryData<IncorrectFieldCreationTestCase>
                  {
                      new IncorrectFieldCreationTestCase {FieldOrder = 8},
                      new IncorrectFieldCreationTestCase
                      {
                          FieldOrder = 8,
                          IrreduciblePolynomial = new Polynomial(new PrimeOrderField(3), 1)
                      },
                      new IncorrectFieldCreationTestCase
                      {
                          FieldOrder = 8,
                          IrreduciblePolynomial = new Polynomial(new PrimeOrderField(2), 1)
                      },
                      new IncorrectFieldCreationTestCase
                      {
                          FieldOrder = 8,
                          IrreduciblePolynomial = new Polynomial(new PrimeOrderField(2), 0, 0, 0, 1)
                      }
                  };

            SumTestsData
                = new TheoryData<BinaryOperationTestCase>
                  {
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 3, SecondOperand = 4, Expected = 7},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 1, SecondOperand = 3, Expected = 2},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 1, SecondOperand = 6, Expected = 7},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 5, SecondOperand = 6, Expected = 3},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 4, SecondOperand = 7, Expected = 3},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 5, SecondOperand = 5, Expected = 0},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 3, SecondOperand = 4, Expected = 7},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 10, SecondOperand = 12, Expected = 22},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 11, SecondOperand = 10, Expected = 18},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 18, SecondOperand = 9, Expected = 0},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 20, SecondOperand = 17, Expected = 7},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 9, SecondOperand = 21, Expected = 3}
                  };
            SubtractTestsData
                = new TheoryData<BinaryOperationTestCase>
                  {
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 3, SecondOperand = 4, Expected = 7},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 1, SecondOperand = 3, Expected = 2},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 1, SecondOperand = 6, Expected = 7},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 5, SecondOperand = 6, Expected = 3},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 4, SecondOperand = 7, Expected = 3},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 5, SecondOperand = 5, Expected = 0},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 3, SecondOperand = 4, Expected = 2},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 10, SecondOperand = 12, Expected = 7},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 11, SecondOperand = 14, Expected = 6},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 18, SecondOperand = 9, Expected = 9},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 20, SecondOperand = 17, Expected = 12},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 9, SecondOperand = 21, Expected = 24}
                  };
            MultiplyTestsData
                = new TheoryData<BinaryOperationTestCase>
                  {
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 0, SecondOperand = 4, Expected = 0},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 2, SecondOperand = 4, Expected = 3},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 3, SecondOperand = 4, Expected = 7},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 2, SecondOperand = 3, Expected = 6},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 6, SecondOperand = 7, Expected = 4},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 2, SecondOperand = 7, Expected = 5},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 0, SecondOperand = 4, Expected = 0},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 12, SecondOperand = 5, Expected = 1},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 3, SecondOperand = 4, Expected = 12},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 21, SecondOperand = 3, Expected = 17},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 20, SecondOperand = 21, Expected = 26},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 2, SecondOperand = 7, Expected = 5}
                  };
            DivideTestsData
                = new TheoryData<BinaryOperationTestCase>
                  {
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 0, SecondOperand = 3, Expected = 0},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 4, SecondOperand = 3, Expected = 5},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 7, SecondOperand = 2, Expected = 6},
                      new BinaryOperationTestCase {Field = Gf8, FirstOperand = 2, SecondOperand = 7, Expected = 3},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 0, SecondOperand = 3, Expected = 0},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 15, SecondOperand = 7, Expected = 6},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 11, SecondOperand = 26, Expected = 14},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 10, SecondOperand = 17, Expected = 15},
                      new BinaryOperationTestCase {Field = Gf27, FirstOperand = 2, SecondOperand = 23, Expected = 25}
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
        public void ShouldNotCreateFieldWithIncorrectIrreduciblePolynomial(IncorrectFieldCreationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => new PrimePowerOrderField(testCase.FieldOrder, testCase.IrreduciblePolynomial));
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
        public void ShouldSumTwoFieldElements(BinaryOperationTestCase testCase)
        {
            // When
            var actual = testCase.Field.Add(testCase.FirstOperand, testCase.SecondOperand);

            //Then
            Assert.Equal(testCase.Expected, actual);
        }

        [Theory]
        [MemberData(nameof(SubtractTestsData))]
        public void ShouldSubtractTwoFieldElements(BinaryOperationTestCase testCase)
        {
            // When
            var actual = testCase.Field.Subtract(testCase.FirstOperand, testCase.SecondOperand);

            //Then
            Assert.Equal(testCase.Expected, actual);
        }

        [Theory]
        [MemberData(nameof(MultiplyTestsData))]
        public void ShouldMultiplyTwoFieldElements(BinaryOperationTestCase testCase)
        {
            // When
            var actual = testCase.Field.Multiply(testCase.FirstOperand, testCase.SecondOperand);

            // Then
            Assert.Equal(testCase.Expected, actual);
        }

        [Theory]
        [MemberData(nameof(DivideTestsData))]
        public void ShouldDivideTwoFieldElements(BinaryOperationTestCase testCase)
        {
            // When
            var actual = testCase.Field.Divide(testCase.FirstOperand, testCase.SecondOperand);

            // Then
            Assert.Equal(testCase.Expected, actual);
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