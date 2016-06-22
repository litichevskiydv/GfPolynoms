namespace RsListDecoding.InterpolationPolynomialFactorisator
{
    using System.Collections.Generic;
    using GfPolynoms;

    public interface IInterpolationPolynomialFactorizator
    {
        Polynomial[] Factorize(BiVariablePolynomial interpolationPolynomial, int maxFactorDegree);
    }
}