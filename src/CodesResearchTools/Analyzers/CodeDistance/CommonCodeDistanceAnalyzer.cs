﻿namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using Microsoft.Extensions.Logging;

    public class CommonCodeDistanceAnalyzer :  CodeDistanceAnalyzerBase
    {
        protected override int AnalyzeInternal(
            GaloisField field, 
            int informationWordLength, 
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            CodeDistanceAnalyzerOptions options)
        {
            var processedPairsCount = 0L;
            var codeDistance = int.MaxValue;
            Parallel.ForEach(
                GenerateMappings(field, informationWordLength, encodingProcedure),
                new ParallelOptions {MaxDegreeOfParallelism = options.MaxDegreeOfParallelism},
                () => int.MaxValue,
                (mapping, loopState, localCodeDistance) =>
                {
                    foreach (var (_, codeword) in GenerateMappings(field, informationWordLength, encodingProcedure, mapping.informationWord).Skip(1))
                    {
                        if(loopState.IsStopped)
                            return localCodeDistance;

                        if (Interlocked.Increment(ref processedPairsCount) % options.LoggingResolution == 0)
                            Logger.LogInformation("Processed {processedPairsCount} pairs, code distance {codeDistance}", processedPairsCount, localCodeDistance);

                        localCodeDistance = Math.Min(localCodeDistance, mapping.codeword.ComputeHammingDistance(codeword));
                        if (options.CodeDistanceMinimumThreshold.HasValue && localCodeDistance < options.CodeDistanceMinimumThreshold.Value)
                        {
                            loopState.Stop();
                            return localCodeDistance;
                        }
                    }

                    return localCodeDistance;
                },
                localCodeDistance =>
                {
                    InterlockedExtensions.Min(ref codeDistance, localCodeDistance);
                }
            );

            return codeDistance;
        }

        public CommonCodeDistanceAnalyzer(IVariantsIterator variantsIterator, ILogger<CommonCodeDistanceAnalyzer> logger) 
            : base(variantsIterator, logger)
        {
        }
    }
}