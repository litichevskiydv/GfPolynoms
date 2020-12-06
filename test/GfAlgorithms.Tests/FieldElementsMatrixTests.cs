namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System;
    using System.Linq;
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

        public class DiagonalizationTestCase
        {
            public FieldElementsMatrix Matrix { get; set; }

            public FieldElementsMatrix ExpectedTriangularMatrix { get; set; }
        }

        public class SubmatrixCreationParametersValidationTestCase
        {
            public FieldElementsMatrix Matrix { get; set; }

            public int[] IncludedRowsIndices { get; set; }

            public int[] IncludedColumnsIndices { get; set; }
        }

        public class AppendColumnParametersValidationTestCase
        {
            public FieldElementsMatrix Matrix { get; set; }

            public FieldElementsMatrix Column { get; set; }
        }

        public class AppendRowParametersValidationTestCase
        {
            public FieldElementsMatrix Matrix { get; set; }

            public FieldElementsMatrix Row { get; set; }
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
        [UsedImplicitly] 
        public static TheoryData<DiagonalizationTestCase> DiagonalizationTestCases;
        [UsedImplicitly]
        public static TheoryData<FieldElement[]> CirculantMatrixFromFieldElementsArrayConstructorParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<SubmatrixCreationParametersValidationTestCase> SubmatrixFromRowsCreationParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<SubmatrixCreationParametersValidationTestCase> SubmatrixFromColumnsCreationParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<AppendColumnParametersValidationTestCase> AppendColumnParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<AppendRowParametersValidationTestCase> AppendRowParametersValidationTestCases;

        static FieldElementsMatrixTests()
        {
            var gf2 = GaloisField.Create(2);
            var gf3 = GaloisField.Create(3);
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
            DiagonalizationTestCases
                = new TheoryData<DiagonalizationTestCase>
                  {
                      new DiagonalizationTestCase
                      {
                          Matrix = new FieldElementsMatrix(gf3, new[,] {{1, 2, 1}, {2, 1, 1}, {0, 1, 2}}),
                          ExpectedTriangularMatrix = new FieldElementsMatrix(gf3, new[,] {{1, 0, 0}, {0, 1, 0}, {0, 0, 2}})
                      },
                      new DiagonalizationTestCase
                      {
                          Matrix = new FieldElementsMatrix(gf3, new[,] {{1, 2, 1}, {2, 1, 1}, {0, 0, 2}}),
                          ExpectedTriangularMatrix = new FieldElementsMatrix(gf3, new[,] {{1, 2, 0}, {0, 0, 2}, {0, 0, 0}})
                      }
                  };
            CirculantMatrixFromFieldElementsArrayConstructorParametersValidationTestCases
                = new TheoryData<FieldElement[]>
                  {
                      null,
                      new FieldElement[0]
                  };

            SubmatrixFromRowsCreationParametersValidationTestCases
                = new TheoryData<SubmatrixCreationParametersValidationTestCase>
                  {
                      new SubmatrixCreationParametersValidationTestCase {IncludedRowsIndices = new[] {0, 2}},
                      new SubmatrixCreationParametersValidationTestCase {Matrix = matrix},
                      new SubmatrixCreationParametersValidationTestCase {Matrix = matrix, IncludedRowsIndices = new int[0]},
                      new SubmatrixCreationParametersValidationTestCase {Matrix = matrix, IncludedRowsIndices = new[] {0, 0}},
                      new SubmatrixCreationParametersValidationTestCase {Matrix = matrix, IncludedRowsIndices = new[] {-1}},
                      new SubmatrixCreationParametersValidationTestCase {Matrix = matrix, IncludedRowsIndices = new[] {5}}
                  };
            SubmatrixFromColumnsCreationParametersValidationTestCases
                = new TheoryData<SubmatrixCreationParametersValidationTestCase>
                  {
                      new SubmatrixCreationParametersValidationTestCase {IncludedColumnsIndices = new[] {0, 2}},
                      new SubmatrixCreationParametersValidationTestCase {Matrix = matrix},
                      new SubmatrixCreationParametersValidationTestCase {Matrix = matrix, IncludedColumnsIndices = new int[0]},
                      new SubmatrixCreationParametersValidationTestCase {Matrix = matrix, IncludedColumnsIndices = new[] {0, 0}},
                      new SubmatrixCreationParametersValidationTestCase {Matrix = matrix, IncludedColumnsIndices = new[] {-1}},
                      new SubmatrixCreationParametersValidationTestCase {Matrix = matrix, IncludedColumnsIndices = new[] {5}}
                  };
            AppendColumnParametersValidationTestCases
                = new TheoryData<AppendColumnParametersValidationTestCase>
                  {
                      new AppendColumnParametersValidationTestCase(),
                      new AppendColumnParametersValidationTestCase {Matrix = matrix},
                      new AppendColumnParametersValidationTestCase {Matrix = matrix, Column = matrix}
                  };
            AppendRowParametersValidationTestCases
                = new TheoryData<AppendRowParametersValidationTestCase>
                  {
                      new AppendRowParametersValidationTestCase(),
                      new AppendRowParametersValidationTestCase {Matrix = matrix},
                      new AppendRowParametersValidationTestCase {Matrix = matrix, Row = matrix}
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
            var gf5 = GaloisField.Create(5);
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
            var gf5 = GaloisField.Create(5);
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
            var gf5 = GaloisField.Create(5);
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
            var gf5 = GaloisField.Create(5);
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
            var gf5 = GaloisField.Create(5);
            var matrix = new FieldElementsMatrix(gf5, new[,] { { 1, 2 }, { 3, 4 } });
            const int degree = 3;

            // When
            var actualResult = FieldElementsMatrix.Pow(matrix, degree);

            // Then
            var expectedResult = new FieldElementsMatrix(gf5, new[,] {{2, 4}, {1, 3}});
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void MustTransposeMatrix()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var matrix = new FieldElementsMatrix(gf5, new[,] {{1, 2, 3}, {4, 0, 1}});

            // When
            var actualResult = FieldElementsMatrix.Transpose(matrix);

            // Then
            var expectedResult = new FieldElementsMatrix(gf5, new[,] {{1, 4}, {2, 0}, {3, 1}});
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [MemberData(nameof(DiagonalizationTestCases))]
        public void MustDiagonalizeMatrix(DiagonalizationTestCase testCase)
        {
            // When
            var actualTriangularMatrix = FieldElementsMatrix.Diagonalize(
                testCase.Matrix,
                new DiagonalizationOptions {MaxDegreeOfParallelism = 1}
            );

            // Then
            Assert.Equal(testCase.ExpectedTriangularMatrix, actualTriangularMatrix);
        }

        [Fact]
        public void MustCalculateDeterminant()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var matrix = new FieldElementsMatrix(gf3, new[,] {{1, 2, 1}, {2, 1, 1}, {0, 1, 2}});

            // When
            var actualDeterminant = matrix.CalculateDeterminant();

            // Then
            var expectedDeterminant = gf3.One();
            Assert.Equal(expectedDeterminant, actualDeterminant);
        }

        [Fact]
        public void MustCalculateRank()
        {
            // Given
            var gf3 = GaloisField.Create(3);
            var matrix = new FieldElementsMatrix(gf3, new[,] { { 1, 2, 1 }, { 2, 1, 1 }, { 0, 0, 2 } });

            // When
            var actualRank = matrix.CalculateRank();

            // Then
            const int expectedRank = 2;
            Assert.Equal(expectedRank, actualRank);
        }

        [Fact]
        public void CirculantMatrixFromNumbersArrayConstructorMustValidateParameters()
        {
            Assert.Throws<ArgumentNullException>(() => FieldElementsMatrix.CirculantMatrix(GaloisField.Create(3), null));
        }

        [Fact]
        public void MustCreateCirculantMatrixFromNumbersArray()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var firstRow = new[] {0, 1, 2, 3, 4};

            // When
            var actualMatrix = FieldElementsMatrix.CirculantMatrix(gf5, firstRow);

            // Then
            var expectedMatrix = new FieldElementsMatrix(
                gf5,
                new[,]
                {
                    {0, 1, 2, 3, 4},
                    {4, 0, 1, 2, 3},
                    {3, 4, 0, 1, 2},
                    {2, 3, 4, 0, 1},
                    {1, 2, 3, 4, 0}
                }
            );
            Assert.Equal(expectedMatrix, actualMatrix);
        }

        [Theory]
        [MemberData(nameof(CirculantMatrixFromFieldElementsArrayConstructorParametersValidationTestCases))]
        public void CirculantMatrixFromFieldElementsArrayConstructorMustValidateParameters(FieldElement[] firstRow)
        {
            Assert.ThrowsAny<ArgumentException>(() => FieldElementsMatrix.CirculantMatrix(firstRow));
        }
        [Fact]
        public void MustCreateCirculantMatrixFromFieldElementsArray()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var firstRow = new[] {gf5.Zero(), gf5.One(), gf5.CreateElement(2), gf5.CreateElement(3), gf5.CreateElement(4)};

            // When
            var actualMatrix = FieldElementsMatrix.CirculantMatrix(firstRow);

            // Then
            var expectedMatrix = new FieldElementsMatrix(
                gf5,
                new[,]
                {
                    {0, 1, 2, 3, 4},
                    {4, 0, 1, 2, 3},
                    {3, 4, 0, 1, 2},
                    {2, 3, 4, 0, 1},
                    {1, 2, 3, 4, 0}
                }
            );
            Assert.Equal(expectedMatrix, actualMatrix);
        }

        [Fact]
        public void MustNotCreateDoubleCirculantMatrixFromNumbersArray()
        {
            Assert.Throws<ArgumentException>(() => FieldElementsMatrix.DoubleCirculantMatrix(GaloisField.Create(3), 0, 1, 2));
        }

        [Fact]
        public void MustCreateDoubleCirculantMatrixFromNumbersArray()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var firstRow = new[] { 0, 1, 2, 3, 4, 0};

            // When
            var actualMatrix = FieldElementsMatrix.DoubleCirculantMatrix(gf5, firstRow);

            // Then
            var expectedMatrix = new FieldElementsMatrix(
                gf5,
                new[,]
                {
                    {0, 1, 2, 3, 4, 0},
                    {4, 0, 0, 1, 2, 3},
                    {2, 3, 4, 0, 0, 1}
                }
            );
            Assert.Equal(expectedMatrix, actualMatrix);
        }

        [Fact]
        public void MustNotCreateDoubleCirculantMatrixFromFieldElementsArray()
        {
            var gf3 = GaloisField.Create(3);
            Assert.Throws<ArgumentException>(() => FieldElementsMatrix.DoubleCirculantMatrix(gf3.Zero(), gf3.Zero(), gf3.Zero()));
        }

        [Fact]
        public void MustCreateDoubleCirculantMatrixFromFieldElementsArray()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var firstRow = new[] {gf5.Zero(), gf5.One(), gf5.CreateElement(2), gf5.CreateElement(3), gf5.CreateElement(4), gf5.Zero()};

            // When
            var actualMatrix = FieldElementsMatrix.DoubleCirculantMatrix(firstRow);

            // Then
            var expectedMatrix = new FieldElementsMatrix(
                gf5,
                new[,]
                {
                    {0, 1, 2, 3, 4, 0},
                    {4, 0, 0, 1, 2, 3},
                    {2, 3, 4, 0, 0, 1}
                }
            );
            Assert.Equal(expectedMatrix, actualMatrix);
        }

        [Theory]
        [MemberData(nameof(SubmatrixFromRowsCreationParametersValidationTestCases))]
        public void MustValidateParametersDuringSubmatrixCreationFromRows(SubmatrixCreationParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Matrix.CreateSubmatrixFromRows(testCase.IncludedRowsIndices));
        }

        [Fact]
        public void MustCreateSumatrixFromSelectedRows()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var matrix = new FieldElementsMatrix(
                gf5,
                new[,]
                {
                    {0, 1, 2, 3, 4},
                    {4, 0, 1, 2, 3},
                    {3, 4, 0, 1, 2},
                    {2, 3, 4, 0, 1},
                    {1, 2, 3, 4, 0}
                }
            );

            // When
            var actualSubmatrix = matrix.CreateSubmatrixFromRows(4, 2, 0);

            // Then
            var expectedSubmatrix = new FieldElementsMatrix(
                gf5,
                new[,]
                {
                    {1, 2, 3, 4, 0},
                    {3, 4, 0, 1, 2},
                    {0, 1, 2, 3, 4}

                }
            );
            Assert.Equal(expectedSubmatrix, actualSubmatrix);
        }

        [Theory]
        [MemberData(nameof(SubmatrixFromColumnsCreationParametersValidationTestCases))]
        public void MustValidateParametersDuringSubmatrixCreationFromColumns(SubmatrixCreationParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Matrix.CreateSubmatrixFromColumns(testCase.IncludedColumnsIndices));
        }

        [Fact]
        public void MustCreateSumatrixFromColumns()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var matrix = new FieldElementsMatrix(
                gf5,
                new[,]
                {
                    {0, 1, 2, 3, 4},
                    {4, 0, 1, 2, 3},
                    {3, 4, 0, 1, 2},
                    {2, 3, 4, 0, 1},
                    {1, 2, 3, 4, 0}
                }
            );

            // When
            var actualSubmatrix = matrix.CreateSubmatrixFromColumns(4, 2, 0);

            // Then
            var expectedSubmatrix = new FieldElementsMatrix(
                gf5,
                new[,]
                {
                    {4, 2, 0},
                    {3, 1, 4},
                    {2, 0, 3},
                    {1, 4, 2},
                    {0, 3, 1}

                }
            );
            Assert.Equal(expectedSubmatrix, actualSubmatrix);
        }

        [Theory]
        [MemberData(nameof(SubmatrixFromRowsCreationParametersValidationTestCases))]
        [MemberData(nameof(SubmatrixFromColumnsCreationParametersValidationTestCases))]
        public void MustValidateParametersDuringSubmatrixCreation(SubmatrixCreationParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Matrix.CreateSubmatrix(testCase.IncludedRowsIndices, testCase.IncludedColumnsIndices));
        }

        [Fact]
        public void MustCreateSumatrix()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var matrix = new FieldElementsMatrix(
                gf5,
                new[,]
                {
                    {0, 1, 2, 3, 4},
                    {4, 0, 1, 2, 3},
                    {3, 4, 0, 1, 2},
                    {2, 3, 4, 0, 1},
                    {1, 2, 3, 4, 0}
                }
            );

            // When
            var actualSubmatrix = matrix.CreateSubmatrix(new[] {4, 2, 0}, new[] {4, 2, 0});

            // Then
            var expectedSubmatrix = new FieldElementsMatrix(
                gf5,
                new[,]
                {
                    {0, 3, 1},
                    {2, 0, 3},
                    {4, 2, 0}

                }
            );
            Assert.Equal(expectedSubmatrix, actualSubmatrix);
        }

        [Theory]
        [MemberData(nameof(AppendColumnParametersValidationTestCases))]
        public void MustValidateParametersDuringColumnAppending(AppendColumnParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Matrix.AppendColumn(testCase.Column));
        }

        [Fact]
        public void MustAppendColumnDeclaredAsMatrix()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var matrix = new FieldElementsMatrix(gf5, new[,] {{0, 3}, {2, 0}, {4, 2}});
            var column = new FieldElementsMatrix(gf5, new[,] {{1}, {3}, {0}});

            // When
            var actual = matrix.AppendColumn(column);

            // Then
            var expected = new FieldElementsMatrix(gf5, new[,] {{0, 3, 1}, {2, 0, 3}, {4, 2, 0}});
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MustAppendColumnDeclaredAsFieldElementsArray()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var matrix = new FieldElementsMatrix(gf5, new[,] {{0, 3}, {2, 0}, {4, 2}});
            var column = new[] {4, 2, 1}.Select(gf5.CreateElement).ToArray();

            // When
            var actual = matrix.AppendColumn(column);

            // Then
            var expected = new FieldElementsMatrix(gf5, new[,] {{0, 3, 4}, {2, 0, 2}, {4, 2, 1}});
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MustAppendColumnDeclaredAsNumbersArray()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var matrix = new FieldElementsMatrix(gf5, new[,] {{0, 3}, {2, 0}, {4, 2}});
            var column = new[] {2, 0, 3};

            // When
            var actual = matrix.AppendColumn(column);

            // Then
            var expected = new FieldElementsMatrix(gf5, new[,] {{0, 3, 2}, {2, 0, 0}, {4, 2, 3}});
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(AppendRowParametersValidationTestCases))]
        public void MustValidateParametersDuringRowAppending(AppendRowParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => testCase.Matrix.AppendRow(testCase.Row));
        }

        [Fact]
        public void MustAppendRowDeclaredAsMatrix()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var matrix = new FieldElementsMatrix(gf5, new[,] {{0, 3, 2}, {4, 2, 0}});
            var row = new FieldElementsMatrix(gf5, new[,] {{1, 3, 4}});

            // When
            var actual = matrix.AppendRow(row);

            // Then
            var expected = new FieldElementsMatrix(gf5, new[,] {{0, 3, 2}, {4, 2, 0}, {1, 3, 4}});
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MustAppendRowDeclaredAsFieldElementsArray()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var matrix = new FieldElementsMatrix(gf5, new[,] {{4, 2, 1}, {3, 1, 2}});
            var row = new[] {4, 1, 0}.Select(gf5.CreateElement).ToArray();

            // When
            var actual = matrix.AppendRow(row);

            // Then
            var expected = new FieldElementsMatrix(gf5, new[,] {{4, 2, 1}, {3, 1, 2}, {4, 1, 0}});
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void MustAppendRowDeclaredAsNumbersArray()
        {
            // Given
            var gf5 = GaloisField.Create(5);
            var matrix = new FieldElementsMatrix(gf5, new[,] {{3, 1, 1}, {4, 2, 0}});
            var row = new[] {0, 0, 3};

            // When
            var actual = matrix.AppendRow(row);

            // Then
            var expected = new FieldElementsMatrix(gf5, new[,] {{3, 1, 1}, {4, 2, 0}, {0, 0, 3}});
            Assert.Equal(expected, actual);
        }
    }
}