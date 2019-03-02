namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeSpaceCovering
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodesAbstractions;
    using GfAlgorithms.Extensions;
    using Microsoft.Extensions.Logging;
    using NoiseGenerator;

    public class MinimalSphereCoveringAnalyzer : IMinimalSphereCoveringAnalyzer
    {
        private readonly INoiseGenerator _noiseGenerator;
        private readonly ILogger _logger;

        public int Analyze(ICode code, MinimalSphereCoveringAnalyzerOptions options = null)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            var opts = options ?? new MinimalSphereCoveringAnalyzerOptions();

            var minimalRadius = 0;
            var processedWordsCount = 0L;
            for (var errorsCount = 0; errorsCount <= code.CodewordLength; errorsCount++)
                Parallel.ForEach(
                    _noiseGenerator.VariatePositionsAndValues(code.Field, code.CodewordLength, errorsCount),
                    new ParallelOptions {MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism},
                    () => 0,
                    (word, loopState, localMinimalRadius) =>
                    {
                        var codewordsInLargestBall = code.DecodeViaList(word, code.CodewordLength - 1).Select(code.Encode);
                        if (Interlocked.Increment(ref processedWordsCount) % opts.LoggingResolution == 0)
                            _logger.LogInformation(
                                "Thread [{managedThreadId}]: processed {processedWordsCount} words, local minimal radius {localMinimalRadius}",
                                Thread.CurrentThread.ManagedThreadId, processedWordsCount, localMinimalRadius
                            );

                        return Math.Max(localMinimalRadius, codewordsInLargestBall.Min(x => x.ComputeHammingDistance(word)));
                    },
                    localMinimalRadius => minimalRadius = Math.Max(minimalRadius, localMinimalRadius)
                );

            return minimalRadius;
        }

        public MinimalSphereCoveringAnalyzer(INoiseGenerator noiseGenerator, ILogger<MinimalSphereCoveringAnalyzer> logger)
        {
            if(noiseGenerator == null)
                throw  new ArgumentNullException(nameof(noiseGenerator));
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            _noiseGenerator = noiseGenerator;
            _logger = logger;
        }
    }
}