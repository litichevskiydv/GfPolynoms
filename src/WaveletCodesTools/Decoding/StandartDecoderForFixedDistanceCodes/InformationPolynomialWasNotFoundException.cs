namespace AppliedAlgebra.WaveletCodesTools.Decoding.StandartDecoderForFixedDistanceCodes
{
    using System;
    /// <summary>
    /// Exception for indication that information polynomial cannot be found
    /// </summary>
    public class InformationPolynomialWasNotFoundException : InvalidOperationException
    {
        public InformationPolynomialWasNotFoundException()
        {
        }

        public InformationPolynomialWasNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}