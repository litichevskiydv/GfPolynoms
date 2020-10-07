namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using TestCases.WaveletTransform;
    using Xunit;

    public class OrthogonalIterationFiltersVectorsCalculationTheoryData : TheoryData<OrthogonalIterationFiltersVectorsCalculationTestCase>
    {
        public OrthogonalIterationFiltersVectorsCalculationTheoryData(params OrthogonalIterationFiltersVectorsCalculationTestCase[] additionalCases)
        {
            var gf2 = GaloisField.Create(2);
            var gf3 = GaloisField.Create(3);
            var gf17 = GaloisField.Create(17);

            var hSource1 = gf17.CreateElementsVector(16, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            var gSource1 = gf17.CreateElementsVector(1, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            var hSource2 = gf2.CreateElementsVector(1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1);
            var gSource2 = gf2.CreateElementsVector(1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1);
            var hSource3 = gf3.CreateElementsVector(2, 1, 0, 0, 1, 1, 0, 0);
            var gSource3 = gf3.CreateElementsVector(0, 0, 1, 1, 0, 0, 1, 2);

            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(0, hSource1, gSource1, gf17.CreateElement(2)));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(1, hSource1, gSource1, gf17.CreateElement(2)));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(2, hSource1, gSource1, gf17.CreateElement(2)));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(3, hSource1, gSource1, gf17.CreateElement(2)));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(0, hSource2, gSource2));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(0, hSource3, gSource3));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(1, hSource3, gSource3));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(2, hSource3, gSource3));

            foreach (var additionalCase in additionalCases)
                Add(additionalCase);
        }
    }
}