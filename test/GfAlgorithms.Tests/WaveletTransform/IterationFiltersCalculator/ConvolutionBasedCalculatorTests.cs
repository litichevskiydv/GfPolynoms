namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
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
        [UsedImplicitly]
        public static TheoryData<ComplementaryIterationFiltersVectorsCalculationTestCase> ComplementaryIterationFiltersVectorsCalculationTestCases;

        static ConvolutionBasedCalculatorTests()
        {
            var gf2 = GaloisField.Create(2);
            var hSource1 = gf2.CreateElementsVector(1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1);
            var gSource1 = gf2.CreateElementsVector(1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1);
            var hSource2 = gf2.CreateElementsVector(1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0);
            var gSource2 = gf2.CreateElementsVector(1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0);


            OrthogonalIterationFiltersVectorsCalculationTestCases
                = new OrthogonalIterationFiltersVectorsCalculationTheoryData(
                    new OrthogonalIterationFiltersVectorsCalculationTestCase(2, hSource1, gSource1)
                );
            ComplementaryIterationFiltersVectorsCalculationTestCases
                = new ComplementaryIterationFiltersVectorsCalculationTheoryData(
                    new ComplementaryIterationFiltersVectorsCalculationTestCase(2, hSource2, gSource2),
                    new ComplementaryIterationFiltersVectorsCalculationTestCase(3, hSource2, gSource2)
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

        [Theory]
        [MemberData(nameof(ComplementaryIterationFiltersVectorsCalculationTestCases))]
        public void MustCalculateComplementaryIterationFiltersVectors(ComplementaryIterationFiltersVectorsCalculationTestCase testCase)
            => TestComplementaryIterationFiltersVectorsCalculation(testCase);
    }
}