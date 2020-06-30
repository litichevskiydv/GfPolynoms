namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using JetBrains.Annotations;
    using TestCases.WaveletTransform;
    using Xunit;

    [UsedImplicitly]
    public class ConvolutionBasedCalculatorTests : IterationFiltersCalculatorTestsBase
    {
        [UsedImplicitly]
        public static TheoryData<OrthogonalIterationFiltersVectorsCalculationTestCase> OrthogonalIterationFiltersVectorsCalculationTestCases;

        static ConvolutionBasedCalculatorTests()
        {
            OrthogonalIterationFiltersVectorsCalculationTestCases = new OrthogonalIterationFiltersVectorsCalculationTheoryData();
        }

        public ConvolutionBasedCalculatorTests() : base(new ConvolutionBasedCalculator())
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
    }
}