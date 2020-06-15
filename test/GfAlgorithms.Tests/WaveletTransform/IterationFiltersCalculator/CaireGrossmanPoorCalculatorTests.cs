namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using System.Linq;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using Matrices;
    using TestCases.WaveletTransform;
    using Xunit;


    public class CaireGrossmanPoorCalculatorTests
    {
        private readonly CaireGrossmanPoorCalculator _iterationFiltersCalculator;

        [UsedImplicitly]
        public static TheoryData<IterationFilterVectorCalculationTestCase> IterationFilterVectorCalculationTestCases;
        [UsedImplicitly]
        public static TheoryData<OrthogonalIterationFiltersVectorsCalculationTestCase> OrthogonalIterationFiltersVectorsCalculationTestCases;

        static CaireGrossmanPoorCalculatorTests()
        {
            var gf17 = GaloisField.Create(17);

            var hSource1 = Enumerable.Repeat(gf17.CreateElement(16), 2).Concat(Enumerable.Repeat(gf17.Zero(), 14)).ToArray();
            var gSource1 = new[] { gf17.One(), gf17.CreateElement(16) }.Concat(Enumerable.Repeat(gf17.Zero(), 14)).ToArray();

            IterationFilterVectorCalculationTestCases
                = new TheoryData<IterationFilterVectorCalculationTestCase>
                  {
                      new IterationFilterVectorCalculationTestCase
                      {
                          IterationNumber = 1,
                          SourceFilter = hSource1,
                          ExpectedIterationFilter = hSource1

                      },
                      new IterationFilterVectorCalculationTestCase
                      {
                          IterationNumber = 1,
                          SourceFilter = gSource1,
                          ExpectedIterationFilter = gSource1

                      },
                      new IterationFilterVectorCalculationTestCase
                      {
                          IterationNumber = 2,
                          SourceFilter = hSource1,
                          ExpectedIterationFilter = Enumerable.Repeat(gf17.CreateElement(16), 2).Concat(Enumerable.Repeat(gf17.Zero(), 6)).ToArray()

                      },
                      new IterationFilterVectorCalculationTestCase
                      {
                          IterationNumber = 2,
                          SourceFilter = gSource1,
                          ExpectedIterationFilter = new[] {gf17.One(), gf17.CreateElement(16)}.Concat(Enumerable.Repeat(gf17.Zero(), 6)).ToArray()

                      },
                      new IterationFilterVectorCalculationTestCase
                      {
                          IterationNumber = 3,
                          SourceFilter = hSource1,
                          ExpectedIterationFilter = Enumerable.Repeat(gf17.CreateElement(16), 2).Concat(Enumerable.Repeat(gf17.Zero(), 2)).ToArray()

                      },
                      new IterationFilterVectorCalculationTestCase
                      {
                          IterationNumber = 3,
                          SourceFilter = gSource1,
                          ExpectedIterationFilter = new[] {gf17.One(), gf17.CreateElement(16)}.Concat(Enumerable.Repeat(gf17.Zero(), 2)).ToArray()

                      },
                      new IterationFilterVectorCalculationTestCase
                      {
                          IterationNumber = 4,
                          SourceFilter = hSource1,
                          ExpectedIterationFilter = new[] {gf17.CreateElement(16), gf17.CreateElement(16)}

                      },
                      new IterationFilterVectorCalculationTestCase
                      {
                          IterationNumber = 4,
                          SourceFilter = gSource1,
                          ExpectedIterationFilter = new[] {gf17.One(), gf17.CreateElement(16)}

                      }
                  };

            OrthogonalIterationFiltersVectorsCalculationTestCases
                = new TheoryData<OrthogonalIterationFiltersVectorsCalculationTestCase>
                  {
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(1, hSource1, gSource1, gf17.CreateElement(2)),
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(2, hSource1, gSource1, gf17.CreateElement(2)),
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(3, hSource1, gSource1, gf17.CreateElement(2)),
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(4, hSource1, gSource1, gf17.CreateElement(2)),
                  };
        }

        public CaireGrossmanPoorCalculatorTests()
        {
            _iterationFiltersCalculator = new CaireGrossmanPoorCalculator();
        }

        [Theory]
        [MemberData(nameof(IterationFilterVectorCalculationTestCases))]
        public void MustCalculateIterationFilterVector(IterationFilterVectorCalculationTestCase testCase)
        {
            // When
            var actualIterationFilter = _iterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, testCase.SourceFilter);

            // Then
            Assert.Equal(testCase.ExpectedIterationFilter, actualIterationFilter);
        }

        [Theory]
        [MemberData(nameof(OrthogonalIterationFiltersVectorsCalculationTestCases))]
        public void MustCalculateOrthogonalIterationFiltersVectors(OrthogonalIterationFiltersVectorsCalculationTestCase testCase)
        {
            // Given
            var field = testCase.SourceFilterH[0].Field;
            var multiplier = testCase.Multiplier ?? field.One();

            // When
            var iterationFilterH = _iterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, testCase.SourceFilterH);
            var iterationFilterG = _iterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, testCase.SourceFilterG);

            // Then
            var hMatrix = FieldElementsMatrix.DoubleCirculantMatrix(iterationFilterH);
            var hMatrixTransposed = FieldElementsMatrix.Transpose(hMatrix);
            var gMatrix = FieldElementsMatrix.DoubleCirculantMatrix(iterationFilterG);
            var gMatrixTransposed = FieldElementsMatrix.Transpose(gMatrix);


            var fullSizeIdentityMatrix = FieldElementsMatrix.IdentityMatrix(hMatrix.Field, iterationFilterH.Length);
            Assert.Equal(multiplier * fullSizeIdentityMatrix, hMatrixTransposed * hMatrix + gMatrixTransposed * gMatrix);

            var halfSizeIdentityMatrix = FieldElementsMatrix.IdentityMatrix(hMatrix.Field, iterationFilterH.Length / 2);
            var halfSizeZeroMatrix = FieldElementsMatrix.ZeroMatrix(hMatrix.Field, iterationFilterH.Length / 2);
            Assert.Equal(multiplier * halfSizeIdentityMatrix, hMatrix * hMatrixTransposed);
            Assert.Equal(multiplier * halfSizeIdentityMatrix, gMatrix * gMatrixTransposed);
            Assert.Equal(halfSizeZeroMatrix, hMatrix * gMatrixTransposed);
            Assert.Equal(halfSizeZeroMatrix, gMatrix * hMatrixTransposed);
        }
    }
}