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
        /// Validates indices <paramref name="indices"/> used for sumbatrix creation
        /// </summary>
        private static void ValidateIndices(int[] indices, int maxIndex)
        {
            if (indices == null)
                throw new ArgumentNullException(nameof(indices));
            if (indices.Length == 0)
                throw new ArgumentException($"{nameof(indices)} must contain at least one index");

            var orderedIndexes = indices.Distinct().OrderBy(x => x).ToArray();
            if (orderedIndexes.Length != indices.Length)
                throw new ArgumentException($"{nameof(indices)} must not contain duplicate indices");
            if (orderedIndexes[0] < 0 || orderedIndexes[orderedIndexes.Length - 1] > maxIndex)
                throw new ArgumentException($"Indexes in {nameof(indices)} must be consistent with source matrix size");
        }

        /// <summary>
        /// Creates submatrix of the matrix <paramref name="matrix"/>
        /// consisted of the rows <paramref name="includedRowsIndices"/>
        /// and columns <paramref name="includedColumnsIndices"/>
        /// </summary>
        /// <param name="matrix">Matrix whose rank will be created</param>
        /// <param name="includedRowsIndices">Rows to be included in the submatrix</param>
        /// <param name="includedColumnsIndices">Columns to be included in the submatrix</param>
        public static FieldElementsMatrix CreateSubmatrix(this FieldElementsMatrix matrix, int[] includedRowsIndices, int[] includedColumnsIndices)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));
            ValidateIndices(includedRowsIndices, matrix.RowsCount - 1);
            ValidateIndices(includedColumnsIndices, matrix.ColumnsCount - 1);

            return new FieldElementsMatrix(
                matrix.Field,
                includedRowsIndices.Length,
                includedColumnsIndices.Length,
                (i, j) => new FieldElement(matrix[includedRowsIndices[i], includedColumnsIndices[j]])
            );
        }

        /// <summary>
        /// Creates submatrix of the matrix <paramref name="matrix"/>
        /// consisted of the rows <paramref name="includedRowsIndices"/>
        /// </summary>
        /// <param name="matrix">Matrix whose rank will be created</param>
        /// <param name="includedRowsIndices">Rows to be included in the submatrix</param>
        public static FieldElementsMatrix CreateSubmatrixFromRows(this FieldElementsMatrix matrix, params int[] includedRowsIndices)
        {
            if(matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            return matrix.CreateSubmatrix(includedRowsIndices, Enumerable.Range(0, matrix.ColumnsCount).ToArray());
        }

        /// <summary>
        /// Creates submatrix of the matrix <paramref name="matrix"/>
        /// consisted of the columns <paramref name="includedColumnsIndices"/>
        /// </summary>
        /// <param name="matrix">Matrix whose rank will be created</param>
        /// <param name="includedColumnsIndices">Columns to be included in the submatrix</param>
        public static FieldElementsMatrix CreateSubmatrixFromColumns(this FieldElementsMatrix matrix, params int[] includedColumnsIndices)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            return matrix.CreateSubmatrix(Enumerable.Range(0, matrix.RowsCount).ToArray(), includedColumnsIndices);
        }
    }
}