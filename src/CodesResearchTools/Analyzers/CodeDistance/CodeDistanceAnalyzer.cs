namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using Microsoft.Extensions.Logging;

    public class CodeDistanceAnalyzer
    {
        private readonly ILogger _logger;

        private static IEnumerable<(int[] informationWord, FieldElement[] codeword)> GenerateMappings(
            GaloisField field,
            Func<FieldElement[], FieldElement[]> encodingProcedure, 
            int[] informationWord, 
            int currentPosition)
        {
            if (currentPosition == informationWord.Length)
            {
                yield return (informationWord.ToArray(), encodingProcedure(informationWord.Select(field.CreateElement).ToArray()));
                yield break;
            }

            for (var i = informationWord[currentPosition]; i < field.Order; i++)
            {
                informationWord[currentPosition] = i;

                foreach (var mapping in GenerateMappings(field, encodingProcedure, informationWord, currentPosition + 1))
                    yield return mapping;

            }
            informationWord[currentPosition] = 0;
        }

        public int Analyze(
            GaloisField field, 
            int informationWordLength, 
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            CodeDistanceAnalyzerOptions options = null)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if(informationWordLength <= 0)
                throw new ArgumentException($"{nameof(informationWordLength)} must be positive");
            if(encodingProcedure == null)
                throw new ArgumentNullException(nameof(encodingProcedure));

            var opts = options ?? new CodeDistanceAnalyzerOptions();

            var processedPairsCount = 0L;
            var syncObject = new object();
            var codeDistance = int.MaxValue;
            Parallel.ForEach(
                GenerateMappings(field, encodingProcedure, new int[informationWordLength], 0),
                new ParallelOptions {MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism},
                mapping =>
                {
                    foreach (var (_, codeword) in GenerateMappings(field, encodingProcedure, mapping.informationWord, 0).Skip(1))
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