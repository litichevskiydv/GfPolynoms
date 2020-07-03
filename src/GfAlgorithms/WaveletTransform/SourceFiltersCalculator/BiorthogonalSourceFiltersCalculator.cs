namespace AppliedAlgebra.GfAlgorithms.WaveletTransform.SourceFiltersCalculator
{
    using System;
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
        public FiltersBankVectors GetSourceFilters(FieldElement[] h)
        {
            if (h == null)
                throw new ArgumentNullException(nameof(h));
            if(h.Length % 2 == 1)
                throw new ArgumentException("Filters length must be even");

            var expectedDegree = h.Length - 1;
            var (_, analysisPair, synthesisPair) = GetSourceFilters(new Polynomial(h), expectedDegree);
            return new FiltersBankVectors(
                (analysisPair.hWithTilde.GetCoefficients(expectedDegree), analysisPair.gWithTilde.GetCoefficients(expectedDegree)),
                (synthesisPair.h.GetCoefficients(expectedDegree), synthesisPair.g.GetCoefficients(expectedDegree))
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

        /// <inheritdoc />
        public FiltersBankPolynomials GetSourceFilters(Polynomial h, int? expectedDegree = null)
        {
            if (h == null)
                throw new ArgumentNullException(nameof(h));
            if (expectedDegree.HasValue && expectedDegree.Value < 0)
                throw new ArgumentException($"{expectedDegree} must not be negative");

            var field = h.Field;
            var filtersLength = (expectedDegree ?? h.Degree) + 1;
            var g = _complementaryFiltersBuilder.Build(h, filtersLength);

            var one = new Polynomial(field, 1);
            var modularPolynomial = (one >> filtersLength) - one;

            if (filtersLength % 2 == 0)
            {
                var multiplier = new Polynomial(field, 0, 1);
                var substitution = -(one >> (filtersLength - 1));
                return new FiltersBankPolynomials(
                    filtersLength,
                    (
                        -multiplier * g.PerformVariableSubstitution(substitution) % modularPolynomial,
                        multiplier * h.PerformVariableSubstitution(substitution) % modularPolynomial
                    ),
                    (new Polynomial(h), g)
                );
            }
            
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

            return new FiltersBankPolynomials(filtersLength, (hWithTilde, gWithTilde), (new Polynomial(h), g));
        }
    }
}