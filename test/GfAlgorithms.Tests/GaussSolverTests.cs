namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using LinearSystemSolver;
    using TestCases;
    using Xunit;

    public class GaussSolverTests
    {
        private readonly ILinearSystemSolver _gaussSolver;

        [UsedImplicitly]
        public static readonly TheoryData<LinearSystemSolverTestCase> EmptySolutionTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<LinearSystemSolverTestCase> OneSolutionTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<LinearSystemSolverTestCase> InfiniteSolutionTestsData;

        private static LinearSystemSolverTestCase PrepareTestCase(GaloisField field, int[,] a, IEnumerable<int> b, SystemSolution expectedSolution)
        {
            var matrix = new FieldElement[a.GetLength(0), a.GetLength(1)];
            for (var i = 0; i < a.GetLength(0); i++)
            for (var j = 0; j < a.GetLength(1); j++)
                matrix[i, j] = new FieldElement(field, a[i, j]);

            var remainders = b.Select(field.CreateElement).ToArray();

            return new LinearSystemSolverTestCase {A = matrix, B = remainders, Expected = expectedSolution};
        }

        private static LinearSystemSolverTestCase PrepareTestCaseForEmptySolution(
            GaloisField field,
            int[,] a,
            IEnumerable<int> b
        ) =>
            PrepareTestCase(field, a, b, SystemSolution.EmptySolution());

        private static LinearSystemSolverTestCase PrepareTestCaseForOneSolution(
            GaloisField field,
            int[,] a,
            IEnumerable<int> b,
            IEnumerable<int> variablesValues
        ) =>
            PrepareTestCase(field, a, b, SystemSolution.OneSolution(variablesValues.Select(field.CreateElement).ToArray()));

        private static LinearSystemSolverTestCase PrepareTestCaseForInfiniteSolution(
            GaloisField field,
            int[,] a,
            IEnumerable<int> b,
            params int[][] solutionSystemBasis
        ) =>
            PrepareTestCase(field, a, b,
                SystemSolution.InfiniteSolution(
                    solutionSystemBasis.Select(vector => vector.Select(field.CreateElement).ToArray()).ToArray()
                )
            );

        static GaussSolverTests()
        {
            var gf27 = GaloisField.Create(27, new[] {2, 2, 0, 1});
            OneSolutionTestsData
                = new TheoryData<LinearSystemSolverTestCase>
                  {
                      PrepareTestCaseForOneSolution(
                          gf27,
                          new[,] {{1, 1, 1}, {1, 2, 1}, {1, 1, 2}},
                          new[] {3, 5, 6},
                          new[] {1, 2, 3}
                      ),
                      PrepareTestCaseForOneSolution(
                          gf27,
                          new[,] {{1, 1, 1}, {1, 2, 1}, {1, 1, 2}, {2, 2, 2}},
                          new[] {3, 5, 6, 6},
                          new[] {1, 2, 3}
                      )
                  };
            EmptySolutionTestsData
                = new TheoryData<LinearSystemSolverTestCase>
                  {
                      PrepareTestCaseForEmptySolution(
                          gf27,
                          new[,] {{1, 1, 1}, {1, 2, 1}, {1, 1, 2}, {2, 2, 2}},
                          new[] {3, 5, 6, 5}
                      )
                  };
            InfiniteSolutionTestsData
                = new TheoryData<LinearSystemSolverTestCase>
                  {
                      PrepareTestCaseForInfiniteSolution(
                          gf27,
                          new[,] {{1, 1, 1}, {1, 2, 1}},
                          new[] {0, 2},
                          new[] {0, 2, 1}
                      ),
                      PrepareTestCaseForInfiniteSolution(
                          gf27,
                          new[,] {{1, 1, 1}, {1, 2, 1}},
                          new[] {0, 0},
                          new[] {2, 0, 1}
                      ),
                      PrepareTestCaseForInfiniteSolution(
                          gf27,
                          new[,] {{1, 1, 1}},
                          new[] {2},
                          new[] {1, 1, 0},
                          new[] {1, 0, 1}
                      )
                  };
        }

        public GaussSolverTests()
        {
            _gaussSolver = new GaussSolver();
        }

        [Theory]
        [MemberData(nameof(EmptySolutionTestsData))]
        public void ShouldFindEmptySolution(LinearSystemSolverTestCase testCase)
        {
            // When
            var actualSolution = _gaussSolver.Solve(testCase.A, testCase.B);

            // Then
            Assert.Equal(testCase.Expected, actualSolution);
        }

        [Theory]
        [MemberData(nameof(OneSolutionTestsData))]
        public void ShouldFindOneSolution(LinearSystemSolverTestCase testCase)
        {
            // When
            var actualSolution = _gaussSolver.Solve(testCase.A, testCase.B);

            // Then
            Assert.Equal(testCase.Expected, actualSolution);
        }

        [Theory]
        [MemberData(nameof(InfiniteSolutionTestsData))]
        public void ShouldFindInfiniteSolution(LinearSystemSolverTestCase testCase)
        {
            // When
            var actualSolution = _gaussSolver.Solve(testCase.A, testCase.B);

            // Then
            Assert.Equal(testCase.Expected, actualSolution);
        }
    }
}