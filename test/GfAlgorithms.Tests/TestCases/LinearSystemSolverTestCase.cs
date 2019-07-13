namespace AppliedAlgebra.GfAlgorithms.Tests.TestCases
{
    using GfPolynoms;
    using LinearSystemSolver;

    public class LinearSystemSolverTestCase
    {
        public FieldElement[,] A { get; set; }

        public FieldElement[] B { get; set; }

        public SystemSolution Expected { get; set; }
    }
}