namespace AppliedAlgebra.GfAlgorithms.ComplementaryRepresentationFinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using LinearSystemSolver;

    public class LinearEquationsBasedFinder : IComplementaryRepresentationFinder
    {
        private class ValueSource
        {
            public int SubfieldOrder { get; }

            public (int index, int degree)? Reference { get; }

            public ValueSource(int subfieldOrder)
            {
                SubfieldOrder = subfieldOrder;
            }

            public ValueSource(int subfieldOrder, int referenceValueIndex, int referenceValueDegree)
            {
                SubfieldOrder = subfieldOrder;
                Reference = (referenceValueIndex, referenceValueDegree);
            }

            public override string ToString() =>
                $"Field order: {SubfieldOrder}" +
                (
                    Reference != null
                        ? $"; Reference value: index {Reference.Value.index}, degree: {Reference.Value.degree}"
                        : ""
                );
        }

        private readonly ILinearSystemSolver _linearSystemSolver;

        private static int GetPower(int a, int n)
        {
            var result = 1;
            while (n > 0)
            {
                if ((n & 1) == 1)
                    result *= a;

                a *= a;
                n >>= 1;
            }

            return result;
        }

        private static IReadOnlyDictionary<int, ValueSource> PrepareValuesSources(GaloisField field, int coefficientsCount)
        {
            var valuesSources = new Dictionary<int, ValueSource>();
            var conjugacyClasses = field.GenerateConjugacyClasses(coefficientsCount);
            foreach (var conjugacyClass in conjugacyClasses)
            {
                var subfieldOrder = GetPower(field.Order, conjugacyClass.Length);
                for (var i = 0; i < conjugacyClass.Length; i++)
                {
                    var valueIndex = conjugacyClass[i];
                    if (i == 0)
                        valuesSources[valueIndex] = new ValueSource(subfieldOrder);
                    else
                        valuesSources[valueIndex] = new ValueSource(subfieldOrder, conjugacyClass[0], GetPower(field.Order, i));
                }
            }

            return valuesSources;
        }

        private static FieldElement GetSubfieldElement(GaloisField field, int subfieldOrder, int elementRepresentation) =>
            elementRepresentation == 0 ? field.Zero() : field.GetPrimitiveRoot(subfieldOrder - 1).Pow(elementRepresentation - 1);

        private static IEnumerable<(FieldElement[], FieldElement[], FieldElement[], FieldElement[])> ComputePolyphaseComponentsValues(
            Polynomial fe,
            Polynomial fo,
            FieldElement lambda,
            FieldElement primitiveRoot,
            IReadOnlyDictionary<int, ValueSource> valuesSources,
            FieldElement argument,
            FieldElement argumentMultiplier,
            int index,
            FieldElement[] heValues,
            FieldElement[] hoValues,
            FieldElement[] geValues,
            FieldElement[] goValues
        )
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
                    argument * argumentMultiplier, argumentMultiplier, index + 1, heValues, hoValues, geValues, goValues))
                    yield return componentsValues;
                yield break;
            }

            var field = fe.Field;
            var feValue = field.CreateElement(fe.Evaluate(argument.Representation));
            var foValue = field.CreateElement(fo.Evaluate(argument.Representation));

            if (feValue.Representation == 0 && foValue.Representation == 0)
            {
                var primitiveRootPower = FieldElement.Pow(primitiveRoot, index);
                for (var goValue = 1; goValue < valueSource.SubfieldOrder; goValue++)
                    for (var geValue = 0; geValue < valueSource.SubfieldOrder; geValue++)
                    {
                        goValues[index] = GetSubfieldElement(field, valueSource.SubfieldOrder, goValue);
                        geValues[index] = GetSubfieldElement(field, valueSource.SubfieldOrder, geValue);

                        var denominator = geValues[index] + primitiveRootPower * goValues[index];
                        if (denominator.Representation == 0) continue;


                        hoValues[index] = -lambda * argument * goValues[index] - FieldElement.InverseForMultiplication(denominator);
                        heValues[index] = -lambda * argument * geValues[index] + primitiveRootPower / denominator;

                        foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, primitiveRoot, valuesSources,
                            argument * argumentMultiplier, argumentMultiplier, index + 1, heValues, hoValues, geValues, goValues))
                            yield return componentsValues;
                    }
            }
            else
                for (var componentValue = 0; componentValue < valueSource.SubfieldOrder; componentValue++)
                {
                    if (feValue.Representation == 0)
                    {
                        goValues[index] = GetSubfieldElement(field, valueSource.SubfieldOrder, componentValue);
                        geValues[index] = -FieldElement.InverseForMultiplication(foValue);
                    }
                    else if (foValue.Representation == 0)
                    {
                        goValues[index] = FieldElement.InverseForMultiplication(feValue);
                        geValues[index] = GetSubfieldElement(field, valueSource.SubfieldOrder, componentValue);

                    }
                    else
                    {
                        goValues[index] = GetSubfieldElement(field, valueSource.SubfieldOrder, componentValue);
                        geValues[index] = (field.One().InverseForAddition() + goValues[index] * feValue) / foValue;
                    }

                    hoValues[index] = foValue - lambda * argument * goValues[index];
                    heValues[index] = feValue - lambda * argument * geValues[index];

                    foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, primitiveRoot, valuesSources,
                        argument * argumentMultiplier, argumentMultiplier, index + 1, heValues, hoValues, geValues, goValues))
                        yield return componentsValues;
                }
        }

        private static IEnumerable<(FieldElement[], FieldElement[], FieldElement[], FieldElement[])> ComputePolyphaseComponentsValues(
            Polynomial polynomial,
            int coefficientsCount,
            FieldElement lambda,
            FieldElement primitiveRoot,
            IReadOnlyDictionary<int, ValueSource> valuesSources
        )
        {
            var (fe, fo) = polynomial.GetPolyphaseComponents();

            var argument = polynomial.Field.One();
            var argumentMultiplier = FieldElement.Pow(primitiveRoot, 2);

            var valuesCount = coefficientsCount / 2;
            var heValues = new FieldElement[valuesCount];
            var hoValues = new FieldElement[valuesCount];
            var geValues = new FieldElement[valuesCount];
            var goValues = new FieldElement[valuesCount];

            foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, primitiveRoot, valuesSources,
                argument, argumentMultiplier, 0, heValues, hoValues, geValues, goValues))
                yield return componentsValues;
        }

        private Polynomial ReconstructPolynomialByValues(
            GaloisField field,
            int coefficientsCount,
            FieldElement argumentMultiplier,
            FieldElement[] values
        )
        {
            var argument = argumentMultiplier.Field.One();
            var a = new FieldElement[values.Length, coefficientsCount];
            for (var i = 0; i < a.GetLength(0); i++, argument.Multiply(argumentMultiplier))
            {
                a[i, 0] = argumentMultiplier.Field.One();
                for (var j = 1; j < a.GetLength(1); j++)
                    a[i, j] = argument * a[i, j - 1];
            }

            var systemSolution = _linearSystemSolver.Solve(a, values.Select(x => new FieldElement(x)).ToArray());
            if (systemSolution.IsCorrect == false)
                throw new InvalidOperationException("Can't reconstruct polynomial by values");

            return new Polynomial(field, systemSolution.VariablesValues
                .Select(x =>
                        {
                            if (x.Representation == 0) return 0;

                            var degreeDelta = (x.Field.Order - 1) / (field.Order - 1);
                            return field.PowGeneratingElement(x.Field.GetGeneratingElementDegree(x.Representation) / degreeDelta);
                        })
                .ToArray()
            );
        }

        private (Polynomial h, Polynomial g) ReconstructComplementaryRepresentation(
            GaloisField field,
            int coefficientsCount,
            FieldElement primitiveRoot,
            FieldElement[] heValues,
            FieldElement[] hoValues,
            FieldElement[] geValues,
            FieldElement[] goValues
        )
        {
            var componentsCoefficientsCount = coefficientsCount / 2;
            var argumentMultiplier = FieldElement.Pow(primitiveRoot, 2);
            var he = ReconstructPolynomialByValues(field, componentsCoefficientsCount, argumentMultiplier, heValues);
            var ho = ReconstructPolynomialByValues(field, componentsCoefficientsCount, argumentMultiplier, hoValues);
            var ge = ReconstructPolynomialByValues(field, componentsCoefficientsCount, argumentMultiplier, geValues);
            var go = ReconstructPolynomialByValues(field, componentsCoefficientsCount, argumentMultiplier, goValues);

            return
            (
                PolynomialsAlgorithmsExtensions.CreateFormPolyphaseComponents(he, ho),
                PolynomialsAlgorithmsExtensions.CreateFormPolyphaseComponents(ge, go)
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
            var coefficientsCount = maxDegree + 1;

            var fieldExtension = field.FindExtensionContainingPrimitiveRoot(coefficientsCount);
            var primitiveRoot = fieldExtension.GetPrimitiveRoot(coefficientsCount);
            var valuesReferences = PrepareValuesSources(field, coefficientsCount / 2);

            var checkedPolynomial = polynomial.TransferToSubfield(fieldExtension);
            var checkedLambda = (lambda ?? field.One()).TransferToSubfield(fieldExtension);

            foreach (var (heValues, hoValues, geValues, goValues) in ComputePolyphaseComponentsValues(checkedPolynomial, coefficientsCount, checkedLambda, primitiveRoot, valuesReferences))
                yield return ReconstructComplementaryRepresentation(field, coefficientsCount, primitiveRoot, heValues, hoValues, geValues, goValues);
        }

        public LinearEquationsBasedFinder(ILinearSystemSolver linearSystemSolver)
        {
            if (linearSystemSolver == null)
                throw new ArgumentNullException(nameof(linearSystemSolver));

            _linearSystemSolver = linearSystemSolver;
        }
    }
}