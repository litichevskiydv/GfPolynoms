namespace AppliedAlgebra.WaveletCodesListDecodingAnalyzer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using GfPolynoms;
    using WaveletCodesTools.Decoding.ListDecoderForFixedDistanceCodes.GsBasedDecoderDependencies;

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
        public ConcurrentDictionary<Tuple<int, int>, List<(FieldElement xValue, FieldElement yValue)[]>> InterestingSamples { get; }

        public GsBasedDecoderTelemetryCollectorForGsBasedDecoder()
        {
            ProcessingResults = new ConcurrentDictionary<Tuple<int, int>, int>();
            InterestingSamples = new ConcurrentDictionary<Tuple<int, int>, List<(FieldElement xValue, FieldElement yValue)[]>>();
        }

        /// <summary>
        /// Метод для регистрации результата декодирования
        /// </summary>
        /// <param name="decodedCodeword">Декодируемое кодовое слово</param>
        /// <param name="frequencyDecodingListSize">Размер списка при декодировании в частотной области</param>
        /// <param name="timeDecodingListSize">Размер списка при декодировании во временной области</param>
        public void ReportDecodingListsSizes(
            (FieldElement xValue, FieldElement yValue)[] decodedCodeword, 
            int frequencyDecodingListSize, 
            int timeDecodingListSize
        )
        {
            var listsSizes = Tuple.Create(frequencyDecodingListSize, timeDecodingListSize);

            ProcessingResults.AddOrUpdate(listsSizes, 1, (key, value) => value + 1);
            if (timeDecodingListSize > 1)
            {
                var clonnedCodeword = decodedCodeword.Select(x => (new FieldElement(x.xValue), new FieldElement(x.yValue))).ToArray();
                InterestingSamples.AddOrUpdate(listsSizes, new List<(FieldElement xValue, FieldElement yValue)[]> {clonnedCodeword},
                    (key, value) =>
                    {
                        value.Add(clonnedCodeword);
                        return value;
                    });
            }

            Interlocked.Increment(ref _processedSamplesCount);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"Processed {ProcessedSamplesCount} samples");
            var listsSizes = ProcessingResults.ToArray();
            var interestingSamples = InterestingSamples.ToDictionary(x => x.Key, x => x.Value.ToArray());
            foreach (var listSize in listsSizes)
            {
                builder.AppendLine($"Frequency decoding list size {listSize.Key.Item1}, time decoding list size {listSize.Key.Item2}, {listSize.Value} samples");

                if (interestingSamples.TryGetValue(listSize.Key, out var collectedSamples))
                {
                    builder.AppendLine("\tInteresting samples were collected:");
                    foreach (var collectedSample in collectedSamples)
                        builder.AppendLine("\t\t[" + string.Join(",", collectedSample.Select(x => $"({x.xValue},{x.yValue})")) + "]");
                }
            }

            return builder.ToString().TrimEnd('\n');
        }
    }
}