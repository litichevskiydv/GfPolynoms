namespace AppliedAlgebra.WaveletCodesTools.FixedDistanceCodesFactory
{
    using CodesAbstractions;
    using GfPolynoms;

    public interface IFixedDistanceCodesFactory
    {
        ICode Create(Polynomial generatingPolynomial);

        ICode Create(Polynomial h, int requiredCodeDistance);
    }
}