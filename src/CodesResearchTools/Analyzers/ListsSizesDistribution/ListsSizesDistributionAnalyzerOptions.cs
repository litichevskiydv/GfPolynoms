namespace AppliedAlgebra.CodesResearchTools.Analyzers.ListsSizesDistribution
{
    using System;

    public class ListsSizesDistributionAnalyzerOptions
    {
        public int MaxDegreeOfParallelism { get; set; }
        public long LoggingResolution { get; set; }
        public string FullLogsPath { get; set; }

        public ListsSizesDistributionAnalyzerOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount;
            LoggingResolution = 10000;
        }
    }
}