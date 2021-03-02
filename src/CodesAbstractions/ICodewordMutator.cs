namespace AppliedAlgebra.CodesAbstractions
{
    using GfPolynoms;

    /// <summary>
    /// Codeword mutator contract
    /// </summary>
    public interface ICodewordMutator
    {
        /// <summary>
        /// Modifies codeword in to achieve required length
        /// </summary>
        /// <param name="originalCodeword">Original codeword</param>
        /// <param name="requiredLength">Required length</param>
        /// <returns>New codeword</returns>
        FieldElement[] Mutate(FieldElement[] originalCodeword, int requiredLength);
    }
}