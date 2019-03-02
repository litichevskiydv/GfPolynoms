namespace AppliedAlgebra.CodesResearchTools.Analyzers.ListsSizesDistribution
{
    using System.Collections.Generic;
    using CodesAbstractions;

    public interface IListsSizesDistributionAnalyzer
    {
        IReadOnlyList<ListsSizesDistribution> Analyze(ICode code, ListsSizesDistributionAnalyzerOptions options = null);
    }
}