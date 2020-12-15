namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using System;
    using System.Linq;
    using Extensions;
    using GfAlgorithms.WaveletTransform;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using TestCases.WaveletTransform;
    using Xunit;

    public abstract class IterationFiltersCalculatorTestsBase
    {
        protected readonly IIterationFiltersCalculator IterationFiltersCalculator;

        [UsedImplicitly]
        public static TheoryData<GetIterationFilterVectorParametersValidationTestCase> GetIterationFilterVectorParametersValidationTestCases;

        static IterationFiltersCalculatorTestsBase()
        {
            var gf3 = GaloisField.Create(3);
            var hSource = new[] { 2, 1, 0, 0, 1, 1, 0, 0 }.Select(x => gf3.CreateElement(x)).ToArray();

            GetIterationFilterVectorParametersValidationTestCases
                = new TheoryData<GetIterationFilterVectorParametersValidationTestCase>
                  {
                      new GetIterationFilterVectorParametersValidationTestCase {IterationNumber = -1, SourceFilter = hSource},
                      new GetIterationFilterVectorParametersValidationTestCase {IterationNumber = 0},
                      new GetIterationFilterVectorParametersValidationTestCase {IterationNumber = 0, SourceFilter = new FieldElement[0]},
                      new GetIterationFilterVectorParametersValidationTestCase {IterationNumber = 3, SourceFilter = hSource}
                  };
        }

        protected IterationFiltersCalculatorTestsBase(IIterationFiltersCalculator iterationFiltersCalculator)
        {
            IterationFiltersCalculator = iterationFiltersCalculator;
        }

        [Theory]
        [MemberData(nameof(GetIterationFilterVectorParametersValidationTestCases))]
        public void GetIterationFilterVectorMustValidateParameters(GetIterationFilterVectorParametersValidationTestCase testCase)
        {
            Assert.ThrowsAny<ArgumentException>(() => IterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, testCase.SourceFilter));
        }

        [Fact]
        public void GetIterationFilterPolynomialMustValidateParameters()
        {
            Assert.Throws<ArgumentNullException>(() => IterationFiltersCalculator.GetIterationFilter(0, (Polynomial)null));
        }

        protected void TestOrthogonalIterationFiltersVectorsCalculation(OrthogonalIterationFiltersVectorsCalculationTestCase testCase)
        {
            var iterationFilterH = IterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, testCase.SourceFilterH);
            var iterationFilterG = IterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, testCase.SourceFilterG);

            var filtersBank = new FiltersBankVectors((iterationFilterH, iterationFilterG), (iterationFilterH, iterationFilterG));
            Assert.True(filtersBank.IsSatisfyBiorthogonalCondition(testCase.Multiplier));
            Assert.True(filtersBank.CanPerformPerfectReconstruction(testCase.Multiplier));
        }

        protected void TestOrthogonalIterationFiltersPolynomialsCalculation(OrthogonalIterationFiltersVectorsCalculationTestCase testCase)
        {
            var sourceFilterExpectedDegree = testCase.SourceFilterH.Length - 1;
            var hFilterPolynomial = new Polynomial(testCase.SourceFilterH);
            var gFilterPolynomial = new Polynomial(testCase.SourceFilterG);

            var iterationFilterExpectedDegree = testCase.SourceFilterH.Length / 2.Pow(testCase.IterationNumber) - 1;
            var iterationFilterH = IterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, hFilterPolynomial, sourceFilterExpectedDegree);
            var iterationFilterG = IterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, gFilterPolynomial, sourceFilterExpectedDegree);

            var filtersBankPolynomials = new FiltersBankPolynomials(
                iterationFilterExpectedDegree + 1,
                (iterationFilterH, iterationFilterG),
                (iterationFilterH, iterationFilterG)
            );
            Assert.True(filtersBankPolynomials.CanPerformPerfectReconstruction(testCase.Multiplier));

            var filtersBankVectors = filtersBankPolynomials.ToFiltersBankVectors();
            Assert.True(filtersBankVectors.IsSatisfyBiorthogonalCondition(testCase.Multiplier));
            Assert.True(filtersBankVectors.CanPerformPerfectReconstruction(testCase.Multiplier));
        }

        protected void TestComplementaryIterationFiltersVectorsCalculation(ComplementaryIterationFiltersVectorsCalculationTestCase testCase)
        {
            var iterationFilterH = IterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, testCase.SourceFilterH);
            var iterationFilterG = IterationFiltersCalculator.GetIterationFilter(testCase.IterationNumber, testCase.SourceFilterG);

            var field = testCase.SourceFilterH.GetField();
            var one = new Polynomial(field, 1);
            var modularPolynomial = (one >> (iterationFilterH.Length / 2)) - one;

            var (he, ho) = new Polynomial(iterationFilterH).GetPolyphaseComponents();
            var (ge, go) = new Polynomial(iterationFilterG).GetPolyphaseComponents();
            Assert.Equal(one, (he * go - ho * ge) % modularPolynomial);
        }
    }
}