namespace AppliedAlgebra.GfPolynoms.IrreduciblePolynomialsFinder
{
    /// <summary>
    /// Contract for finder of irreducible polynomial with specified properties
    /// </summary>
    public interface IIrreduciblePolynomialsFinder
    {
        /// <summary>
        /// Method for finding irreducible polynomial degree <paramref name="degree"/> with coefficients from field with order <paramref name="fieldOrder"/>
        /// </summary>
        /// <param name="fieldOrder">Field order from which irreducible polynomials coefficients come</param>
        /// <param name="degree">Irreducible polynomial degree</param>
        /// <returns>Irreducible polynomial with specified properties</returns>
        Polynomial Find(int fieldOrder, int degree);
    }
}