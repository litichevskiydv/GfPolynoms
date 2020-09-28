namespace AppliedAlgebra.GfAlgorithms.Matrices
{
    using System;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public static class FieldElementsMatrixExtensions
    {
        /// <summary>
        /// Returns matrix <paramref name="matrix"/> row number <paramref name="rowNumber"/>
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <param name="rowNumber">Row number</param>
        /// <returns>Source matrix row</returns>
        public static FieldElement[] GetRow(this FieldElementsMatrix matrix, int rowNumber)
        {
            if(matrix == null)
                throw new ArgumentNullException(nameof(matrix));
            if(rowNumber < 0 || rowNumber >= matrix.RowsCount)
                throw new ArgumentException($"{nameof(rowNumber)} must be consistent with source matrix size");

            var matrixRow = new FieldElement[matrix.ColumnsCount];
            for (var j = 0; j < matrixRow.Length; j++)
                matrixRow[j] = matrix[rowNumber, j];

            return matrixRow;
        }

        /// <summary>
        /// Returns matrix <paramref name="matrix"/> column number <paramref name="columnNumber"/>
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <param name="columnNumber">Column number</param>
        /// <returns>Source matrix column</returns>
        public static FieldElement[] GetColumn(this FieldElementsMatrix matrix, int columnNumber)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));
            if (columnNumber < 0 || columnNumber >= matrix.ColumnsCount)
                throw new ArgumentException($"{nameof(columnNumber)} must be consistent with source matrix size");

            var matrixColumn = new FieldElement[matrix.RowsCount];
            for (var i = 0; i < matrixColumn.Length; i++)
                matrixColumn[i] = matrix[i, columnNumber];

            return matrixColumn;
        }

        /// <summary>
        /// Appends new column <paramref name="column"/> to the matrix <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <param name="column">Column to append</param>
        /// <returns>Extended matrix</returns>
        public static FieldElementsMatrix AppendColumn(this FieldElementsMatrix matrix, FieldElementsMatrix column)
        {
            if(matrix == null)
                throw new ArgumentNullException(nameof(matrix));
            if(column == null)
                throw new ArgumentNullException(nameof(column));

            if(column.ColumnsCount != 1 || matrix.RowsCount != column.RowsCount)
                throw new ArgumentException($"Dimensions of the {column} must be correlated with dimensions of the {matrix}");


            return new FieldElementsMatrix(
                matrix.Field,
                matrix.RowsCount,
                matrix.ColumnsCount + 1,
                (i, j) => j == matrix.ColumnsCount ? column[i, 0] : matrix[i, j]
            );
        }

        /// <summary>
        /// Appends new column <paramref name="column"/> to the matrix <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <param name="column">Column to append</param>
        /// <returns>Extended matrix</returns>
        public static FieldElementsMatrix AppendColumn(this FieldElementsMatrix matrix, params FieldElement[] column) =>
            matrix.AppendColumn(FieldElementsMatrix.ColumnVector(column));

        /// <summary>
        /// Appends new column <paramref name="column"/> to the matrix <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <param name="column">Column to append</param>
        /// <returns>Extended matrix</returns>
        public static FieldElementsMatrix AppendColumn(this FieldElementsMatrix matrix, params int[] column)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            return matrix.AppendColumn(FieldElementsMatrix.ColumnVector(matrix.Field, column));
        }

        /// <summary>
        /// Appends new row <paramref name="row"/> to the matrix <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <param name="row">Row to append</param>
        /// <returns>Extended matrix</returns>
        public static FieldElementsMatrix AppendRow(this FieldElementsMatrix matrix, FieldElementsMatrix row)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            if (row.RowsCount != 1 || matrix.ColumnsCount != row.ColumnsCount)
                throw new ArgumentException($"Dimensions of the {row} must be correlated with dimensions of the {matrix}");

            return new FieldElementsMatrix(
                matrix.Field,
                matrix.RowsCount + 1,
                matrix.ColumnsCount,
                (i, j) => i == matrix.RowsCount ? row[0, j] : matrix[i, j]
            );
        }

        /// <summary>
        /// Appends new row <paramref name="row"/> to the matrix <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <param name="row">Row to append</param>
        /// <returns>Extended matrix</returns>
        public static FieldElementsMatrix AppendRow(this FieldElementsMatrix matrix, params FieldElement[] row) =>
            matrix.AppendRow(FieldElementsMatrix.RowVector(row));

        /// <summary>
        /// Appends new row <paramref name="row"/> to the matrix <paramref name="matrix"/>
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <param name="row">Row to append</param>
        /// <returns>Extended matrix</returns>
        public static FieldElementsMatrix AppendRow(this FieldElementsMatrix matrix, params int[] row)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            return matrix.AppendRow(FieldElementsMatrix.RowVector(matrix.Field, row));
        }


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