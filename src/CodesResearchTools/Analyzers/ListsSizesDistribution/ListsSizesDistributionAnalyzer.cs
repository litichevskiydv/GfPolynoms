namespace AppliedAlgebra.CodesResearchTools.Analyzers.ListsSizesDistribution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodesAbstractions;
    using Microsoft.Extensions.Logging;
    using NoiseGenerator;

    public class ListsSizesDistributionAnalyzer
    {
        private readonly INoiseGenerator _noiseGenerator;
        private readonly ILogger _logger;

        public ListsSizesDistributionAnalyzer(INoiseGenerator noiseGenerator, ILogger logger)
        {
            if(noiseGenerator == null)
                throw new ArgumentNullException(nameof(noiseGenerator));
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            _noiseGenerator = noiseGenerator;
            _logger = logger;
        }

        public IReadOnlyList<ListsSizesDistribution> Analyze(ICode code, int? maxDegreeOfParallelism = null)
        {
            var processedCentersCount = 0;
            var result = Enumerable.Range(1, code.CodewordLength - 1).Select(x => new ListsSizesDistribution(x)).ToArray();
;
            for (var errorsCount = 0; errorsCount <= code.CodewordLength; errorsCount++)
                Parallel.ForEach(
                    _noiseGenerator.VariatePositionsAndValues(code.Field, code.CodewordLength, errorsCount),
                    new ParallelOptions {MaxDegreeOfParallelism = maxDegreeOfParallelism ?? Environment.ProcessorCount},
                    x =>
                    {
                        for (var listDecodingRadius = 1; listDecodingRadius < code.CodewordLength; listDecodingRadius++)
                            result[listDecodingRadius - 1].CollectInformation(x, code.DecodeViaList(x, listDecodingRadius));

                        if(Interlocked.Increment(ref processedCentersCount) % 10000 == 0)
                            _logger.LogInformation("Processed {processedCentersCount} centers", processedCentersCount);
                    }
                );

            return result;
        }
    }
}
