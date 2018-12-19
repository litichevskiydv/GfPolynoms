namespace AppliedAlgebra.CodesResearchTools.Analyzers.ListsSizesDistribution
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using GfPolynoms;

    public class ListsSizesDistribution
    {
        private int _processedSamplesCount;


        public int ListDecodingRadius { get; }
        public int ProcessedSamplesCount => _processedSamplesCount;
        public ConcurrentDictionary<int, int> ListSizesDistribution { get; }
        public ConcurrentDictionary<int, string> NoisyCodewordsExamples { get; }

        public ListsSizesDistribution(int listDecodingRadius)
        {
            ListDecodingRadius = listDecodingRadius;
            ListSizesDistribution = new ConcurrentDictionary<int, int>();
            NoisyCodewordsExamples = new ConcurrentDictionary<int, string>();
        }

        public void CollectInformation(
            FieldElement[] noisyCodeword,
            IReadOnlyList<FieldElement[]> decodingResults)
        {
            Interlocked.Increment(ref _processedSamplesCount);
            ListSizesDistribution.AddOrUpdate(decodingResults.Count, 1, (key, value) => value + 1);
            NoisyCodewordsExamples.TryAdd(decodingResults.Count, $"[{string.Join<FieldElement>(", ", noisyCodeword)}]");
        }

    }
}