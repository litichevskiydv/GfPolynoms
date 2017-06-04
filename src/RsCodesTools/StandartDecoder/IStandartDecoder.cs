namespace AppliedAlgebra.RsCodesTools.StandartDecoder
{
    using System;
    using GfPolynoms;

    /// <summary>
    /// Contract for Reed-Solomon code decoder
    /// </summary>
    public interface IStandartDecoder
    {
        /// <summary>
        /// Method for performing decoding of Reed-Solomon code codeword
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="k">Information word length</param>
        /// <param name="decodedCodeword">Recived codeword for decoding</param>
        /// <param name="errorsCount">Errors count</param>
        /// <returns>Decoding result</returns>
        Polynomial Decode(int n, int k, Tuple<FieldElement, FieldElement>[] decodedCodeword, int errorsCount);
    }
}