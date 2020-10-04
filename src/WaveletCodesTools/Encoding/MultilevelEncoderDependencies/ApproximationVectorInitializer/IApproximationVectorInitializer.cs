namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.ApproximationVectorInitializer
{
    using GfPolynoms;

    /// <summary>
    /// Contract for approximation vector initializer for last level of wavelet decomposition
    /// </summary>
    public interface IApproximationVectorInitializer
    {
        /// <summary>
        /// Generates approximation vector for last level of wavelet decomposition
        /// </summary>
        /// <param name="informationWord">Information word</param>
        /// <param name="signalLength">Signal length after performing reverse wavelet transform</param>
        /// <param name="levelNumber">Number of the last level of wavelet decomposition</param>
        /// <returns>Approximation vector</returns>
        FieldElement[] GetApproximationVector(FieldElement[] informationWord, int signalLength, int levelNumber);
    }
}