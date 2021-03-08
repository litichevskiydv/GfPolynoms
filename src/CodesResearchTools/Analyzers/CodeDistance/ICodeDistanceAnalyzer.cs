namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    /// <summary>
    /// Code distance analyzer contract
    /// </summary>
    public interface ICodeDistanceAnalyzer
    {
        /// <summary>
        /// Computes code distance
        /// </summary>
        /// <param name="field">Field over which the code is defined</param>
        /// <param name="informationWordLength">Information word length</param>
        /// <param name="encodingProcedure">Encoding procedure</param>
        /// <param name="options">Analyzer options</param>
        /// <returns>Code distance if code distance minimum threshold is not defined</returns>
        int Analyze(
            GaloisField field,
            int informationWordLength,
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            CodeDistanceAnalyzerOptions options = null);
    }
}