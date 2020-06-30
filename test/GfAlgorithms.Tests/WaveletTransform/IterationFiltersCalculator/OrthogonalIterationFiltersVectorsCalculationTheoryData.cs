namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using System.Linq;
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

            var hSource1 = new[] {16, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}.Select(x => gf17.CreateElement(x)).ToArray();
            var gSource1 = new[] {1, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}.Select(x => gf17.CreateElement(x)).ToArray();
            var hSource2 = new[] {1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 1}.Select(x => gf2.CreateElement(x)).ToArray();
            var gSource2 = new[] {1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1}.Select(x => gf2.CreateElement(x)).ToArray();
            var hSource3 = new[] {2, 1, 0, 0, 1, 1, 0, 0}.Select(x => gf3.CreateElement(x)).ToArray();
            var gSource3 = new[] {0, 0, 1, 1, 0, 0, 1, 2}.Select(x => gf3.CreateElement(x)).ToArray();

            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(1, hSource1, gSource1, gf17.CreateElement(2)));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(2, hSource1, gSource1, gf17.CreateElement(2)));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(3, hSource1, gSource1, gf17.CreateElement(2)));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(4, hSource1, gSource1, gf17.CreateElement(2)));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(1, hSource2, gSource2));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(1, hSource3, gSource3));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(2, hSource3, gSource3));
            Add(new OrthogonalIterationFiltersVectorsCalculationTestCase(3, hSource3, gSource3));

            foreach (var additionalCase in additionalCases)
                Add(additionalCase);
        }
    }
}