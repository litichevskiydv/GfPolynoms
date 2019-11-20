﻿namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Matrices;
    using Xunit;

    public class FieldElementsMatrixTests
    {
        #region TestCases

        public class FromInitializerConstructorParametersValidationTestCase
        {
            public GaloisField Field { get; set; } 

            public int RowsCount { get; set; }

            public int ColumnsCount { get; set; }

            public Func<int, int, FieldElement> ElementInitializer { get; set; }
        }

        public class FromNumbersArrayConstructorParametersValidationTestCase
        {
            public GaloisField Field { get; set; }

            public int[,] Elements { get; set; }
        }

        public class ElementsGetterParametersValidationTestCase
        {
            public FieldElementsMatrix Matrix { get; set; }

            public int RowNumber { get; set; }

            public int ColumnNumber { get; set; }
        }

        public class ElementsSetterParametersValidationTestCase : ElementsGetterParametersValidationTestCase
        {
            public  FieldElement Element { get; set; }
        }

        public class BinaryOperationParametersValidationTestCase
        {
            public FieldElementsMatrix FirstArgument { get; set; }

            public FieldElementsMatrix SecondArgument { get; set; }
        }

        public class MultiplyByFieldElementParametersValidationTestCase
        {
            public FieldElement FirstArgument { get; set; }

            public FieldElementsMatrix SecondArgument { get; set; }
        }

        public class PowParametersValidationTestCase
        {
            public FieldElementsMatrix Matrix { get; set; }

            public int Degree { get; set; }
        }

        #endregion

        [UsedImplicitly]
        public static TheoryData<FromInitializerConstructorParametersValidationTestCase> FromInitializerConstructorParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<FromNumbersArrayConstructorParametersValidationTestCase> FromNumbersArrayConstructorParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<FieldElement[,]> FromFieldElementsArrayConstructorParametersValidationTestCases;
        [UsedImplicitly] 
        public static TheoryData<ElementsGetterParametersValidationTestCase> ElementsGetterParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<ElementsSetterParametersValidationTestCase> ElementsSetterParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<BinaryOperationParametersValidationTestCase> AdditionParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<MultiplyByFieldElementParametersValidationTestCase> MultiplyByFieldElementParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<BinaryOperationParametersValidationTestCase> MultiplyByMatrixParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<PowParametersValidationTestCase> PowParametersValidationTestCases;

        static FieldElementsMatrixTests()
        {
            var gf2 = new PrimeOrderField(2);
            var gf3 = new PrimeOrderField(3);
            var matrix = new FieldElementsMatrix(gf2, 3, 3);

            FromInitializerConstructorParametersValidationTestCases 
                = new TheoryData<FromInitializerConstructorParametersValidationTestCase>
                  {
                      new FromInitializerConstructorParametersValidationTestCase
                      {
                          RowsCount = 1, ColumnsCount = 1
                      },
                      new FromInitializerConstructorParametersValidationTestCase
                      {
                          Field = gf2, RowsCount = -1, ColumnsCount = 1
                      },
                      new FromInitializerConstructorParametersValidationTestCase
                      {
                          Field = gf2, RowsCount = 1, ColumnsCount = -1
                      },
                      new FromInitializerConstructorParametersValidationTestCase
                      {
                          Field = gf2, RowsCount = 2, ColumnsCount = 2, ElementInitializer = (i, j) => null
                      },
                      new FromInitializerConstructorParametersValidationTestCase
                      {
                          Field = gf2, RowsCount = 2, ColumnsCount = 2, ElementInitializer = (i, j) => gf3.One()
                      }
                  };
            FromNumbersArrayConstructorParametersValidationTestCases
                = new TheoryData<FromNumbersArrayConstructorParametersValidationTestCase>
                  {
                      new FromNumbersArrayConstructorParametersValidationTestCase
                      {
                          Elements = new[,] {{1, 0}, { 0, 1} }
                      },
                      new FromNumbersArrayConstructorParametersValidationTestCase
                      {
                          Field = gf2, Elements = new int[0,2]
                      },
                      new FromNumbersArrayConstructorParametersValidationTestCase
                      {
                          Field = gf2, Elements = new int[2,0]
                      },
                      new FromNumbersArrayConstructorParametersValidationTestCase
                      {
                          Field = gf2, Elements = new[,] {{1, 2}, { 0, 1} }
                      }
                  };
            FromFieldElementsArrayConstructorParametersValidationTestCases
                = new TheoryData<FieldElement[,]>
                  {
                      null,
                      new FieldElement[0, 2],
                      new FieldElement[2, 0],
                      new[,] {{gf2.One(), null}, {null, gf2.One()}},
                      new[,] {{gf2.One(), gf3.Zero()}, {gf3.Zero(), gf2.One()}}
                  };
            ElementsGetterParametersValidationTestCases 
                = new TheoryData<ElementsGetterParametersValidationTestCase>
                  {
                      new ElementsGetterParametersValidationTestCase
                      {
                          Matrix = matrix, RowNumber = -1, ColumnNumber = 0
                      },
                      new ElementsGetterParametersValidationTestCase
                      {
                          Matrix = matrix, RowNumber = 3, ColumnNumber = 0
                      },
                      new ElementsGetterParametersValidationTestCase
                      {
                          Matrix = matrix, RowNumber = 0, ColumnNumber = -1
                      },
                      new ElementsGetterParametersValidationTestCase
                      {
                          Matrix = matrix, RowNumber = 0, ColumnNumber = 3
                      }
                  };
            ElementsSetterParametersValidationTestCases
                = new TheoryData<ElementsSetterParametersValidationTestCase>
                  {
                      new ElementsSetterParametersValidationTestCase
                      {
                          Matrix = matrix, RowNumber = -1, ColumnNumber = 0, Element = gf2.One()
                      },
                      new ElementsSetterParametersValidationTestCase
                      {
                          Matrix = matrix, RowNumber = 3, ColumnNumber = 0, Element = gf2.One()
                      },
                      new ElementsSetterParametersValidationTestCase
                      {
                          Matrix = matrix, RowNumber = 0, ColumnNumber = -1, Element = gf2.One()
                      },
                      new ElementsSetterParametersValidationTestCase
                      {
                          Matrix = matrix, RowNumber = 0, ColumnNumber = 3, Element = gf2.One()
                      },
                      new ElementsSetterParametersValidationTestCase
                      {
                          Matrix = matrix, RowNumber = 0, ColumnNumber = 0
                      },
                      new ElementsSetterParametersValidationTestCase
                      {
                          Matrix = matrix, RowNumber = 0, ColumnNumber = 0, Element = gf3.One()
                      }
                  };
            AdditionParametersValidationTestCases
                = new TheoryData<BinaryOperationParametersValidationTestCase>
                  {
                      new BinaryOperationParametersValidationTestCase
                      {
                          FirstArgument = matrix
                      },
                      new BinaryOperationParametersValidationTestCase
                      {
                          SecondArgument = matrix
                      },
                      new BinaryOperationParametersValidationTestCase
                      {
                          FirstArgument = matrix, SecondArgument = new FieldElementsMatrix(gf3, 3, 3)
                      },
                      new BinaryOperationParametersValidationTestCase
                      {
                          FirstArgument = matrix, SecondArgument = new FieldElementsMatrix(gf2, 2, 3)
                      },
                      new BinaryOperationParametersValidationTestCase
                      {
                          FirstArgument = matrix, SecondArgument = new FieldElementsMatrix(gf2, 3, 2)
                      }
                  };
            MultiplyByFieldElementParametersValidationTestCases
                = new TheoryData<MultiplyByFieldElementParametersValidationTestCase>
                  {
                    new MultiplyByFieldElementParametersValidationTestCase
                    {
                        FirstArgument = gf2.One()
                    },
                    new MultiplyByFieldElementParametersValidationTestCase
                    {
                        SecondArgument = matrix
                    },
                    new MultiplyByFieldElementParametersValidationTestCase
                    {
                        FirstArgument = gf3.CreateElement(2), SecondArgument = matrix
                    }
                  };
            MultiplyByMatrixParametersValidationTestCases
                = new TheoryData<BinaryOperationParametersValidationTestCase>
                  {
                      new BinaryOperationParametersValidationTestCase
                      {
                          FirstArgument = matrix
                      },
                      new BinaryOperationParametersValidationTestCase
                      {
                          SecondArgument = matrix
                      },
                      new BinaryOperationParametersValidationTestCase
                      {
                          FirstArgument = matrix, SecondArgument = new FieldElementsMatrix(gf3, 3, 3)
                      },
                      new BinaryOperationParametersValidationTestCase
                      {
                          FirstArgument = matrix, SecondArgument = new FieldElementsMatrix(gf3, 2, 3)
                      }
                  };
            PowParametersValidationTestCases 
                = new TheoryData<PowParametersValidationTestCase>
                  {
                      new PowParametersValidationTestCase
                      {
                          Degree = 1
                      },
                      new PowParametersValidationTestCase
                      {
                          Matrix = matrix, Degree = -1
                      },
                      new PowParametersValidationTestCase
                      {
                          Matrix = new FieldElementsMatrix(gf2, 2, 3), Degree = 2
                      }
                  };
        }

        [Theory]
        [MemberData(nameof(FromInitializerConstructorParametersValidationTestCases))]
        public void FromInitializerConstructorMustValidateParameters(FromInitializerConstructorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(
                () => new FieldElementsMatrix(testCase.Field, testCase.RowsCount, testCase.ColumnsCount, testCase.ElementInitializer)
            );
        }

        [Theory]
        [MemberData(nameof(FromNumbersArrayConstructorParametersValidationTestCases))]
        public void FromNumbersArrayConstructorMustValidateParameters(FromNumbersArrayConstructorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => new FieldElementsMatrix(testCase.Field, testCase.Elements));
        }

        [Theory]
        [MemberData(nameof(FromFieldElementsArrayConstructorParametersValidationTestCases))]
        public void FromFieldElementsArrayConstructorMustValidateParameters(FieldElement[,] matrixElements)
        {
            Assert.ThrowsAny<ArgumentException>(() => new FieldElementsMatrix(matrixElements));
        }

        [Fact]
        public void FromFieldElementsMatrixConstructorMustValidateParameters()
        {
            Assert.Throws<ArgumentNullException>(() => new FieldElementsMatrix((FieldElementsMatrix) null));
        }

        [Theory]
        [MemberData(nameof(ElementsGetterParametersValidationTestCases))]
        public void ElementsGetterMustValidateParameters(ElementsGetterParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Matrix[testCase.RowNumber, testCase.ColumnNumber]);
        }

        [Theory]
        [MemberData(nameof(ElementsSetterParametersValidationTestCases))]

        public void ElementsSetterMustValidateParameters(ElementsSetterParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Matrix[testCase.RowNumber, testCase.ColumnNumber] = testCase.Element);
        }

        [Theory]
        [MemberData(nameof(AdditionParametersValidationTestCases))]
        public void AddMustValidateParameters(BinaryOperationParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.FirstArgument + testCase.SecondArgument);
        }

        [Theory]
        [MemberData(nameof(AdditionParametersValidationTestCases))]
        public void SubtractMustValidateParameters(BinaryOperationParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.FirstArgument - testCase.SecondArgument);
        }

        [Theory]
        [MemberData(nameof(MultiplyByFieldElementParametersValidationTestCases))]
        public void MultiplyByFieldElementMustValidateParameters(MultiplyByFieldElementParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.FirstArgument * testCase.SecondArgument);
        }

        [Theory]
        [MemberData(nameof(MultiplyByMatrixParametersValidationTestCases))]
        public void MultiplyByMatrixMustValidateParameters(BinaryOperationParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.FirstArgument * testCase.SecondArgument);
        }

        [Theory]
        [MemberData(nameof(PowParametersValidationTestCases))]
        public void PowMustValidateParameters(PowParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => FieldElementsMatrix.Pow(testCase.Matrix, testCase.Degree));
        }

        [Fact]
        public void MustAddTwoMatrices()
        {
            // Given
            var gf5 = new PrimeOrderField(5);
            var first = new FieldElementsMatrix(gf5, new[,] {{1, 2}, {3, 4}});
            var second = new FieldElementsMatrix(gf5, new[,] {{1, 3}, {2, 4}});

            // When
            var actualResult = first + second;

            // Then
            var expectedResult = new FieldElementsMatrix(gf5, new[,] {{2, 0}, {0, 3}});
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void MustSubtractTwoMatrices()
        {
            // Given
            var gf5 = new PrimeOrderField(5);
            var first = new FieldElementsMatrix(gf5, new[,] {{1, 2}, {3, 4}});
            var second = new FieldElementsMatrix(gf5, new[,] {{1, 3}, {2, 4}});

            // When
            var actualResult = first - second;

            // Then
            var expectedResult = new FieldElementsMatrix(gf5, new[,] {{0, 4}, {1, 0}});
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void MustMultiplyMatrixByFieldElement()
        {
            // Given
            var gf5 = new PrimeOrderField(5);
            var fieldElement = gf5.CreateElement(3);
            var matrix = new FieldElementsMatrix(gf5, new[,] {{1, 2}, {3, 4}});

            // When
            var actualResult = fieldElement * matrix;

            // Then
            var expectedResult = new FieldElementsMatrix(gf5, new[,] {{3, 1}, {4, 2}});
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void MustMultiplyTwoMatrices()
        {
            // Given
            var gf5 = new PrimeOrderField(5);
            var first = new FieldElementsMatrix(gf5, new[,] {{1, 2}, {3, 4}});
            var second = new FieldElementsMatrix(gf5, new[,] {{1, 3}, {2, 4}});

            // When
            var actualResult = first * second;

            // Then
            var expectedResult = new FieldElementsMatrix(gf5, new[,] {{0, 1}, {1, 0}});
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void MustCalculateMatrixDegree()
        {
            // Given
            var gf5 = new PrimeOrderField(5);
            var matrix = new FieldElementsMatrix(gf5, new[,] { { 1, 2 }, { 3, 4 } });
            const int degree = 3;

            // When
            var actualResult = FieldElementsMatrix.Pow(matrix, degree);

            // Then
            var expectedResult = new FieldElementsMatrix(gf5, new[,] {{2, 4}, {1, 3}});
            Assert.Equal(expectedResult, actualResult);
        }
    }
}