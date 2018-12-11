namespace AppliedAlgebra.CodesResearchTools.NoiseGenerator
{
    using System.Collections.Generic;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Additive noise generator
    /// </summary>
    public interface INoiseGenerator
    {
        /// <summary>
        /// Generates additive noise for code which was defined over field <paramref name="field"/>
        /// </summary>
        /// <param name="field">The field that produces the code alphabet</param>
        /// <param name="codewordLength">Codeword length</param>
        /// <param name="errorsCount">Errors count</param>
        /// <param name="initialNoiseValue">Generator initial value</param>
        IEnumerable<FieldElement[]> Generate(GaloisField field, int codewordLength, int errorsCount, int[] initialNoiseValue = null);
    }
}