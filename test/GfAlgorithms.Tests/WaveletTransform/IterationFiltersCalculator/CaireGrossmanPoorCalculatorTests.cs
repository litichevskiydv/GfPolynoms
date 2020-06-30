namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using JetBrains.Annotations;
    using TestCases.WaveletTransform;
    using Xunit;

    [UsedImplicitly]
    public class CaireGrossmanPoorCalculatorTests : IterationFiltersCalculatorTestsBase
    {
        [UsedImplicitly]
        public static TheoryData<OrthogonalIterationFiltersVectorsCalculationTestCase> OrthogonalIterationFiltersVectorsCalculationTestCases;
        [UsedImplicitly]
        public static TheoryData<ComplementaryIterationFiltersVectorsCalculationTestCase> ComplementaryIterationFiltersVectorsCalculationTestCases;

        static CaireGrossmanPoorCalculatorTests()
        {
            OrthogonalIterationFiltersVectorsCalculationTestCases = new OrthogonalIterationFiltersVectorsCalculationTheoryData();
            ComplementaryIterationFiltersVectorsCalculationTestCases = new ComplementaryIterationFiltersVectorsCalculationTheoryData();
        }

        public CaireGrossmanPoorCalculatorTests() : base(new CaireGrossmanPoorCalculator())
        {
        }

        [Theory]
        [MemberData(nameof(OrthogonalIterationFiltersVectorsCalculationTestCases))]
        public void MustCalculateOrthogonalIterationFiltersVectors(OrthogonalIterationFiltersVectorsCalculationTestCase testCase)
            => TestOrthogonalIterationFiltersVectorsCalculation(testCase);

        [Theory]
        [MemberData(nameof(OrthogonalIterationFiltersVectorsCalculationTestCases))]
        public void MustCalculateOrthogonalIterationFiltersPolynomials(OrthogonalIterationFiltersVectorsCalculationTestCase testCase)
            => TestOrthogonalIterationFiltersPolynomialsCalculation(testCase);

        [Theory]
        [MemberData(nameof(ComplementaryIterationFiltersVectorsCalculationTestCases))]
        public void MustCalculateComplementaryIterationFiltersVectors(ComplementaryIterationFiltersVectorsCalculationTestCase testCase)
            => TestComplementaryIterationFiltersVectorsCalculation(testCase);
    }
}