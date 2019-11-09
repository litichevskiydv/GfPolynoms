namespace AppliedAlgebra.CodesResearchTools.Analyzers.Spectrum
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using CodesAbstractions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;

    public class MdsCodesSpectrumAnalyzer : ISpectrumAnalyzer
    {
        private static BigInteger GetCombinationsCount(int n, int k)
        {
            if (k == 0 || n == k)
                return BigInteger.One;
            if (n < k)
                return BigInteger.Zero;

            return GetCombinationsCount(n - 1, k - 1) + GetCombinationsCount(n - 1, k);
        }

        private static IReadOnlyDictionary<int, BigInteger> Analyze(int fieldOrder, int codewordLength, int codeDistance)
        {
            var result = new Dictionary<int, BigInteger>();
            BigInteger q = fieldOrder;
            for (var weight = codeDistance; weight <= codewordLength; weight++)
                result[weight]
                    = (q - BigInteger.One)
                      * GetCombinationsCount(codewordLength, weight)
                      * Enumerable.Range(0, weight - codeDistance + 1)
                          .Aggregate(BigInteger.Zero,
                              (acc, j) => acc + BigInteger.Pow(BigInteger.MinusOne, j)
                                          * GetCombinationsCount(weight - 1, j)
                                          * BigInteger.Pow(q, weight - codeDistance - j)
                          );

            return result;
        }

        public IReadOnlyDictionary<int, BigInteger> Analyze(ICode code, SpectrumAnalyzerOptions options = null)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            return Analyze(code.Field.Order, code.CodewordLength, code.CodeDistance);
        }

        public IReadOnlyDictionary<int, BigInteger> Analyze(GaloisField field, int informationWordLength, Func<FieldElement[], FieldElement[]> encodingProcedure, SpectrumAnalyzerOptions options = null)
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