namespace AppliedAlgebra.CodesResearchTools.Analyzers.Spectrum
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public class MdsCodesSpectrumAnalyzer : ISpectrumAnalyzer
    {
        private static long GetCombinationsCount(int n, int k)
        {
            if (k == 0 || n == k)
                return 1L;
            if (n < k)
                return 0L;

            return GetCombinationsCount(n - 1, k - 1) + GetCombinationsCount(n - 1, k);
        }

        private static long GetPower(int a, int n)
        {
            var result = 1L;
            while (n > 0)
            {
                if ((n & 1) == 1)
                    result *= a;

                a *= a;
                n >>= 1;
            }

            return result;
        }

        private static IReadOnlyDictionary<int, long> Analyze(int fieldOrder, int codewordLength, int codeDistance)
        {
            var result = new Dictionary<int, long>();
            for (var weight = codeDistance; weight <= codewordLength; weight++)
                result[weight]
                    = (fieldOrder - 1)
                      * GetCombinationsCount(codewordLength, weight)
                      * Enumerable.Range(0, weight - codeDistance + 1)
                          .Aggregate(0L,
                              (acc, j) => acc + GetPower(-1, j)
                                          * GetCombinationsCount(weight - 1, j)
                                          * GetPower(fieldOrder, weight - codeDistance - j)
                          );

            return result;
        }

        public IReadOnlyDictionary<int, long> Analyze(ICode code, SpectrumAnalyzerOptions options = null)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            return Analyze(code.Field.Order, code.CodewordLength, code.CodeDistance);
        }

        public IReadOnlyDictionary<int, long> Analyze(GaloisField field, int informationWordLength, Func<FieldElement[], FieldElement[]> encodingProcedure, SpectrumAnalyzerOptions options = null)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            if (informationWordLength <= 0)
                throw new ArgumentException($"{nameof(informationWordLength)} must be positive");
            if (encodingProcedure == null)
                throw new ArgumentNullException(nameof(encodingProcedure));

            var codewordLength = encodingProcedure(Enumerable.Repeat(field.Zero(), informationWordLength).ToArray()).Length;
            return Analyze(field.Order, codewordLength, codewordLength - informationWordLength + 1);
        }
    }
}