namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using Microsoft.Extensions.Logging;

    public class LinearCodeDistanceAnalyzer : CodeDistanceAnalyzerBase
    {
        protected override int AnalyzeInternal(
            GaloisField field, 
            int informationWordLength, 
            Func<FieldElement[], FieldElement[]> encodingProcedure, 
            CodeDistanceAnalyzerOptions options)
        {
            var processedCodewords = 0L;
            var codeDistance = int.MaxValue;
            Parallel.ForEach(
                GenerateMappings(field, encodingProcedure, new int[informationWordLength], 0).Skip(1),
                new ParallelOptions { MaxDegreeOfParallelism = options.MaxDegreeOfParallelism },
                () => int.MaxValue,
                (mapping, loopState, localCodeDistance) =>
                {
                    if (Interlocked.Increment(ref processedCodewords) % options.LoggingResolution == 0)
                        Logger.LogInformation("Processed {processedCodewords} pairs, code distance {codeDistance}", processedCodewords, localCodeDistance);

                    var distance = mapping.codeword.Count(x => x.Representation != 0);
                    return Math.Min(localCodeDistance, distance);

                },
                localCodeDistance => { codeDistance = Math.Min(codeDistance, localCodeDistance);}
            );

            return codeDistance;
        }

        public LinearCodeDistanceAnalyzer(ILogger<LinearCodeDistanceAnalyzer> logger) : base(logger)
        {
        }
    }
}