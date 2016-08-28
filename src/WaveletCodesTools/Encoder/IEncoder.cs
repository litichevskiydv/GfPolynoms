namespace WaveletCodesTools.Encoder
{
    using System;
    using GfPolynoms;

    public interface IEncoder
    {
        Tuple<FieldElement, FieldElement>[] Encode(int n, Polynomial generatingPolynomial, Polynomial informationPolynomial,
            Polynomial modularPolynomial = null);
    }
}