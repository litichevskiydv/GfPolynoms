namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.WaveletCoefficientsGenerator
{
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
        /// <returns>Approximation vector</returns>
        FieldElement[] GetApproximationVector(FieldElement[] informationWord, int signalLength, int levelNumber);

        /// <summary>
        /// Generates details vector for wavelet decomposition level <paramref name="levelNumber"/>
        /// </summary>
        /// <param name="informationWord">Information word</param>
        /// <param name="levelNumber">Wavelet decomposition level number</param>
        /// <param name="approximationVector">Approximation vector for level <paramref name="levelNumber"/></param>
        /// <returns>Details vector</returns>
        FieldElement[] GetDetailsVector(FieldElement[] informationWord, int levelNumber, FieldElement[] approximationVector);
    }
}