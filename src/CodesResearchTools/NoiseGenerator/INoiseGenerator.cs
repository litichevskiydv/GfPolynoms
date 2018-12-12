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
        /// Generate additive noise varying errors positions and values
        /// </summary>
        /// <param name="field">The field that produces the code alphabet</param>
        /// <param name="codewordLength">Codeword length</param>
        /// <param name="errorsCount">Errors count</param>
        /// <param name="initialNoiseValue">Generator initial value</param>
        IEnumerable<FieldElement[]> VariatePositionsAndValues(
            GaloisField field, 
            int codewordLength, 
            int errorsCount,
            FieldElement[] initialNoiseValue = null);

        /// <summary>
        /// Generate additive noise varying errors values
        /// </summary>
        /// <param name="initialNoiseValue">Generator initial value</param>
        IEnumerable<FieldElement[]> VariateValues(FieldElement[] initialNoiseValue);
    }
}