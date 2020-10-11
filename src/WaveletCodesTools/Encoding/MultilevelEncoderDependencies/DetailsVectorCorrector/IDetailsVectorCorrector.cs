namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector
{
    using GfAlgorithms.Matrices;
    using GfPolynoms;

    /// <summary>
    /// Contract for details vector corrector that changes details vector components
    /// in order to provide required number of trailing zeros in the signal
    /// </summary>
    public interface IDetailsVectorCorrector
    {
        /// <summary>
        /// Corrects details vector in order to provide required number of trailing zeros in the signal
        /// </summary>
        /// <param name="iterationMatrices">Matrices for performing signal reconstruction</param>
        /// <param name="approximationVector">Approximation vector</param>
        /// <param name="detailsVector">Details vector</param>
        /// <param name="requiredZerosNumber">Required number of trailing zeros in the signal</param>
        /// <returns>Corrected details vector</returns>
        FieldElement[] CorrectDetailsVector(
            (FieldElementsMatrix hMatrix, FieldElementsMatrix gMatrix) iterationMatrices,
            FieldElement[] approximationVector,
            FieldElement[] detailsVector,
            int requiredZerosNumber
        );
    }
}