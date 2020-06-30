namespace AppliedAlgebra.GfAlgorithms.Tests.WaveletTransform.IterationFiltersCalculator
{
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using TestCases.WaveletTransform;
    using Xunit;

    public class ComplementaryIterationFiltersVectorsCalculationTheoryData : TheoryData<ComplementaryIterationFiltersVectorsCalculationTestCase>
    {
        public ComplementaryIterationFiltersVectorsCalculationTheoryData(params ComplementaryIterationFiltersVectorsCalculationTestCase[] additionalCases)
        {
            var gf2 = GaloisField.Create(2);
            var gf9 = GaloisField.Create(9);
            var gf17 = GaloisField.Create(17);

            var hSource1 = gf9.CreateElementsVector(2, 7, 5, 1, 8, 3, 2, 5);
            var gSource1 = gf9.CreateElementsVector(1, 4, 4, 1, 4, 2, 0, 0);
            var hSource2 = gf17.CreateElementsVector(10, 16, 5, 0, 0, 0, 0, 16, 0, 0, 0, 0, 0, 0, 0, 0);
            var gSource2 = gf17.CreateElementsVector(12, 4, 0, 15, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            var hSource3 = gf2.CreateElementsVector(1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0);
            var gSource3 = gf2.CreateElementsVector(1, 0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0);

            Add(new ComplementaryIterationFiltersVectorsCalculationTestCase(1, hSource1, gSource1));
            Add(new ComplementaryIterationFiltersVectorsCalculationTestCase(2, hSource1, gSource1));
            Add(new ComplementaryIterationFiltersVectorsCalculationTestCase(1, hSource2, gSource2));
            Add(new ComplementaryIterationFiltersVectorsCalculationTestCase(2, hSource2, gSource2));
            Add(new ComplementaryIterationFiltersVectorsCalculationTestCase(3, hSource2, gSource2));
            Add(new ComplementaryIterationFiltersVectorsCalculationTestCase(1, hSource3, gSource3));

            foreach (var additionalCase in additionalCases) 
                Add(additionalCase);
        }
    }
}