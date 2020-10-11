namespace AppliedAlgebra.WaveletCodesTools.Tests.TestCases
{
    using GfAlgorithms.Matrices;
    using GfPolynoms;

    public class CorrectDetailsVectorParametersValidationTestCase
    {
        public (FieldElementsMatrix hMatrix, FieldElementsMatrix gMatrix) IterationMatrices { get; set; }

        public FieldElement[] ApproximationVector { get; set; }

        public FieldElement[] DetailsVector { get; set; }

        public int RequiredZerosNumber { get; set; }
    }
}