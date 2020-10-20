namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using Microsoft.Extensions.Logging;

    public abstract class CodeDistanceAnalyzerBase : ICodeDistanceAnalyzer
    {
        protected readonly ILogger Logger;

        protected static IEnumerable<(int[] informationWord, FieldElement[] codeword)> GenerateMappings(
            GaloisField field,
            Func<int[], FieldElement[]> encodingProcedure,
            int[] informationWord,
            int currentPosition)
        {
            if (currentPosition == informationWord.Length)
            {
                yield return (informationWord.ToArray(), encodingProcedure(informationWord));
                yield break;
            }

            for (var i = informationWord[currentPosition]; i < field.Order; i++)
            {
                informationWord[currentPosition] = i;

                foreach (var mapping in GenerateMappings(field, encodingProcedure, informationWord, currentPosition + 1))
                    yield return mapping;

            }
            informationWord[currentPosition] = 0;
        }

        protected abstract int AnalyzeInternal(GaloisField field,
            int informationWordLength,
            Func<int[], FieldElement[]> encodingProcedure,
            CodeDistanceAnalyzerOptions options);

        public int Analyze(
            GaloisField field, 
            int informationWordLength,
            Func<int[], FieldElement[]> encodingProcedure,
            CodeDistanceAnalyzerOptions options = null)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (informationWordLength <= 0)
                throw new ArgumentException($"{nameof(informationWordLength)} must be positive");
            if (encodingProcedure == null)
                throw new ArgumentNullException(nameof(encodingProcedure));

            var opts = options ?? new CodeDistanceAnalyzerOptions();
            return AnalyzeInternal(field, informationWordLength, encodingProcedure, opts);
        }

        protected CodeDistanceAnalyzerBase(ILogger logger)
        {
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            Logger = logger;
        }
    }
}