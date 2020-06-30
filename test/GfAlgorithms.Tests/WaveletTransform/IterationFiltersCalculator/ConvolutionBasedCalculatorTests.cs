namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using System.Linq;
    using GfAlgorithms.WaveletTransform.IterationFiltersCalculator;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
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
            var gf2 = GaloisField.Create(2);
            var hSource = new[] { 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1 }.Select(x => gf2.CreateElement(x)).ToArray();
            var gSource = new[] { 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1 }.Select(x => gf2.CreateElement(x)).ToArray();

            OrthogonalIterationFiltersVectorsCalculationTestCases
                = new OrthogonalIterationFiltersVectorsCalculationTheoryData(
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(2, hSource, gSource)
                );
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