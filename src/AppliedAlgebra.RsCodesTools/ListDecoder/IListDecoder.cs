namespace AppliedAlgebra.RsCodesTools.ListDecoder
{
    using System;
    using GfPolynoms;

    public interface IListDecoder
    {
        Polynomial[] Decode(int n, int k, Tuple<FieldElement, FieldElement>[] decodedCodeword, int minCorrectValuesCount);
    }
}