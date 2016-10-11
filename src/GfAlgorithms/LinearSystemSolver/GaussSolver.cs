namespace GfAlgorithms.LinearSystemSolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public class GaussSolver : ILinearSystemSolver
    {
        private static void SwapElements(FieldElement[,] a, int firstRowIndex, int secondRowsIndex, int columnIndex)
        {
            var tmp = a[firstRowIndex, columnIndex];
            a[firstRowIndex, columnIndex] = a[secondRowsIndex, columnIndex];
            a[secondRowsIndex, columnIndex] = tmp;
        }

        private static void SwapElements(IList<FieldElement> b, int firstRowIndex, int secondRowsIndex)
        {
            var tmp = b[firstRowIndex];
            b[firstRowIndex] = b[secondRowsIndex];
            b[secondRowsIndex] = tmp;
        }

        private static int[] TransformSystemToTriangularView(FieldElement[,] a, FieldElement[] b)
        {
            var rowsCount = a.GetLength(0);
            var columnsCount = a.GetLength(1);

            var where = Enumerable.Repeat(-1, columnsCount).ToArray();
            for (int col = 0, row = 0; col < columnsCount && row < rowsCount; ++col)
            {
                var selectedRow = row;
                for (var i = row; i < rowsCount; ++i)
                    if (a[i, col].Representation > a[selectedRow, col].Representation)
                        selectedRow = i;
                if (a[selectedRow, col].Representation == 0)
                    continue;

                for (var j = col; j < columnsCount; ++j)
                    SwapElements(a, selectedRow, row, j);
                SwapElements(b, selectedRow, row);
                where[col] = row;

                for (var i = 0; i < rowsCount; ++i)
                    if (i != row)
                    {
                        var c = a[i, col] / a[row, col];
                        for (var j = col; j < columnsCount; ++j)
                            a[i, j] -= a[row, j] * c;
                        b[i] -= b[row] * c;
                    }
                ++row;
            }
            return where;
        }

        private static FieldElement[] CalculateSolution(FieldElement[,] a, IReadOnlyList<FieldElement> b,
            IReadOnlyList<int> rowsForVariables)
        {
            var field = b[0].Field;
            var solution = Enumerable.Repeat(field.Zero(), rowsForVariables.Count).ToArray();
            int? freeVariableIndex = null;

            for (var i = 0; i < solution.Length; i++)
                if (rowsForVariables[i] == -1)
                {
                    solution[i] = field.One();
                    freeVariableIndex = i;
                    break;
                }

            for (var i = 0; i < solution.Length; ++i)
                if (rowsForVariables[i] != -1)
                {
                    var remaider = b[rowsForVariables[i]];
                    if (freeVariableIndex.HasValue)
                        remaider -= a[rowsForVariables[i], freeVariableIndex.Value]*solution[freeVariableIndex.Value];
                    solution[i] = remaider / a[rowsForVariables[i], i];
                }
                    
            return solution;
        }

        private static SystemSolution CheckSolution(FieldElement[,] a, IReadOnlyList<FieldElement> b,
            IReadOnlyList<int> rowsForVariables, FieldElement[] solution)
        {
            var field = b[0].Field;
            var equationsCount = a.GetLength(0);
            var variablesCount = solution.Length;

            for (var i = 0; i < equationsCount; ++i)
            {
                var sum = field.Zero();
                for (var j = 0; j < variablesCount; ++j)
                    sum += solution[j] * a[i, j];
                if ((sum - b[i]).Representation != 0)
                    return SystemSolution.EmptySolution();
            }

            return solution.Where((t, i) => rowsForVariables[i] == -1).Any()
                ? SystemSolution.InfiniteSolution(solution)
                : SystemSolution.OneSolution(solution);
        }

        public SystemSolution Solve(FieldElement[,] a, FieldElement[] b)
        {
            if (a == null || a.GetLength(0) == 0 || a.GetLength(0) == 1)
                throw new ArgumentException(nameof(a));

            if (b == null || a.GetLength(0) != b.Length)
                throw new ArgumentException(nameof(b));

            var rowsForVariables = TransformSystemToTriangularView(a, b);
            var solution = CalculateSolution(a, b, rowsForVariables);
            return CheckSolution(a, b, rowsForVariables, solution);
        }
    }
}