namespace AppliedAlgebra.WaveletCodesTools.Encoder
{
    using System;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public class Encoder : IEncoder
    {
        public Tuple<FieldElement, FieldElement>[] Encode(int n, Polynomial generatingPolynomial, Polynomial informationPolynomial,
            Polynomial modularPolynomial = null)
        {
            var field = generatingPolynomial.Field;
            var m = modularPolynomial;

            if (m == null)
            {
                m = new Polynomial(field, 1).RightShift(n);
                m[0] = field.InverseForAddition(1);
            }
            var codeWordPolynomial = (informationPolynomial.RaiseVariableDegree(2) * generatingPolynomial) % m;

            var i = 0;
            var codeword = new Tuple<FieldElement, FieldElement>[n];
            for (; i <= codeWordPolynomial.Degree; i++)
                codeword[i] = Tuple.Create(field.CreateElement(field.GetGeneratingElementPower(i)), field.CreateElement(codeWordPolynomial[i]));
            for (; i < n; i++)
                codeword[i] = Tuple.Create(field.CreateElement(field.GetGeneratingElementPower(i)), field.Zero());

            return codeword;
        }
    }
}