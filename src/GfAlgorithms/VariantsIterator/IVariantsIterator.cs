namespace AppliedAlgebra.GfAlgorithms.VariantsIterator
{
    using System.Collections.Generic;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Contract of the required objects iterator
    /// </summary>
    public interface IVariantsIterator
    {
        /// <summary>
        /// Produces all vectors of the length <paramref name="length"/> over field <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field over which vectors are defined</param>
        /// <param name="length">Vectors length</param>
        /// <param name="initialVector">Iterator start position</param>
        /// <returns>Enumeration consisting of the required vectors</returns>
        IEnumerable<FieldElement[]> IterateVectors(GaloisField field, int length, FieldElement[] initialVector = null);

        /// <summary>
        /// Produces all polynomials of degree less or equal <paramref name="maxDegree"/> over field <paramref name="field"/>
        /// </summary>
        /// <param name="field">Field over which polynomials are defined</param>
        /// <param name="maxDegree">Polynomials maximum degree</param>
        /// <returns>Enumeration consisting of the required polynomials</returns>
        IEnumerable<Polynomial> IteratePolynomials(GaloisField field, int maxDegree);
    }
}