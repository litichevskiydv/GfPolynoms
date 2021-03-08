namespace AppliedAlgebra.CodesResearchTools.Analyzers.CodeDistance
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using GfAlgorithms.VariantsIterator;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using Microsoft.Extensions.Logging;

    public abstract class CodeDistanceAnalyzerBase : ICodeDistanceAnalyzer
    {
        protected readonly IVariantsIterator VariantsIterator;
        protected readonly ILogger Logger;

        protected IEnumerable<(FieldElement[] informationWord, FieldElement[] codeword)> GenerateMappings(
            GaloisField field,
            int informationWordLength,
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            FieldElement[] initialInformationWord = null
        ) => VariantsIterator.IterateVectors(field, informationWordLength, initialInformationWord)
            .Select(informationWord => (informationWord, encodingProcedure(informationWord)));

        protected abstract int AnalyzeInternal(GaloisField field,
            int informationWordLength,
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            CodeDistanceAnalyzerOptions options
        );

        public int Analyze(
            GaloisField field,
            int informationWordLength,
            Func<FieldElement[], FieldElement[]> encodingProcedure,
            CodeDistanceAnalyzerOptions options = null
        )
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

        protected CodeDistanceAnalyzerBase(IVariantsIterator variantsIterator, ILogger logger)
        {
            if (variantsIterator == null)
                throw new ArgumentNullException(nameof(variantsIterator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            VariantsIterator = variantsIterator;
            Logger = logger;
        }
    }
}