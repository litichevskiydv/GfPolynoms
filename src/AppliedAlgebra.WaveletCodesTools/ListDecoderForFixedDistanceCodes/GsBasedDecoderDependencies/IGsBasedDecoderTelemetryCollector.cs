namespace AppliedAlgebra.WaveletCodesTools.ListDecoderForFixedDistanceCodes.GsBasedDecoderDependencies
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using GfPolynoms;

    public interface IGsBasedDecoderTelemetryCollector
    {
        /// <summary>
        /// Общее количество обработанных примеров
        /// </summary>
        int ProcessedSamplesCount { get; }
        /// <summary>
        /// Распределения длин списков в результатах декодирования
        /// </summary>
        ConcurrentDictionary<Tuple<int, int>, int> ProcessingResults { get; }
        /// <summary>
        /// Отобранные интересные примеры
        /// </summary>
        ConcurrentDictionary<Tuple<int, int>, List<Tuple<FieldElement, FieldElement>[]>> InterestingSamples { get; }

        /// <summary>
        /// Метод для регистрации результата декодирования
        /// </summary>
        /// <param name="decodedCodeword">Декодируемое кодовое слово</param>
        /// <param name="frequencyDecodingListSize">Размер списка при декодировании в частотной области</param>
        /// <param name="timeDecodingListSize">Размер списка при декодировании во временной области</param>
        void ReportDecodingListsSizes(Tuple<FieldElement, FieldElement>[] decodedCodeword, int frequencyDecodingListSize, int timeDecodingListSize);
    }
}