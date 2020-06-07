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
        private readonly DiagonalizationOptions _diagonalizationOptions;

        /// <summary>
        /// Initializes factorizer dependencies
        /// </summary>
        /// <param name="gcdFinder">Polynomials gcd finder</param>
        /// <param name="options">Factorization options</param>
        public BerlekampFactorizer(IPolynomialsGcdFinder gcdFinder, BerlekampFactorizerOptions options = null)
        {
            if (gcdFinder == null)
                throw new ArgumentNullException(nameof(gcdFinder));

            _gcdFinder = gcdFinder;

            var opts = options ?? new BerlekampFactorizerOptions();
            _diagonalizationOptions = new DiagonalizationOptions {MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism};
        }

        private static FieldElementsMatrix PrepareLinearSystemMatrix(Polynomial polynomial)
        {
            var one = polynomial.Field.One();
            var currentMonomial = new Polynomial(one);
            var multiplier = new Polynomial(one).RightShift(polynomial.Field.Order);
            var linearSystemMatrix = new FieldElementsMatrix(polynomial.Field, polynomial.Degree, polynomial.Degree);
            for (var j = 0; j < linearSystemMatrix.ColumnsCount; j++, currentMonomial *= multiplier)
            {
                var matrixColumn = (currentMonomial % polynomial).GetCoefficients(polynomial.Degree - 1);
                for (var i = 0; i < linearSystemMatrix.RowsCount; i++)
                    linearSystemMatrix[i, j] = matrixColumn[i];

                linearSystemMatrix[j, j] -= one;
            }

            return linearSystemMatrix;
        }

        private Polynomial FindDecomposingPolynomial(FieldElementsMatrix linearSystemMatrix)
        {
            var diagonalizationResult = linearSystemMatrix.DiagonalizeExtended(_diagonalizationOptions);

            int? freeVariableIndex = null;
            var coefficients = Enumerable.Repeat(linearSystemMatrix.Field.Zero(), linearSystemMatrix.RowsCount).ToArray();
            for (var j = 1; j < linearSystemMatrix.ColumnsCount; j++)
                if (diagonalizationResult.SelectedRowsByColumns[j].HasValue == false)
                {
                    freeVariableIndex = j;
                    coefficients[j] = linearSystemMatrix.Field.One();
                    break;
                }
            if (freeVariableIndex.HasValue == false)
                return null;

            var diagonalizedMatrix = diagonalizationResult.Result;
            for (var j = 0; j < coefficients.Length; j++)
                if (diagonalizationResult.SelectedRowsByColumns[j].HasValue)
                {
                    var selectedRowIndex = diagonalizationResult.SelectedRowsByColumns[j].Value;
                    coefficients[j] = -diagonalizedMatrix[selectedRowIndex, freeVariableIndex.Value]
                        * coefficients[freeVariableIndex.Value] / diagonalizationResult.Result[selectedRowIndex, j];
                }

            return new Polynomial(coefficients);
        }

        private IEnumerable<Polynomial> FactorizeInternal(Polynomial polynomial)
        {
            var field = polynomial.Field;
            var degree = polynomial.Degree;

            if (polynomial[degree] > 1)
            {
                foreach (var factor in FactorizeInternal(polynomial / polynomial[degree]))
                    yield return factor;
                yield break;
            }

            if (degree <= 1 || polynomial.IsMonomial())
            {
                yield return polynomial;
                yield break;
            }


            var decomposingPolynomial = FindDecomposingPolynomial(PrepareLinearSystemMatrix(polynomial));
            if (decomposingPolynomial == null)
            {
                yield return polynomial;
                yield break;
            }

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

            if (polynomial[polynomial.Degree] > 1)
                yield return (new Polynomial(polynomial.Field, polynomial[polynomial.Degree]), 1);
            foreach (var factor in FactorizeInternal(polynomial))
                yield return (factor, 1);
        }
    }
}