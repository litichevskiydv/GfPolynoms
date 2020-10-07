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
        /// <param name="informationWord">Information word</param>
        /// <returns>Codeword</returns>
        FieldElement[] Encode(int codewordLength, FieldElement[] informationWord);
    }
}