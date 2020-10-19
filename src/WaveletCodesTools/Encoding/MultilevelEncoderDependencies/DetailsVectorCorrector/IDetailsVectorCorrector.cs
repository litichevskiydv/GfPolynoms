namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector
{
    using GfAlgorithms.Matrices;

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
        /// <param name="approximationVector">Approximation vector defined as a column vector</param>
        /// <param name="detailsVector">Details vector defined as a column vector</param>
        /// <param name="correctableComponentsCount">Number of the correctable components in the end of the details vector</param>
        /// <param name="requiredZerosCount">Required number of trailing zeros in the signal</param>
        /// <param name="options">Details vector corrector options</param>
        /// <returns>Corrected details vector defined as a column vector</returns>
        FieldElementsMatrix CorrectDetailsVector(
            (FieldElementsMatrix hMatrix, FieldElementsMatrix gMatrix) iterationMatrices,
            FieldElementsMatrix approximationVector,
            FieldElementsMatrix detailsVector,
            int correctableComponentsCount,
            int requiredZerosCount,
            CorrectorOptions options = null
        );
    }
}