namespace WaveletCodesTools.ListDecoderForFixedDistanceCodes.GsBasedDecoderDependencies
{
    using System;
    using System.Collections.Concurrent;

    public interface IGsBasedDecoderTelemetryCollector
    {
        int ProcessedSamplesCount { get; }
        ConcurrentDictionary<Tuple<int, int>, int> ProcessingResults { get; }

        void ReportDecodingListsSizes(int frequencyDecodingListSize, int timeDecodingListSize);
    }
}