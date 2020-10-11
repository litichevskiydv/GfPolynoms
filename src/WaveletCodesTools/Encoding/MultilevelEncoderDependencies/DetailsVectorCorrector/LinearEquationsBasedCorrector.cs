namespace AppliedAlgebra.WaveletCodesTools.Encoding.MultilevelEncoderDependencies.DetailsVectorCorrector
{
    using System;
    using System.Linq;
    using GfAlgorithms.LinearSystemSolver;
    using GfAlgorithms.Matrices;
    using GfPolynoms;

    /// <summary>
    /// Details vector corrector implementation based on linear equations system solving 
    /// </summary>
    public class LinearEquationsBasedCorrector : IDetailsVectorCorrector
    {
        private readonly ILinearSystemSolver _linearSystemSolver;

        public LinearEquationsBasedCorrector(ILinearSystemSolver linearSystemSolver)
        {
            if (linearSystemSolver == null)
                throw new ArgumentNullException(nameof(linearSystemSolver));

            _linearSystemSolver = linearSystemSolver;
        }

        /// <inheritdoc/>
        public FieldElementsMatrix CorrectDetailsVector(
            (FieldElementsMatrix hMatrix, FieldElementsMatrix gMatrix) iterationMatrices,
            FieldElementsMatrix approximationVector,
            FieldElementsMatrix detailsVector,
            int correctableComponentsCount,
            int requiredZerosCount
        )
        {
            if(approximationVector == null)
                throw new ArgumentNullException(nameof(approximationVector));
            if(approximationVector.ColumnsCount != 1)
                throw new ArgumentException($"{nameof(approximationVector)} must be a column vector");
            if(detailsVector == null)
                throw new ArgumentNullException(nameof(detailsVector));
            if (detailsVector.ColumnsCount != 1)
                throw new ArgumentException($"{nameof(detailsVector)} must be a column vector");
            if (approximationVector.RowsCount != detailsVector.RowsCount)
                throw new ArgumentException($"{nameof(approximationVector)} components count must be equal to {nameof(detailsVector)} components count");
            if(correctableComponentsCount < 0)
                throw new ArgumentException($"{nameof(correctableComponentsCount)} must not be negative");
            if(correctableComponentsCount > detailsVector.RowsCount)
                throw new ArgumentException($"{nameof(correctableComponentsCount)} must not be greater than details vector length");
            if(requiredZerosCount < 0)
                throw new ArgumentException($"{nameof(requiredZerosCount)} must not be negative");
            if (correctableComponentsCount < requiredZerosCount)
                throw new ArgumentException($"{nameof(correctableComponentsCount)} must not be less than {requiredZerosCount}");

            if (requiredZerosCount == 0)
                return detailsVector;

            var (hMatrix, gMatrix) = iterationMatrices;
            var equationsRowsRange = Enumerable.Range(hMatrix.RowsCount - requiredZerosCount, requiredZerosCount).ToArray();
            var valuesColumnsRange = Enumerable.Range(0, gMatrix.ColumnsCount - requiredZerosCount).ToArray();
            var variablesColumnsRange = Enumerable.Range(gMatrix.ColumnsCount - requiredZerosCount, requiredZerosCount).ToArray();

            var a = gMatrix.CreateSubmatrix(equationsRowsRange, variablesColumnsRange);

            FieldElementsMatrix detailsVectorPart = null;
            var valuesVector = hMatrix * approximationVector;
            if (valuesColumnsRange.Length > 0)
            {
                detailsVectorPart = detailsVector.CreateSubmatrixFromRows(valuesColumnsRange);
                valuesVector += gMatrix.CreateSubmatrixFromColumns(valuesColumnsRange) * detailsVectorPart;
            }
            var b = valuesVector.CreateSubmatrixFromRows(equationsRowsRange);

            var systemSolution = _linearSystemSolver.Solve(a, b);
            if(systemSolution.IsEmpty)
                throw new InvalidOperationException("Can't correct details vector");

            return FieldElementsMatrix.ColumnVector(
                (detailsVectorPart?.GetColumn(0) ?? new FieldElement[0])
                .Concat(systemSolution.VariablesValues.Select(x => x.InverseForAddition()))
                .ToArray()
            );
        }
    }
}