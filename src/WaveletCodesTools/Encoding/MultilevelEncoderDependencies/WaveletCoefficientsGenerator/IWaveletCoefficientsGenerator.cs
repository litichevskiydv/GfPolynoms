namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator
{
    using GfAlgorithms.Matrices;
    using GfPolynoms;

    /// <summary>
    /// Contract for wavelet coefficients vectors generator
    /// </summary>
    public interface IWaveletCoefficientsGenerator
    {
        /// <summary>
        /// Generates approximation vector for last level of wavelet decomposition
        /// </summary>
        /// <param name="informationWord">Information word</param>
        /// <param name="signalLength">Signal length after performing reverse wavelet transform</param>
        /// <param name="levelNumber">Number of the last level of wavelet decomposition</param>
        /// <returns>Approximation vector defined as a column vector</returns>
        FieldElementsMatrix GetApproximationVector(FieldElement[] informationWord, int signalLength, int levelNumber);

        /// <summary>
        /// Generates details vector for wavelet decomposition level <paramref name="levelNumber"/>
        /// </summary>
        /// <param name="informationWord">Information word</param>
        /// <param name="levelNumber">Wavelet decomposition level number</param>
        /// <param name="approximationVector">Approximation vector for level <paramref name="levelNumber"/> defined as a column vector</param>
        /// <returns>Details vector generation result</returns>
        DetailsVectorGenerationResult GetDetailsVector(FieldElement[] informationWord, int levelNumber, FieldElementsMatrix approximationVector);
    }
}