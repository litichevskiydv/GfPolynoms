namespace AppliedAlgebra.CodesResearchTools.Analyzers.ListsSizesDistribution
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodesAbstractions;
    using GfAlgorithms.Extensions;
    using GfPolynoms;
    using Microsoft.Extensions.Logging;
    using NoiseGenerator;

    public class ListsSizesDistributionAnalyzer
    {
        private readonly INoiseGenerator _noiseGenerator;
        private readonly ILogger _logger;

        internal virtual void WriteLineToLog(string fullLogsPath, ICode code, int listDecodingRadius, string line, bool append = true)
        {
            var fileName = Path.Combine(fullLogsPath, $"{code}_list_decoding_radius_{listDecodingRadius}.csv");

            lock (this)
                if (append) File.AppendAllText(fileName, line + Environment.NewLine);
                else File.WriteAllText(fileName, line + Environment.NewLine);
        }

        private void PrepareLogFiles(string fullLogsPath, ICode code)
        {
            if(string.IsNullOrWhiteSpace(fullLogsPath))
                return;

            for (var listDecodingRadius = 1; listDecodingRadius < code.CodewordLength; listDecodingRadius++)
                WriteLineToLog(fullLogsPath, code, listDecodingRadius,
                    string.Join(",", Enumerable.Range(1, code.CodewordLength).Select(i => $"x{i}").Append("list_size")),
                    false
                );
        }

        private void LogListSize(string fullLogsPath, ICode code, int listDecodingRadius, FieldElement[] ballCenter, int listSize)
        {
            if (string.IsNullOrWhiteSpace(fullLogsPath))
                return;

            WriteLineToLog(fullLogsPath, code, listDecodingRadius,
                string.Join(",", ballCenter.Select(x => x.Representation).Append(listSize))
            );
        }

        public IReadOnlyList<ListsSizesDistribution> Analyze(ICode code, ListsSizesDistributionAnalyzerOptions options = null)
        {
            if(code == null)
                throw new ArgumentNullException(nameof(code));

            var opts = options ?? new ListsSizesDistributionAnalyzerOptions();
            PrepareLogFiles(opts.FullLogsPath, code);

            var processedCentersCount = 0;
            var result = Enumerable.Range(1, code.CodewordLength - 1).Select(x => new ListsSizesDistribution(x)).ToArray();
            for (var errorsCount = 0; errorsCount <= code.CodewordLength; errorsCount++)
                Parallel.ForEach(
                    _noiseGenerator.VariatePositionsAndValues(code.Field, code.CodewordLength, errorsCount),
                    new ParallelOptions {MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism},
                    ballCenter =>
                    {
                        var lists = result.Select(x => new List<FieldElement[]>()).ToArray();
                        var codewordsForLargestBall = code.DecodeViaList(ballCenter, code.CodewordLength - 1).Select(code.Encode);
                        foreach (var codeword in codewordsForLargestBall)
                            for (var i = Math.Max(0, ballCenter.ComputeHammingDistance(codeword) - 1); i < lists.Length; i++)
                                lists[i].Add(codeword);

                        for (var i = 0; i < lists.Length; i++)
                        {
                            result[i].CollectInformation(ballCenter, lists[i]);
                            LogListSize(opts.FullLogsPath, code, i + 1, ballCenter, lists[i].Count);
                        }

                        if (Interlocked.Increment(ref processedCentersCount) % opts.LoggingResolution == 0)
                            _logger.LogInformation("Processed {processedCentersCount} centers", processedCentersCount);
                    }
                );

            return result;
        }

        public ListsSizesDistributionAnalyzer(INoiseGenerator noiseGenerator, ILogger<ListsSizesDistributionAnalyzer> logger)
        {
            if (noiseGenerator == null)
                throw new ArgumentNullException(nameof(noiseGenerator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _noiseGenerator = noiseGenerator;
            _logger = logger;
        }
    }
}
