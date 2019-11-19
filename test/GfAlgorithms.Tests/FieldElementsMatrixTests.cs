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

        #endregion

        [UsedImplicitly]
        public static TheoryData<FromInitializerConstructorParametersValidationTestCase> FromInitializerConstructorParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<FromNumbersArrayConstructorParametersValidationTestCase> FromNumbersArrayConstructorParametersValidationTestCases;
        [UsedImplicitly]
        public static TheoryData<FieldElement[,]> FromFieldElementsArrayConstructorParametersValidationTestCases;

        static FieldElementsMatrixTests()
        {
            var gf2 = new PrimeOrderField(2);
            var gf3 = new PrimeOrderField(3);

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
    }
}