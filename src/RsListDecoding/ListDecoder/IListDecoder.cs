namespace RsListDecoding.ListDecoder
{
    using System;
    using GfPolynoms;

    public interface IListDecoder
    {
        Polynomial[] Decode(int n, int k, int minCorrectValuesCount, Tuple<FieldElement, FieldElement>[] decodedCodeword);
    }
}