namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeSpaceCovering
{
    using CodesAbstractions;

    public interface IMinimalSphereCoveringAnalyzer
    {
        int Analyze(ICode code, MinimalSphereCoveringAnalyzerOptions options = null);
    }
}