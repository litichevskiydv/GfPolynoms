namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeSpaceCovering
{
    using System;

    public class MinimalSphereCoveringAnalyzerOptions
    {
        public int MaxDegreeOfParallelism { get; set; }
        public long LoggingResolution { get; set; }

        public MinimalSphereCoveringAnalyzerOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount;
            LoggingResolution = 10000;
        }
    }
}