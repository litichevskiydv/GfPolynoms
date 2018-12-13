namespace AppliedAlgebra.GolayCodesAnalyzer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodesResearchTools.NoiseGenerator;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GolayCodesTools;
    using Newtonsoft.Json;

    public class ListDecodingResearchResult
    {
        private int _processedSamplesCount;
        private string _wrongDecodingExample;

        public int ProcessedSamplesCount => _processedSamplesCount;
        
        public ConcurrentDictionary<int, int> ListSizesDistribution { get; }

        public ConcurrentDictionary<int, string> NoisyCodewordsExamples { get; }

        public string WrongDecodingExample => _wrongDecodingExample;



        public ListDecodingResearchResult()
        {
            ListSizesDistribution = new ConcurrentDictionary<int, int>();
            NoisyCodewordsExamples = new ConcurrentDictionary<int, string>();
        }

        public void CollectInformation(
            FieldElement[] informationWord,
            FieldElement[] noisyCodeword,
            IReadOnlyList<FieldElement[]> decodingResults)
        {
            Interlocked.Increment(ref _processedSamplesCount);
            ListSizesDistribution.AddOrUpdate(decodingResults.Count, 1, (key, value) => value + 1);
            NoisyCodewordsExamples.TryAdd(decodingResults.Count, $"[{string.Join<FieldElement>(", ", noisyCodeword)}]");

            if (decodingResults.All(x => x.SequenceEqual(informationWord) == false))
                Interlocked.CompareExchange(ref _wrongDecodingExample, $"[{string.Join<FieldElement>(", ", noisyCodeword)}]", null);
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            var noiseGenerator = new RecursiveGenerator();

            for (var listDecodingRadius = 1; listDecodingRadius < 12; listDecodingRadius++)
            {
                var code = new G12GolayCode(listDecodingRadius);
                var informationWord = Enumerable.Repeat(code.Field.Zero(), code.InformationWordLength).ToArray();
                var codeword = code.Encode(informationWord);

                var researchResult = new ListDecodingResearchResult();
                for (var errorsCount = 1; errorsCount <= code.CodewordLength; errorsCount++)
                    Parallel.ForEach(
                        noiseGenerator.VariatePositionsAndValues(code.Field, code.CodewordLength, errorsCount),
                        new ParallelOptions {MaxDegreeOfParallelism = 3},
                        x =>
                        {
                            var noisyCodeword = codeword.AddNoise(x);
                            researchResult.CollectInformation(informationWord, noisyCodeword, code.DecodeViaList(noisyCodeword));
                        }
                    );

                Console.WriteLine($"Results for radius {listDecodingRadius}");
                Console.WriteLine(JsonConvert.SerializeObject(researchResult, Formatting.Indented));
            }
        }
    }
}
