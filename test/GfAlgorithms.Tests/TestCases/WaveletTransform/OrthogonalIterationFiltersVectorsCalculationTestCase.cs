namespace AppliedAlgebra.GfAlgorithms.Tests.TestCases.WaveletTransform
{
    using GfPolynoms;

    public class OrthogonalIterationFiltersVectorsCalculationTestCase
    {
        public int IterationNumber { get; }

        public FieldElement[] SourceFilterH { get; }

        public FieldElement[] SourceFilterG { get; }

        public FieldElement Multiplier { get; }

        public OrthogonalIterationFiltersVectorsCalculationTestCase(
            int iterationNumber,
            FieldElement[] sourceFilterH,
            FieldElement[] sourceFilterG,
            FieldElement multiplier = null
        )
        {
            IterationNumber = iterationNumber;
            SourceFilterH = sourceFilterH;
            SourceFilterG = sourceFilterG;
            Multiplier = multiplier;
        }
    }
}