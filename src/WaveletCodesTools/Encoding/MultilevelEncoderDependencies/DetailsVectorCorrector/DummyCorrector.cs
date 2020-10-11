namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector
{
    using System;
    using GfAlgorithms.Matrices;
    using GfPolynoms;

    /// <summary>
    /// Dummy corrector implementation that returns original details vector
    /// </summary>
    public class DummyCorrector : IDetailsVectorCorrector
    {
        public FieldElement[] CorrectDetailsVector(
            (FieldElementsMatrix hMatrix, FieldElementsMatrix gMatrix) iterationMatrices,
            FieldElement[] approximationVector,
            FieldElement[] detailsVector,
            int requiredZerosNumber
        )
        {
            if(detailsVector == null)
                throw new ArgumentNullException(nameof(detailsVector));
            if(requiredZerosNumber < 0)
                throw new ArgumentException($"{requiredZerosNumber} must not be negative");

            if(requiredZerosNumber > 0)
                throw new NotSupportedException($"Positive {requiredZerosNumber} is not supported");
            return detailsVector;
        }
    }
}