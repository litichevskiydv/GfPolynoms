namespace AppliedAlgebra.GfAlgorithms.LinearSystemSolver
{
    using GfPolynoms;
    using Matrices;

    /// <summary>
    /// Contract for solver of linear equations system over finite fields
    /// </summary>
    public interface ILinearSystemSolver
    {
        /// <summary>
        /// Method for computing solution of linear equations system with matrix <paramref name="a"/> and constant terms vector <paramref name="b"/>
        /// </summary>
        /// <param name="a">System matrix</param>
        /// <param name="b">Constant terms vector</param>
        /// <param name="options">Solver options</param>
        /// <returns>Solution of the system</returns>
        SystemSolution Solve(FieldElement[,] a, FieldElement[] b, SolverOptions options = null);

        /// <summary>
        /// Method for computing solution of linear equations system with matrix <paramref name="a"/> and constant terms vector <paramref name="b"/>
        /// </summary>
        /// <param name="a">System matrix</param>
        /// <param name="b">Constant terms vector</param>
        /// <param name="options">Solver options</param>
        /// <returns>Solution of the system</returns>
        SystemSolution Solve(FieldElementsMatrix a, FieldElementsMatrix b, SolverOptions options = null);
    }
}