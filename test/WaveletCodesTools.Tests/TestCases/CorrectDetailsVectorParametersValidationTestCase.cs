namespace AppliedAlgebra.WaveletCodesTools.Tests.TestCases
{
    using GfAlgorithms.Matrices;

    public class CorrectDetailsVectorParametersValidationTestCase
    {
        public (FieldElementsMatrix hMatrix, FieldElementsMatrix gMatrix) IterationMatrices { get; set; }

        public FieldElementsMatrix ApproximationVector { get; set; }

        public FieldElementsMatrix DetailsVector { get; set; }

        public int CorrectableComponentsCount { get; set; }

        public int RequiredZerosNumber { get; set; }
    }
}