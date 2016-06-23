namespace RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator
{
    using GfPolynoms;

    public interface IInterpolationPolynomialFactorizator
    {
        Polynomial[] Factorize(BiVariablePolynomial interpolationPolynomial, int maxFactorDegree);
    }
}