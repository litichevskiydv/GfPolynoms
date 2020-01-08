namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeSpaceCovering
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodesAbstractions;
    using GfAlgorithms.Extensions;
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms;
    using Microsoft.Extensions.Logging;

    public class MinimalSphereCoveringAnalyzer : IMinimalSphereCoveringAnalyzer
    {
        private readonly IVariantsIterator _variantsIterator;
        private readonly ILogger _logger;

        private int ProcessWord(
            ICode code,
            MinimalSphereCoveringAnalyzerOptions options,
            ref long processedWordsCount,
            int minimalSphereCoveringRadius,
            FieldElement[] word
        )
        {
            var codewordsInLargestBall = code.DecodeViaList(word, code.CodewordLength - 1).Select(code.Encode);
            if (Interlocked.Increment(ref processedWordsCount) % options.LoggingResolution == 0)
                _logger.LogInformation(
                    "Thread [{managedThreadId}]: processed {processedWordsCount} words, local minimal radius {minimalSphereCoveringRadius}",
                    Thread.CurrentThread.ManagedThreadId, processedWordsCount, minimalSphereCoveringRadius
                );

            return Math.Max(minimalSphereCoveringRadius, codewordsInLargestBall.Min(x => x.ComputeHammingDistance(word)));
        }

        public int Analyze(ICode code, MinimalSphereCoveringAnalyzerOptions options = null)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            var opts = options ?? new MinimalSphereCoveringAnalyzerOptions();
            var firstPortionLength = Math.Min(
                Math.Max((int)Math.Ceiling(Math.Log(opts.MaxDegreeOfParallelism, code.Field.Order)), 1),
                code.CodewordLength
            );
            var secondPortionLength = code.CodewordLength - firstPortionLength;

            var minimalRadius = 0;
            var processedWordsCount = 0L;
            var syncRoot = new object();
            Parallel.ForEach(
                _variantsIterator.IterateVectors(code.Field, firstPortionLength),
                new ParallelOptions {MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism},
                () => 0,
                (wordFirstPart, loopState, localMinimalRadius) =>
                {
                    if (secondPortionLength == 0)
                        return ProcessWord(code, opts, ref processedWordsCount, localMinimalRadius, wordFirstPart);

                    return _variantsIterator.IterateVectors(code.Field, secondPortionLength)
                        .Select(wordSecondPart => wordFirstPart.Concat(wordSecondPart).ToArray())
                        .Aggregate(localMinimalRadius, (current, word) => ProcessWord(code, opts, ref processedWordsCount, current, word));
                },
                localMinimalRadius =>
                {
                    lock (syncRoot) minimalRadius = Math.Max(minimalRadius, localMinimalRadius);
                }
            );

            return minimalRadius;
        }

        public MinimalSphereCoveringAnalyzer(IVariantsIterator variantsIterator, ILogger<MinimalSphereCoveringAnalyzer> logger)
        {
            if(variantsIterator == null)
                throw  new ArgumentNullException(nameof(variantsIterator));
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            _variantsIterator = variantsIterator;
            _logger = logger;
        }
    }
}