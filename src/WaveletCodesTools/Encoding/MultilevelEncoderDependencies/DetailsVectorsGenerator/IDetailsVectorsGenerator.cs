namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorsGenerator
{
    using GfPolynoms;

    /// <summary>
    /// Contract for details vectors generator
    /// </summary>
    public interface IDetailsVectorsGenerator
    {
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