namespace AppliedAlgebra.CodesResearchTools.Analyzers.Spectrum
{
    using System;

    public class SpectrumAnalyzerOptions
    {
        public int MaxDegreeOfParallelism { get; set; }

        public long LoggingResolution { get; set; }

        public SpectrumAnalyzerOptions()
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount;
            LoggingResolution = 100000;
        }
    }
}