namespace AppliedAlgebra.RsCodesTools.ListDecoder.GsDecoderDependencies.InterpolationPolynomialFactorisator
{
    using GfAlgorithms.BiVariablePolynomials;
    using GfPolynoms;

    /// <summary>
    /// Contract for bivariate polynomial's factorization algorithm
    /// </summary>
    public interface IInterpolationPolynomialFactorizator
    {
        /// <summary>
        /// Method for finding bivariate polynomial's <paramref name="interpolationPolynomial"/> factors with max degree <paramref name="maxFactorDegree"/>
        /// </summary>
        /// <param name="interpolationPolynomial">Bivariate polynomial for factorization</param>
        /// <param name="maxFactorDegree">Maximum degree of factor</param>
        /// <returns>Array of finded factors</returns>
        Polynomial[] Factorize(BiVariablePolynomial interpolationPolynomial, int maxFactorDegree);
    }
}