namespace AppliedAlgebra.GfAlgorithms.Tests
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
    }
}