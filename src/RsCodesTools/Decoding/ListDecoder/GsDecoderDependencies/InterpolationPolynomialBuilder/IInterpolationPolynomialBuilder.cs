namespace AppliedAlgebra.RsCodesTools.Decoding.ListDecoder.GsDecoderDependencies.InterpolationPolynomialBuilder
{
    using System;
    using GfAlgorithms.BiVariablePolynomials;
    using GfPolynoms;

    /// <summary>
    /// Contarct for bivariate interpolation polynomial builder
    /// </summary>
    public interface IInterpolationPolynomialBuilder
    {
        /// <summary>
        /// Method for bivariate interpolation polynomial building
        /// </summary>
        /// <param name="degreeWeight">Weight of bivariate monomials degree</param>
        /// <param name="maxWeightedDegree">Maximum value of bivariate monomial degree</param>
        /// <param name="roots">Roots of the interpolation polynomial</param>
        /// <param name="rootsMultiplicity">Multiplicity of bivariate polynomial's roots</param>
        /// <returns>Builded interpolation polynomial</returns>
        BiVariablePolynomial Build(Tuple<int, int> degreeWeight, int maxWeightedDegree, Tuple<FieldElement, FieldElement>[] roots, int rootsMultiplicity);
    }
}