namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;

    public class CodeDistanceAnalyzerOptions
    {
        public int MaxDegreeOfParallelism { get; set; }
        public long LoggingResolution { get; set; }

        public CodeDistanceAnalyzerOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount;
            LoggingResolution = 100000;
        }
    }
}