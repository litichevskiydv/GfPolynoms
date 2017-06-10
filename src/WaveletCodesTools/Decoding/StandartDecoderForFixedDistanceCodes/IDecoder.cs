namespace AppliedAlgebra.WaveletCodesTools.Decoding.StandartDecoderForFixedDistanceCodes
{
    using System;
    using GfPolynoms;

    /// <summary>
    /// Contract for wavelet code standart decoder
    /// </summary>
    public interface IDecoder
    {
        /// <summary>
        /// Method for performing list decoding of the wavelet code codeword
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="k">Information word length</param>
        /// <param name="d">Code distance</param>
        /// <param name="generatingPolynomial">Generating polynomial of the wavelet code</param>
        /// <param name="decodedCodeword">Recived codeword for decoding</param>
        /// <param name="errorsCount">Number of errors in received codeword</param>
        /// <returns>Decoding result</returns>
        Polynomial Decode(int n, int k, int d, Polynomial generatingPolynomial,
            Tuple<FieldElement, FieldElement>[] decodedCodeword, int? errorsCount = null);
    }
}