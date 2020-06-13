namespace AppliedAlgebra.GfPolynoms.IrreduciblePolynomialsFinder
{
    using System.Collections.Generic;
    using GaloisFields;

    /// <summary>
    /// Contract for finder of irreducible polynomial with specified properties
    /// </summary>
    public interface IIrreduciblePolynomialsFinder
    {
        /// <summary>
        /// Finds irreducible polynomials degree <paramref name="degree"/>
        /// with coefficients from field <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field from which irreducible polynomials coefficients come</param>
        /// <param name="degree">Irreducible polynomial degree</param>
        /// <returns>Irreducible polynomials with with specified properties</returns>
        IEnumerable<Polynomial> Find(GaloisField field, int degree);
    }
}