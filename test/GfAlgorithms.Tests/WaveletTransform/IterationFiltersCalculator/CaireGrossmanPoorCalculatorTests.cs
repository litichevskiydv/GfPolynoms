namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using System.Linq;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases.WaveletTransform;
    using Xunit;


    public class CaireGrossmanPoorCalculatorTests
    {
        private readonly CaireGrossmanPoorCalculator _iterationFiltersCalculator;

        [UsedImplicitly]
        public static TheoryData<IterationFilterVectorCalculationTestCase> IterationFilterVectorCalculationTestCases;

        static CaireGrossmanPoorCalculatorTests()
        {
            var gf17 = GaloisField.Create(17);

            var hSource1 = Enumerable.Repeat(gf17.CreateElement(16), 2).Concat(Enumerable.Repeat(gf17.Zero(), 14)).ToArray();
            var gSource1 = new[] {gf17.One(), gf17.CreateElement(16)}.Concat(Enumerable.Repeat(gf17.Zero(), 14)).ToArray();
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
    }
}