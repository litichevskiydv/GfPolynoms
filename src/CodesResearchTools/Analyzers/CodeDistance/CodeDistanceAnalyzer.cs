namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using Microsoft.Extensions.Logging;

    public class CodeDistanceAnalyzer :  CodeDistanceAnalyzerBase
    {
        protected override int AnalyzeInternal(
            GaloisField field, 
            int informationWordLength, 
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            CodeDistanceAnalyzerOptions options)
        {
            var processedPairsCount = 0L;
            var syncObject = new object();
            var codeDistance = int.MaxValue;
            Parallel.ForEach(
                GenerateMappings(field, encodingProcedure, new int[informationWordLength], 0),
                new ParallelOptions {MaxDegreeOfParallelism = options.MaxDegreeOfParallelism},
                mapping =>
                {
                    foreach (var (_, codeword) in GenerateMappings(field, encodingProcedure, mapping.informationWord, 0).Skip(1))
                    {
                        var distance = mapping.codeword.ComputeHammingDistance(codeword);
                        lock (syncObject) codeDistance = Math.Min(codeDistance, distance);

                        if (Interlocked.Increment(ref processedPairsCount) % options.LoggingResolution == 0)
                            Logger.LogInformation("Processed {processedPairsCount} centers", processedPairsCount);
                    }
                }
            );

            return codeDistance;
        }

        public CodeDistanceAnalyzer(ILogger<CodeDistanceAnalyzer> logger) : base(logger)
        {
        }
    }
}