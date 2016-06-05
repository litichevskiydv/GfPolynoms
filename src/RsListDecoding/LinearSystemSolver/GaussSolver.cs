namespace RsListDecoding.LinearSystemSolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;

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
            var solution = Enumerable.Repeat(new FieldElement(b[0].Field, 0), rowsForVariables.Count).ToArray();
            for (var j = 0; j < solution.Length; ++j)
                if (rowsForVariables[j] != -1)
                    solution[j] = b[rowsForVariables[j]]/a[rowsForVariables[j], j];
            return solution;
        }

        private static SystemSolution CheckSolution(FieldElement[,] a, IReadOnlyList<FieldElement> b,
            IReadOnlyList<int> rowsForVariables, FieldElement[] solution)
        {
            var equationsCount = a.GetLength(0);
            var variablesCount = solution.Length;

            for (var i = 0; i < equationsCount; ++i)
            {
                var sum = new FieldElement(b[0].Field, 0);
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