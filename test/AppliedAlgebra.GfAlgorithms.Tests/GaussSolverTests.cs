namespace AppliedAlgebra.GfAlgorithms.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using GfPolynoms;
    using GfPolynoms.GaloisFields;
    using JetBrains.Annotations;
    using LinearSystemSolver;
    using Xunit;

    public class GaussSolverTests
    {
        private readonly ILinearSystemSolver _gausSolver;

        [UsedImplicitly]
        public static readonly IEnumerable<object[]> EmptySolutionTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> OneSolutionTestsData;
        [UsedImplicitly]
        public static readonly IEnumerable<object[]> InfiniteSolutionTestsData;

        private static object[] PrepareTestCaseForEmptySolution(GaloisField field, int[,] a, IEnumerable<int> b)
        {
            return PrepareTestCase(field, a, b, SystemSolution.EmptySolution());
        }

        private static object[] PrepareTestCaseForOneSolution(GaloisField field, int[,] a, IEnumerable<int> b, IEnumerable<int> variablesValues)
        {
            return PrepareTestCase(field, a, b,
                SystemSolution.OneSolution(variablesValues.Select(x => new FieldElement(field, x)).ToArray()));
        }

        private static object[] PrepareTestCaseForInfiniteSolution(GaloisField field, int[,] a, IEnumerable<int> b, IEnumerable<int> variablesValues)
        {
            return PrepareTestCase(field, a, b,
                SystemSolution.InfiniteSolution(variablesValues.Select(x => new FieldElement(field, x)).ToArray()));
        }

        private static object[] PrepareTestCase(GaloisField field, int[,] a, IEnumerable<int> b, SystemSolution expectedSolution)
        {
            var matrix = new FieldElement[a.GetLength(0), a.GetLength(1)];
            for (var i = 0; i < a.GetLength(0); i++)
                for (var j = 0; j < a.GetLength(1); j++)
                    matrix[i, j] = new FieldElement(field, a[i, j]);

            var remainders = b.Select(x => new FieldElement(field, x)).ToArray();

            return new object[] {matrix, remainders, expectedSolution};
        }

        static GaussSolverTests()
        {
            var gf27 = new PrimePowerOrderField(27, new Polynomial(new PrimeOrderField(3), 2, 2, 0, 1));
            OneSolutionTestsData = new[]
                                   {
                                       PrepareTestCaseForOneSolution(gf27, new[,] {{1, 1, 1}, {1, 2, 1}, {1, 1, 2}}, new[] {3, 5, 6},
                                           new[] {1, 2, 3}),
                                       PrepareTestCaseForOneSolution(gf27, new[,] {{1, 1, 1}, {1, 2, 1}, {1, 1, 2}, {2, 2, 2}},
                                           new[] {3, 5, 6, 6}, new[] {1, 2, 3})
                                   };
            EmptySolutionTestsData = new[]
                                     {
                                         PrepareTestCaseForEmptySolution(gf27, new[,] {{1, 1, 1}, {1, 2, 1}, {1, 1, 2}, {2, 2, 2}},
                                             new[] {3, 5, 6, 5})
                                     };
            InfiniteSolutionTestsData = new[]
                                        {
                                            PrepareTestCaseForInfiniteSolution(gf27, new[,] {{1, 1, 1}, {1, 2, 1}}, new[] {0, 2},
                                                new[] {0, 2, 1}),
                                            PrepareTestCaseForInfiniteSolution(gf27, new[,] {{1, 1, 1}, {1, 2, 1}}, new[] {0, 0},
                                                new[] {2, 0, 1}),
                                        };
        }

        public GaussSolverTests()
        {
            _gausSolver = new GaussSolver();
        }

        [Theory]
        [MemberData(nameof(EmptySolutionTestsData))]
        public void ShouldFindEmptySolution(FieldElement[,] a, FieldElement[] b, SystemSolution expectedSolution)
        {
            // When
            var actualSolution = _gausSolver.Solve(a, b);

            // Then
            Assert.Equal(expectedSolution, actualSolution);
        }

        [Theory]
        [MemberData(nameof(OneSolutionTestsData))]
        public void ShouldFindOneSolution(FieldElement[,] a, FieldElement[] b, SystemSolution expectedSolution)
        {
            // When
            var actualSolution = _gausSolver.Solve(a, b);

            // Then
            Assert.Equal(expectedSolution, actualSolution);
        }

        [Theory]
        [MemberData(nameof(InfiniteSolutionTestsData))]
        public void ShouldFindInfiniteSolution(FieldElement[,] a, FieldElement[] b, SystemSolution expectedSolution)
        {
            // When
            var actualSolution = _gausSolver.Solve(a, b);

            // Then
            Assert.Equal(expectedSolution, actualSolution);
        }
    }
}