namespace RsListDecoding.LinearSystemSolver
{
    using GfPolynoms;

    public interface ILinearSystemSolver
    {
        SystemSolution Solve(FieldElement[,] a, FieldElement[] b);
    }
}