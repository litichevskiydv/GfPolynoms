namespace AppliedAlgebra.GfAlgorithms.ComplementaryRepresentationFinder
{
    using System;
    using System.Collections.Generic;
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

        private static (FieldElement[], FieldElement[], FieldElement[], FieldElement[]) ComputePolyphaseComponentsValues(
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
            for (var i = 0; i <= componentsMaxDegree; i++, argument.Multiply(argumentMultiplier))
            {
                var feValue = field.CreateElement(fe.Evaluate(argument.Representation));
                var foValue = field.CreateElement(fo.Evaluate(argument.Representation));

                if (feValue.Representation == 0 && foValue.Representation == 0)
                {
                    var denominator = field.One();
                    var generatingElementPower = field.CreateElement(field.GetGeneratingElementPower(i));


                    goValues[i] = field.One();
                    geValues[i] = denominator - generatingElementPower * goValues[i];

                    hoValues[i] = lambda.InverseForAddition() * argument * goValues[i] + field.One().InverseForAddition() / denominator;
                    heValues[i] = lambda.InverseForAddition() * argument * geValues[i] +
                                  generatingElementPower.InverseForAddition() / denominator;
                }
                else
                {
                    if (feValue.Representation == 0)
                    {
                        goValues[i] = field.One();
                        geValues[i] = field.One().InverseForAddition() / foValue;
                    }
                    else if (foValue.Representation == 0)
                    {
                        goValues[i] = feValue.InverseForMultiplication();
                        geValues[i] = field.One();

                    }
                    else
                    {
                        goValues[i] = field.One();
                        geValues[i] = (field.One().InverseForAddition() + goValues[i] * feValue) / foValue;
                    }

                    hoValues[i] = foValue - lambda * argument * goValues[i];
                    heValues[i] = feValue - lambda * argument * geValues[i];
                }
            }

            return (heValues, hoValues, geValues, goValues);
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

            var systemSolution = _linearSystemSolver.Solve(a, values);
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

            var (heValues, hoValues, geValues, goValues) = ComputePolyphaseComponentsValues(polynomial, maxDegree, polynomial.Field.One());

            var he = ReconstructPolynomialByValues(heValues);
            var ho = ReconstructPolynomialByValues(hoValues);
            var ge = ReconstructPolynomialByValues(geValues);
            var go = ReconstructPolynomialByValues(goValues);

            return new[]
                   {
                       (
                           PolynomialsAlgorithmsExtensions.CreateFormPolyphaseComponents(he, ho),
                           PolynomialsAlgorithmsExtensions.CreateFormPolyphaseComponents(ge, go)
                       )
                   };
        }
    }
}