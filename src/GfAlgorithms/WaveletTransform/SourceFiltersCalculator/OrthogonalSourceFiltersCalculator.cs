﻿namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.SourceFiltersCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    /// <summary>
    /// Calculates orthogonal source filters by synthesis filter h
    /// </summary>
    public class OrthogonalSourceFiltersCalculator : ISourceFiltersCalculator
    {
        /// <inheritdoc />
        public IEnumerable<FiltersBankVectors> GetSourceFilters(FieldElement[] h)
        {
            if(h == null)
                throw new ArgumentNullException(nameof(h));
            if(h.Length == 0)
                throw new ArgumentException($"{nameof(h)} must not be empty");
            if (h.Length % 2 == 1)
                throw new ArgumentException("Filters length must be even");

            h.GetField();
            var filtersLength = h.Length;
            var g = Enumerable.Range(0, filtersLength).Select(i => h[filtersLength - 1 - i]).ToArray();
            yield return new FiltersBankVectors((h.CloneVector(), g.CloneVector()), (h.CloneVector(), g.CloneVector()));
        }

        /// <inheritdoc />
        public IEnumerable<FiltersBankPolynomials> GetSourceFilters(Polynomial h, int? expectedDegree = null)
        {
            if (h == null)
                throw new ArgumentNullException(nameof(h));

            foreach (var (analysisPair, synthesisPair) in GetSourceFilters(h.GetCoefficients(expectedDegree)))
                yield return new FiltersBankPolynomials(
                    synthesisPair.h.Length,
                    (new Polynomial(analysisPair.hWithTilde), new Polynomial(analysisPair.gWithTilde)),
                    (new Polynomial(synthesisPair.h), new Polynomial(synthesisPair.g))
                );
        }
    }
}