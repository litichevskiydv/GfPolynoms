namespace AppliedAlgebra.GfAlgorithms.PolynomialsFactorizer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using Matrices;
    using PolynomialsGcdFinder;

    /// <summary>
    /// Polynomials factorizer based on Berlekamp's algorithm
    /// </summary>
    public class BerlekampFactorizer : IPolynomialsFactorizer
    {
        private readonly IPolynomialsGcdFinder _gcdFinder;

        /// <summary>
        /// Initializes factorizer dependencies
        /// </summary>
        /// <param name="gcdFinder">Polynomials gcd finder</param>
        public BerlekampFactorizer(IPolynomialsGcdFinder gcdFinder)
        {
            if (gcdFinder == null)
                throw new ArgumentNullException(nameof(gcdFinder));

            _gcdFinder = gcdFinder;
        }

        private IEnumerable<Polynomial> FactorizeInternal(Polynomial polynomial)
        {
            var field = polynomial.Field;
            var degree = polynomial.Degree;

            if (polynomial[polynomial.Degree] > 1)
            {
                yield return new Polynomial(field, polynomial[polynomial.Degree]);
                foreach (var factor in FactorizeInternal(polynomial / polynomial[polynomial.Degree]))
                    yield return factor;
                yield break;
            }

            if (degree <= 1 || polynomial.IsMonomial())
            {
                yield return polynomial;
                yield break;
            }

            var one = field.One();
            var currentMonomial = new Polynomial(one);
            var multiplier = new Polynomial(one).RightShift(field.Order);
            var linearSystemMatrix = new FieldElementsMatrix(field, degree, degree);
            for (var j = 0; j < linearSystemMatrix.ColumnsCount; j++, currentMonomial *= multiplier)
            {
                var matrixColumn = (currentMonomial % polynomial).GetCoefficients(degree - 1);
                for (var i = 0; i < linearSystemMatrix.RowsCount; i++)
                    linearSystemMatrix[i, j] = matrixColumn[i];

                linearSystemMatrix[j, j] -= one;
            }

            int? freeVariableIndex = null;
            var diagonalizationResult = linearSystemMatrix.DiagonalizeExtended();
            var coefficients = Enumerable.Repeat(field.Zero(), linearSystemMatrix.RowsCount).ToArray();
            for(var j = 1; j < linearSystemMatrix.ColumnsCount; j++)
                if (diagonalizationResult.SelectedRowsByColumns[j].HasValue == false)
                {
                    freeVariableIndex = j;
                    coefficients[j] = one;
                    break;
                }
            if (freeVariableIndex.HasValue == false)
            {
                yield return polynomial;
                yield break;
            }

            var diagonalizedMatrix = diagonalizationResult.Result;
            for (var j = 0; j < coefficients.Length; j++)
                if (diagonalizationResult.SelectedRowsByColumns[j].HasValue)
                {
                    var selectedRowIndex = diagonalizationResult.SelectedRowsByColumns[j].Value;
                    coefficients[j] = -diagonalizedMatrix[selectedRowIndex, freeVariableIndex.Value]
                        * coefficients[freeVariableIndex.Value] / diagonalizationResult.Result[selectedRowIndex, j];
                }
            var decomposingPolynomial = new Polynomial(coefficients);

            for (var i = 0; i < field.Order; i++)
            {
                var possibleFactor = _gcdFinder.Gcd(polynomial, decomposingPolynomial - new Polynomial(field, i));
                if(possibleFactor.Degree > 0)
                    foreach (var factor in FactorizeInternal(possibleFactor))
                        yield return factor;
            }
        }

        /// <inheritdoc />
        public IEnumerable<(Polynomial factor, int degree)> Factorize(Polynomial polynomial)
        {
            if (polynomial == null)
                throw new ArgumentNullException(nameof(polynomial));

            foreach (var factor in FactorizeInternal(polynomial))
                yield return (factor, 1);
        }
    }
}