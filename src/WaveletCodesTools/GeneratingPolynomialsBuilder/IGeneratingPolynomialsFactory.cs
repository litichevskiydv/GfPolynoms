namespace AppliedAlgebra.WaveletCodesTools.GeneratingPolynomialsBuilder
{
    using GfPolynoms;

    /// <summary>
    /// Generating polynomials factory
    /// </summary>
    public interface IGeneratingPolynomialsFactory
    {
        /// <summary>
        /// Creates generating polynomial with expected degree
        /// </summary>
        /// <param name="h">Synthesis filter h</param>
        /// <param name="g">Synthesis filter g</param>
        /// <param name="expectedDegree">Generating polynomial expected degree</param>
        /// <returns>Built generating polynomial</returns>
        Polynomial Create(Polynomial h, Polynomial g, int expectedDegree);
    }
}