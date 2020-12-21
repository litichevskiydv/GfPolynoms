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
            if(filtersBank.FiltersLength % 2 == 1)
                throw new ArgumentException("Operation can be performed only for filters of even length");

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
            var one = new Polynomial(field, 1);
            var componentVariableDegree = filtersBank.FiltersLength % 2 + 1;
            var modularPolynomialDegree = filtersBank.FiltersLength % 2 == 0 ? filtersBank.FiltersLength / 2 : filtersBank.FiltersLength;
            var modularPolynomial = (one >> modularPolynomialDegree) - one;
            var checkedMultiplier = multiplier != null ? FieldElement.InverseForMultiplication(multiplier) : field.One();

            var zero = new Polynomial(field);
            return one.Equals(
                       checkedMultiplier.Representation 
                       * (
                           hEvenComponent.RaiseVariableDegree(componentVariableDegree) * hWithTildeEvenComponent.RaiseVariableDegree(modularPolynomialDegree - 1)
                           + gEvenComponent.RaiseVariableDegree(componentVariableDegree) * gWithTildeEvenComponent.RaiseVariableDegree(modularPolynomialDegree - 1)
                       ) % modularPolynomial
                   ) &&
                   one.Equals(
                       checkedMultiplier.Representation * (
                           hOddComponent.RaiseVariableDegree(componentVariableDegree) * hWithTildeOddComponent.RaiseVariableDegree(modularPolynomialDegree - 1)
                           + gOddComponent.RaiseVariableDegree(componentVariableDegree) * gWithTildeOddComponent.RaiseVariableDegree(modularPolynomialDegree - 1)
                       ) % modularPolynomial
                   ) &&
                   zero.Equals(
                       (
                           hEvenComponent.RaiseVariableDegree(componentVariableDegree) * hWithTildeOddComponent.RaiseVariableDegree(modularPolynomialDegree - 1)
                           + gEvenComponent.RaiseVariableDegree(componentVariableDegree) * gWithTildeOddComponent.RaiseVariableDegree(modularPolynomialDegree - 1)
                       ) % modularPolynomial
                   ) &&
                   zero.Equals(
                       (
                           hOddComponent.RaiseVariableDegree(componentVariableDegree) * hWithTildeEvenComponent.RaiseVariableDegree(modularPolynomialDegree - 1)
                           + gOddComponent.RaiseVariableDegree(componentVariableDegree) * gWithTildeEvenComponent.RaiseVariableDegree(modularPolynomialDegree - 1)
                       ) % modularPolynomial
                   );
        }
    }
}