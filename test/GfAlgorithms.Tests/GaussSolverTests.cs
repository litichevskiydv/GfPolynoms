namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.Extensions;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using LinearSystemSolver;
    using Xunit;

    public class GaussSolverTests
    {
        public class GaussSolverTestCase
        {
            public FieldElement[,] A { get; set; }

            public FieldElement[] B { get; set; }

            public SystemSolution Expected { get; set; }
        }

        private readonly ILinearSystemSolver _gaussSolver;

        [UsedImplicitly]
        public static readonly TheoryData<GaussSolverTestCase> EmptySolutionTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<GaussSolverTestCase> OneSolutionTestsData;
        [UsedImplicitly]
        public static readonly TheoryData<GaussSolverTestCase> InfiniteSolutionTestsData;

        private static GaussSolverTestCase PrepareTestCase(GaloisField field, int[,] a, IEnumerable<int> b, SystemSolution expectedSolution)
        {
            var matrix = new FieldElement[a.GetLength(0), a.GetLength(1)];
            for (var i = 0; i < a.GetLength(0); i++)
            for (var j = 0; j < a.GetLength(1); j++)
                matrix[i, j] = new FieldElement(field, a[i, j]);

            var remainders = b.Select(field.CreateElement).ToArray();

            return new GaussSolverTestCase {A = matrix, B = remainders, Expected = expectedSolution};
        }

        private static GaussSolverTestCase PrepareTestCaseForEmptySolution(GaloisField field, int[,] a, IEnumerable<int> b) =>
            PrepareTestCase(field, a, b, SystemSolution.EmptySolution());

        private static GaussSolverTestCase PrepareTestCaseForOneSolution(GaloisField field, int[,] a, IEnumerable<int> b, IEnumerable<int> variablesValues) =>
            PrepareTestCase(field, a, b, SystemSolution.OneSolution(variablesValues.Select(field.CreateElement).ToArray()));

        private static GaussSolverTestCase PrepareTestCaseForInfiniteSolution(GaloisField field, int[,] a, IEnumerable<int> b, IEnumerable<int> variablesValues) =>
            PrepareTestCase(field, a, b, SystemSolution.InfiniteSolution(variablesValues.Select(field.CreateElement).ToArray()));

        static GaussSolverTests()
        {
            var gf27 = new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1));
            OneSolutionTestsData
                = new TheoryData<GaussSolverTestCase>
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
                = new TheoryData<GaussSolverTestCase>
                  {
                      PrepareTestCaseForEmptySolution(
                          gf27,
                          new[,] {{1, 1, 1}, {1, 2, 1}, {1, 1, 2}, {2, 2, 2}},
                          new[] {3, 5, 6, 5}
                      )
                  };
            InfiniteSolutionTestsData
                = new TheoryData<GaussSolverTestCase>
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
                      )
                  };
        }

        public GaussSolverTests()
        {
            _gaussSolver = new GaussSolver();
        }

        [Theory]
        [MemberData(nameof(EmptySolutionTestsData))]
        public void ShouldFindEmptySolution(GaussSolverTestCase testCase)
        {
            // When
            var actualSolution = _gaussSolver.Solve(testCase.A, testCase.B);

            // Then
            Assert.Equal(testCase.Expected, actualSolution);
        }

        [Theory]
        [MemberData(nameof(OneSolutionTestsData))]
        public void ShouldFindOneSolution(GaussSolverTestCase testCase)
        {
            // When
            var actualSolution = _gaussSolver.Solve(testCase.A, testCase.B);

            // Then
            Assert.Equal(testCase.Expected, actualSolution);
        }

        [Theory]
        [MemberData(nameof(InfiniteSolutionTestsData))]
        public void ShouldFindInfiniteSolution(GaussSolverTestCase testCase)
        {
            // When
            var actualSolution = _gaussSolver.Solve(testCase.A, testCase.B);

            // Then
            Assert.Equal(testCase.Expected, actualSolution);
        }
    }
}