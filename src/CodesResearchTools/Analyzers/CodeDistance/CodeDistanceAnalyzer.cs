namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CodesAbstractions;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using Microsoft.Extensions.Logging;

    public class CodeDistanceAnalyzer
    {
        private readonly ILogger _logger;

        private static IEnumerable<(int[] informationWord, FieldElement[] codeword)> GenerateMappings(ICode code, int[] informationWord, int currentPosition)
        {
            if (currentPosition == code.InformationWordLength)
            {
                yield return (informationWord.ToArray(), code.Encode(informationWord.Select(x => code.Field.CreateElement(x)).ToArray()));
                yield break;
            }

            for (var i = informationWord[currentPosition]; i < code.Field.Order; i++)
            {
                informationWord[currentPosition] = i;

                foreach (var mapping in GenerateMappings(code, informationWord, currentPosition + 1))
                    yield return mapping;

            }
            informationWord[currentPosition] = 0;
        }

        public int Analyze(ICode code, CodeDistanceAnalyzerOptions options = null)
        {
            if(code == null)
                throw new ArgumentNullException(nameof(code));

            var opts = options ?? new CodeDistanceAnalyzerOptions();

            var processedPairsCount = 0L;
            var syncObject = new object();
            var codeDistance = code.CodewordLength;
            Parallel.ForEach(
                GenerateMappings(code, new int[code.InformationWordLength], 0),
                new ParallelOptions { MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism},
                mapping =>
                {
                    foreach (var (_, codeword) in GenerateMappings(code, mapping.informationWord, 0).Skip(1))
                    {
                        var distance = mapping.codeword.ComputeHammingDistance(codeword);
                        lock (syncObject) codeDistance = Math.Min(codeDistance, distance);

                        if (Interlocked.Increment(ref processedPairsCount) % opts.LoggingResolution == 0)
                            _logger.LogInformation("Processed {processedPairsCount} centers", processedPairsCount);
                    }
                }
            );

            return codeDistance;
        }

        public CodeDistanceAnalyzer(ILogger<CodeDistanceAnalyzer> logger)
        {
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }
    }
}