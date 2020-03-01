namespace AppliedAlgebra.GfPolynoms.IrreduciblePolynomialsFinder
{
    using GaloisFields;

    /// <summary>
    /// Contract for finder of irreducible polynomial with specified properties
    /// </summary>
    public interface IIrreduciblePolynomialsFinder
    {
        /// <summary>
        /// Method for finding irreducible polynomial degree <paramref name="degree"/>
        /// with coefficients from field <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field from which irreducible polynomials coefficients come</param>
        /// <param name="degree">Irreducible polynomial degree</param>
        /// <returns>Irreducible polynomial with specified properties</returns>
        Polynomial Find(GaloisField field, int degree);
    }
}