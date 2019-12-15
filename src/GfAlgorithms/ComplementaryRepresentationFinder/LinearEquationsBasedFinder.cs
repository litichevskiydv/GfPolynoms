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
            int maxDegree,
            FieldElement lambda
        )
        {
            var (fe, fo) = polynomial.GetPolyphaseComponents();

            var componentsMaxDegree = (maxDegree + 1) / 2 - 1;
            var heValues = new FieldElement[componentsMaxDegree + 1];
            var hoValues = new FieldElement[componentsMaxDegree + 1];
            var geValues = new FieldElement[componentsMaxDegree + 1];
            var goValues = new FieldElement[componentsMaxDegree + 1];

            var field = polynomial.Field;
            var argument = field.One();
            var argumentMultiplier = field.CreateElement(field.GetGeneratingElementPower(2));

            foreach (var componentsValues in ComputePolyphaseComponentsValues(fe, fo, lambda, argument, argumentMultiplier, 0,
                heValues, hoValues, geValues, goValues))
                yield return componentsValues;
        }

        private Polynomial ReconstructPolynomialByValues(FieldElement[] values)
        {
            var field = values[0].Field;
            var argument = field.One();
            var argumentMultiplier = field.CreateElement(field.GetGeneratingElementPower(2));

            var a = new FieldElement[values.Length, values.Length];
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

        public IEnumerable<(Polynomial h, Polynomial g)> Find(Polynomial polynomial, int maxDegree)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));
            if (maxDegree <= 0)
                throw new ArgumentException($"{nameof(maxDegree)} must be positive");
            if (maxDegree < polynomial.Degree)
                throw new ArgumentException($"{nameof(maxDegree)} must not be less than {nameof(polynomial)} degree");
            if(maxDegree != polynomial.Field.Order - 2)
                throw new ArgumentException($"{nameof(maxDegree)} must be correlated with {nameof(polynomial)} field order");

            return ComputePolyphaseComponentsValues(polynomial, maxDegree, polynomial.Field.One())
                .Select(x =>
                        {
                            var (heValues, hoValues, geValues, goValues) = x;

                            var he = ReconstructPolynomialByValues(heValues);
                            var ho = ReconstructPolynomialByValues(hoValues);
                            var ge = ReconstructPolynomialByValues(geValues);
                            var go = ReconstructPolynomialByValues(goValues);

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