namespace AppliedAlgebra.WaveletCodesTools.Encoding
{
    using GfPolynoms;

    /// <summary>
    /// Contract for the multilevel wavelet code encoder
    /// </summary>
    public interface IMultilevelEncoder
    {
        /// <summary>
        /// Computes wavelet code codeword for information word <paramref name="informationWord"/>
        /// </summary>
        /// <param name="codewordLength">Codeword length</param>
        /// <param name="levelsCount">Wavelet decomposition levels count</param>
        /// <param name="synthesisFilters">Synthesis filters pair</param>
        /// <param name="informationWord">Information word</param>
        /// <returns>Codeword</returns>
        FieldElement[] Encode(
            int codewordLength,
            int levelsCount,
            (FieldElement[] h, FieldElement[] g) synthesisFilters,
            FieldElement[] informationWord
        );
    }
}