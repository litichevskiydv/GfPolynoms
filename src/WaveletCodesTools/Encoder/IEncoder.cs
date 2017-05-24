namespace AppliedAlgebra.WaveletCodesTools.Encoder
{
    using System;
    using GfPolynoms;

    /// <summary>
    /// Contract for the wavelet code encoder
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// Method for computing wavelet code's codeword for information polynomial <paramref name="informationPolynomial"/>
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="generatingPolynomial">Generating polynomial of the wavelet code</param>
        /// <param name="informationPolynomial">Information polynomial</param>
        /// <param name="modularPolynomial">Modular polynomial for wavelet code encoding scheme</param>
        /// <returns>Computed codeword</returns>
        Tuple<FieldElement, FieldElement>[] Encode(int n, Polynomial generatingPolynomial, Polynomial informationPolynomial,
            Polynomial modularPolynomial = null);
    }
}