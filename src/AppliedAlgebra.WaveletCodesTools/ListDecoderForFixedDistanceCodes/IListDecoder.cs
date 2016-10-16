namespace AppliedAlgebra.WaveletCodesTools.ListDecoderForFixedDistanceCodes
{
    using System;
    using GfPolynoms;

    public interface IListDecoder
    {
        Polynomial[] Decode(int n, int k, int d, Polynomial generatingPolynomial,
            Tuple<FieldElement, FieldElement>[] decodedCodewordint, int minCorrectValuesCount);
    }
}