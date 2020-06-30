namespace AppliedAlgebra.GfAlgorithms.Tests.TestCases.WaveletTransform
{
    using GfPolynoms;

    public class ComplementaryIterationFiltersVectorsCalculationTestCase
    {
        public int IterationNumber { get; }

        public FieldElement[] SourceFilterH { get; }

        public FieldElement[] SourceFilterG { get; }

        public ComplementaryIterationFiltersVectorsCalculationTestCase(
            int iterationNumber,
            FieldElement[] sourceFilterH,
            FieldElement[] sourceFilterG
        )
        {
            IterationNumber = iterationNumber;
            SourceFilterH = sourceFilterH;
            SourceFilterG = sourceFilterG;
        }
    }
}