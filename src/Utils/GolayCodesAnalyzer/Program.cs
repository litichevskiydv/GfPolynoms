namespace AppliedAlgebra.GolayCodesAnalyzer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using CodesResearchTools.NoiseGenerator;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GolayCodesTools;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Serilog;

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

    [UsedImplicitly]
    public class Program
    {
        [UsedImplicitly]
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var loggerFactory = new LoggerFactory()
                .AddSerilog(
                    new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Version", Assembly.GetEntryAssembly().GetName().Version.ToString(4))
                        .CreateLogger()
                );
            var logger = loggerFactory.CreateLogger<Program>();

            var noiseGenerator = new RecursiveGenerator();
            for (var listDecodingRadius = 1; listDecodingRadius < 12; listDecodingRadius++)
            {
                var code = new G12GolayCode(listDecodingRadius);
                var informationWord = Enumerable.Repeat(code.Field.Zero(), code.InformationWordLength).ToArray();
                var codeword = code.Encode(informationWord);

                var researchResult = new ListDecodingResearchResult();
                for (var errorsCount = 0; errorsCount <= code.CodewordLength; errorsCount++)
                    Parallel.ForEach(
                        noiseGenerator.VariatePositionsAndValues(code.Field, code.CodewordLength, errorsCount),
                        new ParallelOptions {MaxDegreeOfParallelism = 3},
                        x =>
                        {
                            var noisyCodeword = codeword.AddNoise(x);
                            researchResult.CollectInformation(informationWord, noisyCodeword, code.DecodeViaList(noisyCodeword));
                        }
                    );

                logger.LogInformation($"Results for radius {listDecodingRadius}");
                logger.LogInformation(JsonConvert.SerializeObject(researchResult, Formatting.Indented));
            }
        }
    }
}
