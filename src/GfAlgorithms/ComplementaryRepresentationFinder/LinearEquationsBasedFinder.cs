namespace AppliedAlgebra.GfAlgorithms.ComplementaryRepresentationFinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
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
                for (var denominatorValue = 1; denominatorValue < field.Order; denominatorValue++)
                {
                    var denominator = field.CreateElement(denominatorValue);
                    var generatingElementPower = field.CreateElement(field.GetGeneratingElementPower(index));

                    for (var goValue = 1; goValue < field.Order; goValue++)
                    {
                        goValues[index] = field.CreateElement(goValue);
                        geValues[index] = denominator - generatingElementPower * goValues[index];

                        hoValues[index] = lambda.InverseForAddition() * argument * goValues[index] + field.One().InverseForAddition() / denominator;
                        heValues[index] = lambda.InverseForAddition() * argument * geValues[index] +
                                      generatingElementPower.InverseForAddition() / denominator;

                        foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, argument * argumentMultiplier,
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
                        geValues[index] = field.One().InverseForAddition() / foValue;
                    }
                    else if (foValue.Representation == 0)
                    {
                        goValues[index] = feValue.InverseForMultiplication();
                        geValues[index] = field.CreateElement(componentValue);

                    }
                    else
                    {
                        goValues[index] = field.CreateElement(componentValue); 
                        geValues[index] = (field.One().InverseForAddition() + goValues[index] * feValue) / foValue;
                    }

                    hoValues[index] = foValue - lambda * argument * goValues[index];
                    heValues[index] = feValue - lambda * argument * geValues[index];

                    foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, argument * argumentMultiplier,
                        argumentMultiplier, index + 1, heValues, hoValues, geValues, goValues))
                        yield return componentsValues;
                }
        }

        private static IEnumerable<(FieldElement[], FieldElement[], FieldElement[], FieldElement[])> ComputePolyphaseComponentsValues(
            Polynomial polynomial,
            FieldElement lambda
        )
        {
            var field = polynomial.Field;
            var (fe, fo) = polynomial.GetPolyphaseComponents();

            var valuesCount = (field.Order - 1) / 2;
            var heValues = new FieldElement[valuesCount];
            var hoValues = new FieldElement[valuesCount];
            var geValues = new FieldElement[valuesCount];
            var goValues = new FieldElement[valuesCount];

            
            var argument = field.One();
            var argumentMultiplier = field.CreateElement(field.GetGeneratingElementPower(2));
            foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, argument, argumentMultiplier, 0,
                heValues, hoValues, geValues, goValues))
                yield return componentsValues;
        }

        private Polynomial ReconstructPolynomialByValues(FieldElement[] values, int coefficientsCount)
        {
            var field = values[0].Field;
            var argument = field.One();
            var argumentMultiplier = field.CreateElement(field.GetGeneratingElementPower(2));

            var a = new FieldElement[values.Length, coefficientsCount];
            for (var i = 0; i < a.GetLength(0); i++, argument.Multiply(argumentMultiplier))
            {
                a[i, 0] = field.One();
                for (var j = 1; j < a.GetLength(1); j++)
                    a[i, j] = argument * a[i, j - 1];
            }

            var systemSolution = _linearSystemSolver.Solve(a, values.Select(x => new FieldElement(x)).ToArray());
            if (systemSolution.IsCorrect == false)
                throw new InvalidOperationException("Can't reconstruct polynomial by values");

            return new Polynomial(systemSolution.VariablesValues);
        }

        /// <inheritdoc />
        public IEnumerable<(Polynomial h, Polynomial g)> Find(Polynomial polynomial, int maxDegree, FieldElement lambda = null)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (maxDegree <= 0)
                throw new ArgumentException($"{nameof(maxDegree)} must be positive");
            if (maxDegree < polynomial.Degree)
                throw new ArgumentException($"{nameof(maxDegree)} must not be less than {nameof(polynomial)} degree");
            if(maxDegree != polynomial.Field.Order - 2)
                throw new ArgumentException($"{nameof(maxDegree)} must be correlated with {nameof(polynomial)} field order");
            if (lambda != null && polynomial.Field.Equals(lambda.Field) == false)
                throw new ArgumentException($"{nameof(lambda)} must belong to the field of the polynomial {nameof(polynomial)}");

            var field = polynomial.Field;
            return ComputePolyphaseComponentsValues(polynomial, lambda ?? field.One())
                .Select(x =>
                        {
                            var (heValues, hoValues, geValues, goValues) = x;
                            var evenComponentCoefficientsCount = (field.Order - 1) / 2;
                            var oddComponentCoefficientsCount = (field.Order - 1) / 2;

                            var he = ReconstructPolynomialByValues(heValues, evenComponentCoefficientsCount);
                            var ho = ReconstructPolynomialByValues(hoValues, oddComponentCoefficientsCount);
                            var ge = ReconstructPolynomialByValues(geValues, evenComponentCoefficientsCount);
                            var go = ReconstructPolynomialByValues(goValues, oddComponentCoefficientsCount);

                            return
                            (
                                PolynomialsAlgorithmsExtensions.CreateFormPolyphaseComponents(he, ho),
                                PolynomialsAlgorithmsExtensions.CreateFormPolyphaseComponents(ge, go)
                            );
                        }
                );
        }
    }
}