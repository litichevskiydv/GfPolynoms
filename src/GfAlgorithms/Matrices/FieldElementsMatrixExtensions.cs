namespace AppliedAlgebra.GfAlgorithms.Matrices
{
    using System;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public static class FieldElementsMatrixExtensions
    {
        /// <summary>
        /// Calculates determinant of the matrix <paramref name="matrix"/>
        /// <param name="matrix">Matrix whose determinant will be calculated</param>
        /// </summary>
        public static FieldElement CalculateDeterminant(this FieldElementsMatrix matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));
            if (matrix.RowsCount != matrix.ColumnsCount)
                throw new InvalidOperationException("Determinant can be calculated only for square matrices");

            var determinant = matrix.Field.One();
            var diagonalizationSummary = FieldElementsMatrix.DiagonalizeExtended(matrix);
            for (var i = 0; i < matrix.RowsCount; i++)
                determinant *= diagonalizationSummary.Result[i, i];

            return diagonalizationSummary.PermutationsCount % 2 == 0 ? determinant : determinant.InverseForAddition();
        }

        /// <summary>
        /// Calculates rank of the matrix <paramref name="matrix"/>
        /// <param name="matrix">Matrix whose rank will be calculated</param>
        /// </summary>
        public static int CalculateRank(this FieldElementsMatrix matrix) =>
            FieldElementsMatrix.DiagonalizeExtended(matrix).SelectedRowsByColumns.Count(x => x.HasValue);

        /// <summary>
        /// Creates submatrix of the matrix <paramref name="matrix"/>
        /// consisted of the rows <paramref name="includedRowsIndexes"/>
        /// </summary>
        /// <param name="matrix">Matrix whose rank will be created</param>
        /// <param name="includedRowsIndexes">Rows to be included in the submatrix</param>
        /// <returns></returns>
        public static FieldElementsMatrix CreateSubmatrix(this FieldElementsMatrix matrix, params int[] includedRowsIndexes)
        {
            if(matrix == null)
                throw new ArgumentNullException(nameof(matrix));
            if (includedRowsIndexes == null)
                throw new ArgumentNullException(nameof(includedRowsIndexes));
            if(includedRowsIndexes.Length == 0)
                throw new ArgumentException($"{nameof(includedRowsIndexes)} must contain at least one index");

            var orderedRowsIndexes = includedRowsIndexes.Distinct().OrderBy(x => x).ToArray();
            if(orderedRowsIndexes.Length != includedRowsIndexes.Length)
                throw new ArgumentException($"{nameof(includedRowsIndexes)} must not contain duplicate indices");
            if(orderedRowsIndexes[0] < 0 || orderedRowsIndexes[orderedRowsIndexes.Length - 1] >= matrix.RowsCount)
                throw new ArgumentException($"Indexes in {nameof(includedRowsIndexes)} must be consistent with source matrix size");

            return new FieldElementsMatrix(
                matrix.Field,
                orderedRowsIndexes.Length,
                matrix.ColumnsCount,
                (i, j) => new FieldElement(matrix[orderedRowsIndexes[i], j])
            );
        }
    }
}