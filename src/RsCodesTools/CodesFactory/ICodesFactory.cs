namespace AppliedAlgebra.RsCodesTools.CodesFactory
{
    using CodesAbstractions;
    using GfPolynoms.GaloisFields;

    public interface ICodesFactory
    {
        ICode Create(GaloisField field, int codewordLength, int informationWordLength);
    }
}