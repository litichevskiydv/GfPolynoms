namespace AppliedAlgebra.WaveletCodesTools.Encoding
{
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Implementation of the wavelet code's encoder contract
    /// </summary>
    public class Encoder : IEncoder
    {
        /// <summary>
        /// Method for computing wavelet code's codeword for information polynomial <paramref name="informationPolynomial"/>
        /// </summary>
        /// <param name="n">Codeword length</param>
        /// <param name="generatingPolynomial">Generating polynomial of the wavelet code</param>
        /// <param name="informationPolynomial">Information polynomial</param>
        /// <param name="modularPolynomial">Modular polynomial for wavelet code encoding scheme</param>
        /// <returns>Computed codeword</returns>
        public (FieldElement xValue, FieldElement yValue)[] Encode(
            int n, 
            Polynomial generatingPolynomial, 
            Polynomial informationPolynomial,
            Polynomial modularPolynomial = null
        )
        {
            var field = generatingPolynomial.Field;
            var m = modularPolynomial;

            if (m == null)
            {
                m = new Polynomial(field, 1).RightShift(n);
                m[0] = field.InverseForAddition(1);
            }
            var codeWordPolynomial = informationPolynomial.RaiseVariableDegree(2) * generatingPolynomial % m;

            var i = 0;
            var codeword = new (FieldElement xValue, FieldElement yValue)[n];
            for (; i <= codeWordPolynomial.Degree; i++)
                codeword[i] = (field.CreateElement(field.PowGeneratingElement(i)), field.CreateElement(codeWordPolynomial[i]));
            for (; i < n; i++)
                codeword[i] = (field.CreateElement(field.PowGeneratingElement(i)), field.Zero());

            return codeword;
        }
    }
}