namespace WaveletCodesTools.ListDecoderForFixedDistanceCodes.GsBasedDecoderDependencies
{
    public interface IGsBasedDecoderTelemetryCollector
    {
        void ReportDecodingListsSizes(int frequencyDecodingListSize, int timeDecodingListSize);
    }
}