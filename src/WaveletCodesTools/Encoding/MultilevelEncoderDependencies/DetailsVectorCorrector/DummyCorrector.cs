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
        /// <inheritdoc/>
        public FieldElementsMatrix CorrectDetailsVector(
            (FieldElementsMatrix hMatrix, FieldElementsMatrix gMatrix) iterationMatrices,
            FieldElementsMatrix approximationVector,
            FieldElementsMatrix detailsVector,
            int correctableComponentsCount,
            int requiredZerosCount
        )
        {
            if(detailsVector == null)
                throw new ArgumentNullException(nameof(detailsVector));
            if(requiredZerosCount < 0)
                throw new ArgumentException($"{requiredZerosCount} must not be negative");

            if(requiredZerosCount > 0)
                throw new NotSupportedException($"Positive {requiredZerosCount} is not supported");
            return detailsVector;
        }
    }
}