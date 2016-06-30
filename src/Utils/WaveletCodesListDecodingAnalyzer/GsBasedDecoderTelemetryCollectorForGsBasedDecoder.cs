namespace WaveletCodesListDecodingAnalyzer
{
    using System;
    using System.Collections.Generic;
    using WaveletCodesTools.ListDecoderForFixedDistanceCodes;
    using WaveletCodesTools.ListDecoderForFixedDistanceCodes.GsBasedDecoderDependencies;

    public class GsBasedDecoderTelemetryCollectorForGsBasedDecoder : IGsBasedDecoderTelemetryCollector
    {
        public HashSet<Tuple<int, int>> ListsSizes { get; }

        public GsBasedDecoderTelemetryCollectorForGsBasedDecoder()
        {
            ListsSizes = new HashSet<Tuple<int, int>>();
        }

        public void ReportDecodingListsSizes(int frequencyDecodingListSize, int timeDecodingListSize)
        {
            lock (this)
                ListsSizes.Add(new Tuple<int, int>(frequencyDecodingListSize, timeDecodingListSize));
        }
    }
}