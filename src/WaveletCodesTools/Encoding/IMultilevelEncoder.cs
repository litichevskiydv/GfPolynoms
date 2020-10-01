namespace AppliedAlgebra.WaveletCodesTools.Encoding
{
    using GfPolynoms;

    public interface IMultilevelEncoder
    {
        FieldElement[] Encode(
            int codewordLength,
            (FieldElement[] h, FieldElement[] g) synthesisFilters,
            FieldElement[] informationWord
        );
    }
}