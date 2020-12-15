namespace AppliedAlgebra.GfAlgorithms.WaveletTransform
{
    using System;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public static class FiltersBankPolynomialsExtensions
    {
        /// <summary>
        /// Transforms filters bank polynomials to vectors
        /// </summary>
        /// <param name="filtersBank">Transformable filters bank</param>
        public static FiltersBankVectors ToFiltersBankVectors(this FiltersBankPolynomials filtersBank)
        {
            if(filtersBank == null)
                throw new ArgumentNullException(nameof(filtersBank));

            return new FiltersBankVectors(
                (
                    filtersBank.AnalysisPair.hWithTilde.GetCoefficients(filtersBank.FiltersLength - 1),
                    filtersBank.AnalysisPair.gWithTilde.GetCoefficients(filtersBank.FiltersLength - 1)
                ),
                (
                    filtersBank.SynthesisPair.h.GetCoefficients(filtersBank.FiltersLength - 1),
                    filtersBank.SynthesisPair.g.GetCoefficients(filtersBank.FiltersLength - 1)
                )
            );
        }

        /// <summary>
        /// checks if the filter bank can perform perfect reconstruction
        /// </summary>
        /// <param name="filtersBank">Verifiable filter bank</param>
        /// <param name="multiplier">Condition multiplier</param>
        public static bool CanPerformPerfectReconstruction(this FiltersBankPolynomials filtersBank, FieldElement multiplier = null)
        {
            if (filtersBank == null)
                throw new ArgumentNullException(nameof(filtersBank));

            var (hWithTildeEvenComponent, hWithTildeOddComponent) = filtersBank.AnalysisPair.hWithTilde.GetPolyphaseComponents();
            var (gWithTildeEvenComponent, gWithTildeOddComponent) = filtersBank.AnalysisPair.gWithTilde.GetPolyphaseComponents();
            var (hEvenComponent, hOddComponent) = filtersBank.SynthesisPair.h.GetPolyphaseComponents();
            var (gEvenComponent, gOddComponent) = filtersBank.SynthesisPair.g.GetPolyphaseComponents();

            var field = hEvenComponent.Field;
            var zero = new Polynomial(field);
            var one = new Polynomial(field, 1);
            var polyphaseComponentsLength = filtersBank.FiltersLength / 2;
            var modularPolynomial = (one >> polyphaseComponentsLength) - one;
            var checkedMultiplier = multiplier != null ? FieldElement.InverseForMultiplication(multiplier) : field.One();

            return one.Equals(
                       checkedMultiplier.Representation * (
                           hEvenComponent * hWithTildeEvenComponent.RaiseVariableDegree(polyphaseComponentsLength - 1)
                           + gEvenComponent * gWithTildeEvenComponent.RaiseVariableDegree(polyphaseComponentsLength - 1)
                       ) % modularPolynomial
                   ) &&
                   one.Equals(
                       checkedMultiplier.Representation * (
                           hOddComponent * hWithTildeOddComponent.RaiseVariableDegree(polyphaseComponentsLength - 1)
                           + gOddComponent * gWithTildeOddComponent.RaiseVariableDegree(polyphaseComponentsLength - 1)
                       ) % modularPolynomial
                   ) &&
                   zero.Equals(
                       (
                           hEvenComponent * hWithTildeOddComponent.RaiseVariableDegree(polyphaseComponentsLength - 1)
                           + gEvenComponent * gWithTildeOddComponent.RaiseVariableDegree(polyphaseComponentsLength - 1)
                       ) % modularPolynomial
                   ) &&
                   zero.Equals(
                       (
                           hOddComponent * hWithTildeEvenComponent.RaiseVariableDegree(polyphaseComponentsLength - 1)
                           + gOddComponent * gWithTildeEvenComponent.RaiseVariableDegree(polyphaseComponentsLength - 1)
                       ) % modularPolynomial
                   );
        }
    }
}