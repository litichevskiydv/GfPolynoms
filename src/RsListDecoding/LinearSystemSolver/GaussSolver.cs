namespace RsListDecoding.LinearSystemSolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;

    public class GaussSolver : ILinearSystemSolver
    {
        private static void Swap(ref FieldElement a, ref FieldElement b)
        {
            var temp = a;
            a = b;
            b = temp;
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
                    Swap(ref a[selectedRow, j], ref a[row, j]);
                Swap(ref b[selectedRow], ref b[row]);
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