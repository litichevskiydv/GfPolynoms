namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;

    public interface ICodeDistanceAnalyzer
    {
        int Analyze(
            GaloisField field,
            int informationWordLength,
            Func<int[], FieldElement[]> encodingProcedure,
            CodeDistanceAnalyzerOptions options = null);
    }
}