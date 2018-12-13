namespace AppliedAlgebra.RsCodesTools.Decoding.ListDecoder
{
    using System;
    using GfPolynoms;

    /// <summary>
    /// Contract for Reed-Solomon code list decoder
    /// </summary>
    public interface IListDecoder
    {
        /// <summary>
        /// Method for performing list decoding of Reed-Solomon code codeword
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="k">Information word length</param>
        /// <param name="decodedCodeword">Received codeword for decoding</param>
        /// <param name="minCorrectValuesCount">Minimum number of valid values</param>
        /// <returns>Decoding result</returns>
        Polynomial[] Decode(int n, int k, Tuple<FieldElement, FieldElement>[] decodedCodeword, int minCorrectValuesCount);
    }
}