namespace AppliedAlgebra.RsCodesTools.StandartDecoder
{
    using System;
    using GfPolynoms;

    /// <summary>
    /// Contract for Reed-Solomon codes decoder
    /// </summary>
    public interface IStandartDecoder
    {
        /// <summary>
        /// Method for performing decoding of Reed-Solomon code codeword, if unsuccess will throw InformationPolynomialWasNotFoundException
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="k">Information word length</param>
        /// <param name="decodedCodeword">Recived codeword for decoding</param>
        /// <param name="errorsCount">Errors count</param>
        /// <returns>Decoding result</returns>
        Polynomial Decode(int n, int k, Tuple<FieldElement, FieldElement>[] decodedCodeword, int? errorsCount = null);
    }
}