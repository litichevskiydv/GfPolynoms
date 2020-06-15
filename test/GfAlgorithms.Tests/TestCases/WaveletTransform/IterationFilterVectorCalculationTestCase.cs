namespace AppliedAlgebra.GfAlgorithms.Tests.TestCases.WaveletTransform
{
    using GfPolynoms;

    public class IterationFilterVectorCalculationTestCase
    {
        public int IterationNumber { get; set; } 
        
        public FieldElement[] SourceFilter { get; set; }

        public FieldElement[] ExpectedIterationFilter { get; set; }
    }
}