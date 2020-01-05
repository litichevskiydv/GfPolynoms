namespace AppliedAlgebra.CodesResearchTools.Analyzers.Spectrum
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
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

        public IReadOnlyDictionary<int, BigInteger> Analyze(ICode code, SpectrumAnalyzerOptions options = null)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            return Analyze(code.Field, code.InformationWordLength, code.Encode, options);
        }

        private Dictionary<int, BigInteger> ProcessInformationWord(
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            long loggingResolution,
            ref long processedInformationWords,
            Dictionary<int, BigInteger> localSpectrum,
            FieldElement[] informationWord
        )
        {
            if (informationWord.All(x => x.Representation == 0))
                return localSpectrum;

            var codewordWeight = encodingProcedure(informationWord).Count(x => x.Representation != 0);

            if (localSpectrum.ContainsKey(codewordWeight) == false)
                localSpectrum[codewordWeight] = 0;
            localSpectrum[codewordWeight]++;

            if (Interlocked.Increment(ref processedInformationWords) % loggingResolution == 0)
                _logger.LogInformation(
                    "Thread {threadId}, processed {processedInformationWords} pairs, local spectrum {localSpectrum}",
                    Thread.CurrentThread.ManagedThreadId,
                    processedInformationWords,
                    string.Join(", ", localSpectrum.OrderBy(x => x.Key).Select(x => $"(W: {x.Key}, C: {x.Value})"))
                );

            return localSpectrum;
        }

        public IReadOnlyDictionary<int, BigInteger> Analyze(
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
            var firstPortionLength = Math.Min((int) Math.Ceiling(Math.Log(opts.MaxDegreeOfParallelism, field.Order)), informationWordLength);
            var secondPortionLength = informationWordLength - firstPortionLength;

            var processedInformationWords = 0L;
            var spectrum = new ConcurrentDictionary<int, BigInteger>();
            Parallel.ForEach(
                _variantsIterator.IterateVectors(field, firstPortionLength),
                new ParallelOptions {MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism},
                () => new Dictionary<int, BigInteger>(),
                (informationWordFirstPart, loopState, localSpectrum) =>
                {
                    if (secondPortionLength == 0)
                        return ProcessInformationWord(encodingProcedure, opts.LoggingResolution, ref processedInformationWords, localSpectrum, informationWordFirstPart);

                    foreach (var informationWordSecondPart in _variantsIterator.IterateVectors(field, secondPortionLength))
                    {
                        var informationWord = informationWordFirstPart.Concat(informationWordSecondPart).ToArray();
                        ProcessInformationWord(encodingProcedure, opts.LoggingResolution, ref processedInformationWords, localSpectrum, informationWord);
                    }

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