﻿namespace AppliedAlgebra.GfAlgorithms.ComplementaryRepresentationFinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using PolynomialExtensions = Extensions.PolynomialExtensions;

    public class LinearEquationsBasedFinder : IComplementaryRepresentationFinder
    {
        private class ValueSource
        {
            public GaloisField Field { get; }

            public (int index, int degree)? Reference { get; }

            public ValueSource(GaloisField field)
            {
                Field = field;
            }

            public ValueSource(GaloisField field, int referenceValueIndex, int referenceValueDegree)
            {
                Field = field;
                Reference = (referenceValueIndex, referenceValueDegree);
            }

            public override string ToString() =>
                $"Subfield: {Field}" +
                (
                    Reference != null
                        ? $"; Reference value: index {Reference.Value.index}, degree: {Reference.Value.degree}"
                        : ""
                );
        }

        private static IReadOnlyDictionary<int, ValueSource> PrepareValuesSources(GaloisField field, int coefficientsCount)
        {
            var valuesSources = new Dictionary<int, ValueSource>();
            var conjugacyClasses = field.GenerateConjugacyClasses(coefficientsCount);
            foreach (var conjugacyClass in conjugacyClasses)
            {
                var conjugacyClassField = GaloisField.Create(field.Order.Pow(conjugacyClass.Length));
                for (var i = 0; i < conjugacyClass.Length; i++)
                {
                    var valueIndex = conjugacyClass[i];
                    if (i == 0)
                        valuesSources[valueIndex] = new ValueSource(conjugacyClassField);
                    else
                        valuesSources[valueIndex] = new ValueSource(conjugacyClassField, conjugacyClass[0], field.Order.Pow(i));
                }
            }

            return valuesSources;
        }

        private static FieldElement GetSubfieldElement(GaloisField field, int fieldElement, GaloisField fieldExtension) =>
            fieldExtension.CreateElement(field.TransferElementToSubfield(fieldElement, fieldExtension));

        private static IEnumerable<(FieldElement[], FieldElement[], FieldElement[], FieldElement[])> ComputePolyphaseComponentsValues(
            Polynomial fe,
            Polynomial fo,
            FieldElement lambda,
            FieldElement primitiveRoot,
            IReadOnlyDictionary<int, ValueSource> valuesSources,
            FieldElement argument,
            int index,
            FieldElement[] heValues,
            FieldElement[] hoValues,
            FieldElement[] geValues,
            FieldElement[] goValues)
        {
            if (index == heValues.Length)
            {
                yield return (heValues, hoValues, geValues, goValues);
                yield break;
            }

            var valueSource = valuesSources[index];
            if (valueSource.Reference != null)
            {
                var (valueIndex, valueDegree) = valueSource.Reference.Value;

                goValues[index] = FieldElement.Pow(goValues[valueIndex], valueDegree);
                geValues[index] = FieldElement.Pow(geValues[valueIndex], valueDegree);

                hoValues[index] = FieldElement.Pow(hoValues[valueIndex], valueDegree);
                heValues[index] = FieldElement.Pow(heValues[valueIndex], valueDegree);

                foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, primitiveRoot, valuesSources,
                    argument * primitiveRoot, index + 1, heValues, hoValues, geValues, goValues))
                    yield return componentsValues;
                yield break;
            }

            var fieldExtension = fe.Field;
            var feValue = fieldExtension.CreateElement(fe.Evaluate(argument.Representation));
            var foValue = fieldExtension.CreateElement(fo.Evaluate(argument.Representation));

            for (var componentValue = 0; componentValue < valueSource.Field.Order; componentValue++)
            {
                if (feValue.Representation == 0)
                {
                    goValues[index] = GetSubfieldElement(valueSource.Field, componentValue, fieldExtension);
                    geValues[index] = -FieldElement.InverseForMultiplication(foValue);
                }
                else if (foValue.Representation == 0)
                {
                    goValues[index] = FieldElement.InverseForMultiplication(feValue);
                    geValues[index] = GetSubfieldElement(valueSource.Field, componentValue, fieldExtension);

                }
                else
                {
                    goValues[index] = GetSubfieldElement(valueSource.Field, componentValue, fieldExtension);
                    geValues[index] = (fieldExtension.One().InverseForAddition() + goValues[index] * feValue) / foValue;
                }

                hoValues[index] = foValue - lambda * argument * goValues[index];
                heValues[index] = feValue - lambda * argument * geValues[index];

                foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, primitiveRoot, valuesSources,
                    argument * primitiveRoot, index + 1, heValues, hoValues, geValues, goValues))
                    yield return componentsValues;
            }
        }

        private static IEnumerable<(FieldElement[], FieldElement[], FieldElement[], FieldElement[])> ComputePolyphaseComponentsValues(
            Polynomial polynomial,
            int componentsCoefficientsCount,
            FieldElement lambda,
            FieldElement primitiveRoot,
            IReadOnlyDictionary<int, ValueSource> valuesSources
        )
        {
            var argument = polynomial.Field.One();

            var (fe, fo) = polynomial.GetPolyphaseComponents();

            var heValues = new FieldElement[componentsCoefficientsCount];
            var hoValues = new FieldElement[componentsCoefficientsCount];
            var geValues = new FieldElement[componentsCoefficientsCount];
            var goValues = new FieldElement[componentsCoefficientsCount];

            foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, primitiveRoot, valuesSources, argument, 0, heValues, hoValues, geValues, goValues))
                yield return componentsValues;
        }

        private static Polynomial ReconstructPolynomialBySpectrum(GaloisField field, FieldElement[] values) =>
            new Polynomial(values.GetSignal().Select(x => x.TransferFromSubfield(field)).ToArray());

        private static (Polynomial h, Polynomial g) ReconstructComplementaryRepresentation(
            GaloisField field,
            FieldElement[] heValues,
            FieldElement[] hoValues,
            FieldElement[] geValues,
            FieldElement[] goValues
        )
        {
            var he = ReconstructPolynomialBySpectrum(field, heValues);
            var ho = ReconstructPolynomialBySpectrum(field, hoValues);
            var ge = ReconstructPolynomialBySpectrum(field, geValues);
            var go = ReconstructPolynomialBySpectrum(field, goValues);

            return
            (
                PolynomialExtensions.CreateFormPolyphaseComponents(he, ho),
                PolynomialExtensions.CreateFormPolyphaseComponents(ge, go)
            );
        }

        /// <inheritdoc />
        public IEnumerable<(Polynomial h, Polynomial g)> Find(Polynomial polynomial, int maxDegree, FieldElement lambda = null)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (polynomial.Field.Characteristic == 2)
                throw new ArgumentException($"{nameof(polynomial)} must be over the field with odd characteristic");
            if (maxDegree <= 0)
                throw new ArgumentException($"{nameof(maxDegree)} must be positive");
            if (maxDegree < polynomial.Degree)
                throw new ArgumentException($"{nameof(maxDegree)} must not be less than {nameof(polynomial)} degree");
            if (maxDegree % 2 == 0)
                throw new ArgumentException($"{nameof(maxDegree)} must be odd value");
            if (lambda != null && polynomial.Field.Equals(lambda.Field) == false)
                throw new ArgumentException($"{nameof(lambda)} must belong to the field of the polynomial {nameof(polynomial)}");

            var field = polynomial.Field;
            var componentsCoefficientsCount = (maxDegree + 1) / 2;

            var fieldExtension = field.FindExtensionContainingPrimitiveRoot(componentsCoefficientsCount);
            var primitiveRoot = fieldExtension.GetPrimitiveRoot(componentsCoefficientsCount);
            var valuesReferences = PrepareValuesSources(field, componentsCoefficientsCount);

            var checkedPolynomial = polynomial.TransferToSubfield(fieldExtension);
            var checkedLambda = (lambda ?? field.One()).TransferToSubfield(fieldExtension);

            var (checkedPolynomialEvenComponent, checkedPolynomialOddComponent) = checkedPolynomial.GetPolyphaseComponents();
            for (int i = 0, argument = 1; i < componentsCoefficientsCount; i++, argument = fieldExtension.Multiply(argument, primitiveRoot.Representation))
                if (checkedPolynomialEvenComponent.Evaluate(argument) == 0 && checkedPolynomialOddComponent.Evaluate(argument) == 0)
                    throw new ArgumentException($"Even and odd component of {polynomial} can't have zero in one point simultaneously");

            foreach (var (heValues, hoValues, geValues, goValues) in ComputePolyphaseComponentsValues(checkedPolynomial, componentsCoefficientsCount, checkedLambda, primitiveRoot, valuesReferences))
                yield return ReconstructComplementaryRepresentation(field, heValues, hoValues, geValues, goValues);
        }
    }
}