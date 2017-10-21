namespace AppliedAlgebra.WaveletCodesTools.GeneratingPolynomialsBuilder
{
    using GfPolynoms;

    /// <summary>
    /// Contract of generating polynomials builder for wavelet code
    /// </summary>
    public interface IGeneratingPolynomialsBuilder
    {
        /// <summary>
        /// Method for building generating polynomial for the wavelet code
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="d">Code distance</param>
        /// <param name="h">Filter from which generating polynomial will be built</param>
        /// <returns>Built generating polynomial</returns>
        Polynomial Build(int n, int d, Polynomial h);
    }
}