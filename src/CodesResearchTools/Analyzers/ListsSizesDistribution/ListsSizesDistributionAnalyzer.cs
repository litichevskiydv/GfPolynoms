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
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms;
    using Microsoft.Extensions.Logging;

    public class ListsSizesDistributionAnalyzer : IListsSizesDistributionAnalyzer
    {
        private readonly IVariantsIterator _variantsIterator;
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

        private void ProcessBallCenter(
            ICode code,
            ListsSizesDistributionAnalyzerOptions options,
            ref long processedCentersCount,
            IReadOnlyList<ListsSizesDistribution> listsSizesDistributions,
            FieldElement[] ballCenter
        )
        {
            var lists = listsSizesDistributions.Select(x => new List<FieldElement[]>()).ToArray();
            var codewordsForLargestBall = code.DecodeViaList(ballCenter, code.CodewordLength - 1).Select(code.Encode);
            foreach (var codeword in codewordsForLargestBall)
                for (var i = Math.Max(0, ballCenter.ComputeHammingDistance(codeword) - 1); i < lists.Length; i++)
                    lists[i].Add(codeword);

            for (var i = 0; i < lists.Length; i++)
            {
                listsSizesDistributions[i].CollectInformation(ballCenter, lists[i]);
                LogListSize(options.FullLogsPath, code, i + 1, ballCenter, lists[i].Count);
            }

            if (Interlocked.Increment(ref processedCentersCount) % options.LoggingResolution == 0)
                _logger.LogInformation("Processed {processedCentersCount} centers", processedCentersCount);
        }

        public IReadOnlyList<ListsSizesDistribution> Analyze(ICode code, ListsSizesDistributionAnalyzerOptions options = null)
        {
            if(code == null)
                throw new ArgumentNullException(nameof(code));

            var opts = options ?? new ListsSizesDistributionAnalyzerOptions();
            var firstPortionLength = Math.Min(
                Math.Max((int) Math.Ceiling(Math.Log(opts.MaxDegreeOfParallelism, code.Field.Order)), 1),
                code.CodewordLength
            );
            var secondPortionLength = code.CodewordLength - firstPortionLength;
            PrepareLogFiles(opts.FullLogsPath, code);

            var processedCentersCount = 0L;
            var result = Enumerable.Range(1, code.CodewordLength - 1).Select(x => new ListsSizesDistribution(x)).ToArray();
            Parallel.ForEach(
                _variantsIterator.IterateVectors(code.Field, firstPortionLength),
                new ParallelOptions {MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism},
                ballCenterFirstPart =>
                {
                    if (secondPortionLength == 0)
                        ProcessBallCenter(code, opts, ref processedCentersCount, result, ballCenterFirstPart);
                    else
                        foreach (var ballCenterSecondPart in _variantsIterator.IterateVectors(code.Field, secondPortionLength))
                        {
                            var ballCenter = ballCenterFirstPart.Concat(ballCenterSecondPart).ToArray();
                            ProcessBallCenter(code, opts, ref processedCentersCount, result, ballCenter);
                        }
                }
            );

            return result;
        }

        public ListsSizesDistributionAnalyzer(IVariantsIterator variantsIterator, ILogger<ListsSizesDistributionAnalyzer> logger)
        {
            if (variantsIterator == null)
                throw new ArgumentNullException(nameof(variantsIterator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _variantsIterator = variantsIterator;
            _logger = logger;
        }
    }
}
