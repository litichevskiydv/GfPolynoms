namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using Microsoft.Extensions.Logging;

    public class LinearCodeDistanceAnalyzer : CodeDistanceAnalyzerBase
    {
        private int ProcessInformationWord(
            FieldElement[] informationWord,
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            CodeDistanceAnalyzerOptions options,
            ParallelLoopState loopState,
            int localCodeDistance,
            ref long processedCodewords
        )
        {
            if (Interlocked.Increment(ref processedCodewords) % options.LoggingResolution == 0)
                Logger.LogInformation("Processed {processedCodewords} pairs, code distance {codeDistance}", processedCodewords, localCodeDistance);

            var distance = encodingProcedure(informationWord).Count(x => x.Representation != 0);
            if (distance == 0 && informationWord.All(x => x.Representation == 0))
                return localCodeDistance;

            if (options.CodeDistanceMinimumThreshold.HasValue && distance < options.CodeDistanceMinimumThreshold.Value)
                loopState.Stop();

            return Math.Min(localCodeDistance, distance);
        }

        protected override int AnalyzeInternal(
            GaloisField field,
            int informationWordLength,
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            CodeDistanceAnalyzerOptions options
        )
        {
            var firstPortionLength = Math.Min(
                Math.Max((int)Math.Ceiling(Math.Log(options.MaxDegreeOfParallelism, field.Order)), 1),
                informationWordLength
            );
            var secondPortionLength = informationWordLength - firstPortionLength;

            var processedCodewords = 0L;
            var codeDistance = int.MaxValue;
            Parallel.ForEach(
                VariantsIterator.IterateVectors(field, firstPortionLength),
                new ParallelOptions {MaxDegreeOfParallelism = options.MaxDegreeOfParallelism},
                () => int.MaxValue,
                (informationWordFirstPart, loopState, localCodeDistance) =>
                {
                    if (loopState.IsStopped)
                        return localCodeDistance;

                    if (secondPortionLength == 0)
                        return ProcessInformationWord(
                            informationWordFirstPart,
                            encodingProcedure,
                            options,
                            loopState,
                            localCodeDistance,
                            ref processedCodewords
                        );

                    foreach (var informationWordSecondPart in VariantsIterator.IterateVectors(field, secondPortionLength))
                    {
                        if (loopState.IsStopped)
                            return localCodeDistance;

                        localCodeDistance = ProcessInformationWord(
                            informationWordFirstPart.Concat(informationWordSecondPart).ToArray(),
                            encodingProcedure,
                            options,
                            loopState,
                            localCodeDistance,
                            ref processedCodewords
                        );
                    }

                    return localCodeDistance;
                },
                localCodeDistance => { InterlockedExtensions.Min(ref codeDistance, localCodeDistance); }
            );

            return codeDistance;
        }

        public LinearCodeDistanceAnalyzer(IVariantsIterator variantsIterator, ILogger<LinearCodeDistanceAnalyzer> logger) 
            : base(variantsIterator, logger)
        {
        }
    }
}