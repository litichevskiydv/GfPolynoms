namespace AppliedAlgebra.CodesResearchTools.Analyzers.ListsSizesDistribution
{
    using System;

    public class ListsSizesDistributionAnalyzerOptions
    {
        public int MaxDegreeOfParallelism { get; set; }
        public int LoggingResolution { get; set; }

        public ListsSizesDistributionAnalyzerOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount;
            LoggingResolution = 10000;
        }
    }
}