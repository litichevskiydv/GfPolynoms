namespace RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator
{
    using GfAlgorithms.BiVariablePolynomials;
    using GfPolynoms;

    public interface IInterpolationPolynomialFactorizator
    {
        Polynomial[] Factorize(BiVariablePolynomial interpolationPolynomial, int maxFactorDegree);
    }
}