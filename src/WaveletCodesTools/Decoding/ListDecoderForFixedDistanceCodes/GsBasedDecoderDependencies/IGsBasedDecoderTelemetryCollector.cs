namespace AppliedAlgebra.WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes.GsBasedDecoderDependencies
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using GfPolynoms;

    /// <summary>
    /// Contract for telemetry reciver for wavelet code's list decoder based on Guruswami–Sudan algorithm
    /// </summary>
    public interface IGsBasedDecoderTelemetryCollector
    {
        /// <summary>
        /// Count of processed samples
        /// </summary>
        int ProcessedSamplesCount { get; }
        /// <summary>
        /// Samples count stored by lists sizes in frequency and time domains
        /// </summary>
        ConcurrentDictionary<Tuple<int, int>, int> ProcessingResults { get; }
        /// <summary>
        /// Important samples stored by lists sizes in frequency and time domains
        /// </summary>
        ConcurrentDictionary<Tuple<int, int>, List<(FieldElement xValue, FieldElement yValue)[]>> InterestingSamples { get; }

        /// <summary>
        /// Method for registering result of decoding 
        /// </summary>
        /// <param name="decodedCodeword">Decoded codeword</param>
        /// <param name="frequencyDecodingListSize">List size in frequency domain</param>
        /// <param name="timeDecodingListSize">List size in time domain</param>
        void ReportDecodingListsSizes((FieldElement xValue, FieldElement yValue)[] decodedCodeword, int frequencyDecodingListSize, int timeDecodingListSize);
    }
}