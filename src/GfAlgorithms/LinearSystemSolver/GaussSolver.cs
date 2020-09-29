namespace AppliedAlgebra.GfAlgorithms.LinearSystemSolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using Matrices;

    /// <summary>
    /// Implementation of linear equations system solver contract via Gauss's approach
    /// </summary>
    public class GaussSolver : ILinearSystemSolver
    {
        private static FieldElement[] CalculateSolutionVector(
            FieldElementsMatrix extendedSystemMatrix,
            IReadOnlyList<int?> rowsForVariables,
            int? freeVariableIndex = null
        )
        {
            var field = extendedSystemMatrix.Field;
            var solution = Enumerable.Repeat(field.Zero(), rowsForVariables.Count - 1).ToArray();

            if(freeVariableIndex.HasValue)
                solution[freeVariableIndex.Value] = field.One();

            for (var i = 0; i < solution.Length; ++i)
                if (rowsForVariables[i].HasValue)
                {
                    var remainder = extendedSystemMatrix[rowsForVariables[i].Value, solution.Length];
                    if (freeVariableIndex.HasValue)
                        remainder -= extendedSystemMatrix[rowsForVariables[i].Value, freeVariableIndex.Value] * solution[freeVariableIndex.Value];
                    solution[i] = remainder / extendedSystemMatrix[rowsForVariables[i].Value, i];
                }

            return solution;
        }

        private static SystemSolution CalculateSolution(
            FieldElementsMatrix extendedSystemMatrix,
            IReadOnlyList<int?> rowsForVariables
        )
        {
            var solutionSystemBasis = new List<FieldElement[]>();
            for (var i = 0; i < rowsForVariables.Count - 1; i++)
                if (rowsForVariables[i].HasValue == false)
                    solutionSystemBasis.Add(CalculateSolutionVector(extendedSystemMatrix, rowsForVariables, i));

            return solutionSystemBasis.Count == 0
                ? SystemSolution.OneSolution(CalculateSolutionVector(extendedSystemMatrix, rowsForVariables))
                : SystemSolution.InfiniteSolution(solutionSystemBasis);
        }

        private static SystemSolution CheckSolution(
            FieldElementsMatrix a,
            FieldElementsMatrix b,
            SystemSolution solution
        )
        {
            var field = a.Field;
            var equationsCount = a.RowsCount;
            var variablesCount = solution.VariablesValues.Length;

            for (var i = 0; i < equationsCount; ++i)
            {
                var sum = field.Zero();
                for (var j = 0; j < variablesCount; ++j)
                    sum += solution.VariablesValues[j] * a[i, j];

                if (sum.Equals(b[i, 0]) == false)
                    return SystemSolution.EmptySolution();
            }

            return solution;
        }

        /// <inheritdoc />
        public SystemSolution Solve(FieldElementsMatrix a, FieldElementsMatrix b, SolverOptions options = null)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            if (a.RowsCount == 0)
                throw new ArgumentException("At least one equation must be presented");
            if (a.ColumnsCount == 0)
                throw new ArgumentException("At least one variable must be presented");

            var opts = options ?? new SolverOptions();

            var (_, rowsForVariables, extendedSystemMatrix) = a.AppendColumn(b)
                .DiagonalizeExtended(new DiagonalizationOptions {MaxDegreeOfParallelism = opts.MaxDegreeOfParallelism});
            var solution = CalculateSolution(extendedSystemMatrix, rowsForVariables);
            return CheckSolution(a, b, solution);
        }

        /// <inheritdoc />
        public SystemSolution Solve(FieldElement[,] a, FieldElement[] b, SolverOptions options = null) =>
            Solve(new FieldElementsMatrix(a), FieldElementsMatrix.ColumnVector(b), options);
    }
}