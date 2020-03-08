﻿namespace AppliedAlgebra.GfAlgorithms.ComplementaryRepresentationFinder
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
        private readonly ILinearSystemSolver _linearSystemSolver;

        public LinearEquationsBasedFinder(ILinearSystemSolver linearSystemSolver)
        {
            if (linearSystemSolver == null)
                throw new ArgumentNullException(nameof(linearSystemSolver));

            _linearSystemSolver = linearSystemSolver;
        }

        private static IEnumerable<(FieldElement[], FieldElement[], FieldElement[], FieldElement[])> ComputePolyphaseComponentsValues(
            Polynomial fe,
            Polynomial fo,
            FieldElement lambda,
            FieldElement primitiveRoot,
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

            var field = fe.Field;
            var feValue = field.CreateElement(fe.Evaluate(argument.Representation));
            var foValue = field.CreateElement(fo.Evaluate(argument.Representation));

            if (feValue.Representation == 0 && foValue.Representation == 0)
            {
                var primitiveRootPower = FieldElement.Pow(primitiveRoot, index);
                for (var denominatorValue = 1; denominatorValue < field.Order; denominatorValue++)
                {
                    var denominator = field.CreateElement(denominatorValue);

                    for (var goValue = 1; goValue < field.Order; goValue++)
                    {
                        goValues[index] = field.CreateElement(goValue);
                        geValues[index] = denominator - primitiveRootPower * goValues[index];

                        hoValues[index] = -lambda * argument * goValues[index] - FieldElement.InverseForMultiplication(denominator);
                        heValues[index] = -lambda * argument * geValues[index] + primitiveRootPower / denominator;

                        foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, primitiveRoot, argument * argumentMultiplier,
                            argumentMultiplier, index + 1, heValues, hoValues, geValues, goValues))
                            yield return componentsValues;
                    }
                }
            }
            else
                for (var componentValue = 0; componentValue < field.Order; componentValue++)
                {
                    if (feValue.Representation == 0)
                    {
                        goValues[index] = field.CreateElement(componentValue);
                        geValues[index] = -FieldElement.InverseForMultiplication(foValue);
                    }
                    else if (foValue.Representation == 0)
                    {
                        goValues[index] = FieldElement.InverseForMultiplication(feValue);
                        geValues[index] = field.CreateElement(componentValue);

                    }
                    else
                    {
                        goValues[index] = field.CreateElement(componentValue); 
                        geValues[index] = (field.One().InverseForAddition() + goValues[index] * feValue) / foValue;
                    }

                    hoValues[index] = foValue - lambda * argument * goValues[index];
                    heValues[index] = feValue - lambda * argument * geValues[index];

                    foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, primitiveRoot, argument * argumentMultiplier,
                        argumentMultiplier, index + 1, heValues, hoValues, geValues, goValues))
                        yield return componentsValues;
                }
        }

        private static IEnumerable<(FieldElement[], FieldElement[], FieldElement[], FieldElement[])> ComputePolyphaseComponentsValues(
            Polynomial polynomial,
            int coefficientsCount,
            FieldElement lambda,
            FieldElement primitiveRoot
        )
        {
            var field = polynomial.Field;
            var (fe, fo) = polynomial.GetPolyphaseComponents();

            var valuesCount = coefficientsCount / 2;
            var heValues = new FieldElement[valuesCount];
            var hoValues = new FieldElement[valuesCount];
            var geValues = new FieldElement[valuesCount];
            var goValues = new FieldElement[valuesCount];

            
            var argument = field.One();
            var argumentMultiplier = FieldElement.Pow(primitiveRoot, 2);
            foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, primitiveRoot, argument, argumentMultiplier, 0,
                heValues, hoValues, geValues, goValues))
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

            return new Polynomial(systemSolution.VariablesValues).ChangeField(field);
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
            if(polynomial.Field.Characteristic == 2)
                throw new ArgumentException($"{nameof(polynomial)} must be over the field with odd characteristic");
            if (maxDegree <= 0)
                throw new ArgumentException($"{nameof(maxDegree)} must be positive");
            if (maxDegree < polynomial.Degree)
                throw new ArgumentException($"{nameof(maxDegree)} must not be less than {nameof(polynomial)} degree");
            if(maxDegree != polynomial.Field.Order - 2)
                throw new ArgumentException($"{nameof(maxDegree)} must be correlated with {nameof(polynomial)} field order");
            if (lambda != null && polynomial.Field.Equals(lambda.Field) == false)
                throw new ArgumentException($"{nameof(lambda)} must belong to the field of the polynomial {nameof(polynomial)}");

            var field = polynomial.Field;
            var coefficientsCount = maxDegree + 1;
            var checkedLambda = lambda ?? field.One();
            var primitiveRoot = field.GetPrimitiveRoot(coefficientsCount);
            foreach (var (heValues, hoValues, geValues, goValues) in ComputePolyphaseComponentsValues(polynomial, coefficientsCount, checkedLambda, primitiveRoot))
                yield return ReconstructComplementaryRepresentation(field, coefficientsCount, primitiveRoot, heValues, hoValues, geValues, goValues);
        }
    }
}