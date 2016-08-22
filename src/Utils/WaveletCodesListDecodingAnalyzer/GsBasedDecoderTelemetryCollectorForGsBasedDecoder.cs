namespace WaveletCodesListDecodingAnalyzer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using GfPolynoms;
    using WaveletCodesTools.ListDecoderForFixedDistanceCodes.GsBasedDecoderDependencies;

    public class GsBasedDecoderTelemetryCollectorForGsBasedDecoder : IGsBasedDecoderTelemetryCollector
    {
        private int _processedSamplesCount;

        /// <summary>
        /// Общее количество обработанных примеров
        /// </summary>
        public int ProcessedSamplesCount => _processedSamplesCount;
        /// <summary>
        /// Распределения длин списков в результатах декодирования
        /// </summary>
        public ConcurrentDictionary<Tuple<int, int>, int> ProcessingResults { get; }
        /// <summary>
        /// Отобранные интересные примеры
        /// </summary>
        public ConcurrentDictionary<Tuple<int, int>, List<Tuple<FieldElement, FieldElement>[]>> InterestingSamples { get; }

        public GsBasedDecoderTelemetryCollectorForGsBasedDecoder()
        {
            ProcessingResults = new ConcurrentDictionary<Tuple<int, int>, int>();
            InterestingSamples = new ConcurrentDictionary<Tuple<int, int>, List<Tuple<FieldElement, FieldElement>[]>>();
        }

        /// <summary>
        /// Метод для регистрации результата декодирования
        /// </summary>
        /// <param name="decodedCodeword">Декодируемое кодовое слово</param>
        /// <param name="frequencyDecodingListSize">Размер списка при декодировании в частотной области</param>
        /// <param name="timeDecodingListSize">Размер списка при декодировании во временной области</param>
        public void ReportDecodingListsSizes(Tuple<FieldElement, FieldElement>[] decodedCodeword, 
            int frequencyDecodingListSize, int timeDecodingListSize)
        {
            var listsSizes = Tuple.Create(frequencyDecodingListSize, timeDecodingListSize);

            ProcessingResults.AddOrUpdate(listsSizes, 1, (key, value) => value + 1);
            if (listsSizes.Item1 == listsSizes.Item2 && listsSizes.Item1 != 1)
                InterestingSamples.AddOrUpdate(listsSizes, new List<Tuple<FieldElement, FieldElement>[]> {decodedCodeword},
                    (key, value) =>
                    {
                        value.Add(decodedCodeword);
                        return value;
                    });

            Interlocked.Increment(ref _processedSamplesCount);
        }
    }
}