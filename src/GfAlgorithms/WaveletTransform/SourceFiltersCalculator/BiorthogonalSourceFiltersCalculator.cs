namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.SourceFiltersCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ComplementaryFilterBuilder;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using PolynomialExtensions = Extensions.PolynomialExtensions;

    /// <summary>
    /// Calculates orthogonal source filters by synthesis filter h
    /// </summary>
    public class BiorthogonalSourceFiltersCalculator : ISourceFiltersCalculator
    {
        private readonly IComplementaryFiltersBuilder _complementaryFiltersBuilder;

        public BiorthogonalSourceFiltersCalculator(IComplementaryFiltersBuilder complementaryFiltersBuilder)
        {
            if (complementaryFiltersBuilder == null)
                throw new ArgumentNullException(nameof(complementaryFiltersBuilder));

            _complementaryFiltersBuilder = complementaryFiltersBuilder;
        }

        /// <inheritdoc />
        public IEnumerable<FiltersBankVectors> GetSourceFilters(FieldElement[] h)
        {
            if (h == null)
                throw new ArgumentNullException(nameof(h));
            if(h.Length % 2 == 1)
                throw new ArgumentException("Filters length must be even");

            var expectedDegree = h.Length - 1;
            foreach (var (_, analysisPair, synthesisPair) in GetSourceFilters(new Polynomial(h), expectedDegree))
                yield return new FiltersBankVectors(
                    (analysisPair.hWithTilde.GetCoefficients(expectedDegree), analysisPair.gWithTilde.GetCoefficients(expectedDegree)),
                    (synthesisPair.h.GetCoefficients(expectedDegree), synthesisPair.g.GetCoefficients(expectedDegree))
                );
        }

        private Polynomial ComputeComplementaryFilter(int filtersLength, Polynomial h)
        {
            Polynomial g = null;
            try
            {
                g = _complementaryFiltersBuilder.Build(h, filtersLength);
            }
            catch (Exception)
            {
            }

            return g;
        }

        private static IEnumerable<FiltersBankPolynomials> GetSourceFiltersOfEvenLength(
            int filtersLength,
            Polynomial h,
            Polynomial g
        )
        {
            var field = h.Field;
            var one = new Polynomial(field, 1);
            var modularPolynomial = (one >> filtersLength) - one;

            var multiplier = one >> 1;
            var substitution = -(one >> (filtersLength - 1));
            yield return new FiltersBankPolynomials(
                filtersLength,
                (
                    -multiplier * g.PerformVariableSubstitution(substitution) % modularPolynomial,
                    multiplier * h.PerformVariableSubstitution(substitution) % modularPolynomial
                ),
                (new Polynomial(h), g)
            );
        }

        private static Polynomial ComputeDualComponentForOddFiltersLength(int filtersLength, Polynomial component)
        {
            var componentCoefficients = component.RaiseVariableDegree(2).GetCoefficients(filtersLength - 1);
            return new Polynomial(
                componentCoefficients
                    .Take(1)
                    .Concat(componentCoefficients.Skip(1).Reverse())
                    .ToArray()
            );
        }

        private static IEnumerable<FiltersBankPolynomials> GetSourceFiltersOfOddLength(
            int filtersLength,
            Polynomial h,
            Polynomial g
        )
        {
            var (ge, go) = g.GetPolyphaseComponents();
            var hWithTilde = PolynomialExtensions.CreateFormPolyphaseComponents(
                ComputeDualComponentForOddFiltersLength(filtersLength, go),
                ComputeDualComponentForOddFiltersLength(filtersLength, -ge)
            );

            var (he, ho) = h.GetPolyphaseComponents();
            var gWithTilde = PolynomialExtensions.CreateFormPolyphaseComponents(
                ComputeDualComponentForOddFiltersLength(filtersLength, -ho),
                ComputeDualComponentForOddFiltersLength(filtersLength, he)
            );

            yield return new FiltersBankPolynomials(filtersLength, (hWithTilde, gWithTilde), (new Polynomial(h), g));
        }


        /// <inheritdoc />
        public IEnumerable<FiltersBankPolynomials> GetSourceFilters(Polynomial h, int? expectedDegree = null)
        {
            if (h == null)
                throw new ArgumentNullException(nameof(h));
            if (expectedDegree < 0)
                throw new ArgumentException($"{expectedDegree} must not be negative");

            var filtersLength = (expectedDegree ?? h.Degree) + 1;

            var g = ComputeComplementaryFilter(filtersLength, h);
            if (g == null)
                return Enumerable.Empty<FiltersBankPolynomials>();

            return filtersLength % 2 == 0
                ? GetSourceFiltersOfEvenLength(filtersLength, h, g)
                : GetSourceFiltersOfOddLength(filtersLength, h, g);
        }
    }
}