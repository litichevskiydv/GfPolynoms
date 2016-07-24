namespace WaveletCodesListDecodingAnalyzer
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using WaveletCodesTools.ListDecoderForFixedDistanceCodes.GsBasedDecoderDependencies;

    public class GsBasedDecoderTelemetryCollectorForGsBasedDecoder : IGsBasedDecoderTelemetryCollector
    {
        private int _processedSamplesCount;

        public int ProcessedSamplesCount => _processedSamplesCount;
        public ConcurrentDictionary<Tuple<int, int>, int> ProcessingResults { get; }

        public GsBasedDecoderTelemetryCollectorForGsBasedDecoder()
        {
            ProcessingResults = new ConcurrentDictionary<Tuple<int, int>, int>();
        }

        public void ReportDecodingListsSizes(int frequencyDecodingListSize, int timeDecodingListSize)
        {
            var listsSizes = new Tuple<int, int>(frequencyDecodingListSize, timeDecodingListSize);
            ProcessingResults.AddOrUpdate(listsSizes, 1, (key, value) => value + 1);
            Interlocked.Increment(ref _processedSamplesCount);
        }
    }
}