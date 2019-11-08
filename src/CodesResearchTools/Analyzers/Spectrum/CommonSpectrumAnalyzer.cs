namespace AppliedAlgebra.CodesResearchTools.Analyzers.Spectrum
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodesAbstractions;
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using Microsoft.Extensions.Logging;

    public class CommonSpectrumAnalyzer : ISpectrumAnalyzer
    {
        private readonly IVariantsIterator _variantsIterator;
        private readonly ILogger _logger;

        public CommonSpectrumAnalyzer(IVariantsIterator variantsIterator, ILogger<CommonSpectrumAnalyzer> logger)
        {
            if(variantsIterator == null)
                throw new ArgumentNullException(nameof(variantsIterator));
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            _variantsIterator = variantsIterator;
            _logger = logger;
        }

        public IReadOnlyDictionary<int, long> Analyze(ICode code, SpectrumAnalyzerOptions options = null)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            return Analyze(code.Field, code.InformationWordLength, code.Encode, options);
        }

        public IReadOnlyDictionary<int, long> Analyze(
            GaloisField field,
            int informationWordLength,
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            SpectrumAnalyzerOptions options = null
        )
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (informationWordLength <= 0)
                throw new ArgumentException($"{nameof(informationWordLength)} must be positive");
            if (encodingProcedure == null)
                throw new ArgumentNullException(nameof(encodingProcedure));

            var opts = options ?? new SpectrumAnalyzerOptions();

            var processedCodewords = 0L;
            var spectrum = new ConcurrentDictionary<int, long>();
            Parallel.ForEach(
                _variantsIterator.IterateVectors(field, informationWordLength).Skip(1),
                new ParallelOptions {MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism},
                () => new Dictionary<int, long>(),
                (informationWord, loopState, localSpectrum) =>
                {
                    var codewordWeight = encodingProcedure(informationWord).Count(x => x.Representation != 0);

                    if (localSpectrum.ContainsKey(codewordWeight) == false)
                        localSpectrum[codewordWeight] = 0;
                    localSpectrum[codewordWeight]++;

                    if (Interlocked.Increment(ref processedCodewords) % opts.LoggingResolution == 0)
                        _logger.LogInformation(
                            "Thread {threadId}, processed {processedCodewords} pairs, local spectrum {localSpectrum}",
                            Thread.CurrentThread.ManagedThreadId,
                            processedCodewords,
                            string.Join(", ", localSpectrum.OrderBy(x => x.Key).Select(x => $"(W: {x.Key}, C: {x.Value})"))
                        );

                    return localSpectrum;
                },
                localSpectrum =>
                {
                    foreach (var pair in localSpectrum) 
                        spectrum.AddOrUpdate(pair.Key, pair.Value, (key, value) => value + pair.Value);
                }
            );

            return spectrum;
        }
    }
}