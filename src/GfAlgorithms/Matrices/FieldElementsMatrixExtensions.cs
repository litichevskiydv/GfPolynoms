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
    }
}