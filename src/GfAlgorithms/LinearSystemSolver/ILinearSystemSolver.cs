namespace AppliedAlgebra.GfAlgorithms.LinearSystemSolver
{
    using GfPolynoms;

    /// <summary>
    /// Contract for solver of linear equations system over finite fields
    /// </summary>
    public interface ILinearSystemSolver
    {
        /// <summary>
        /// Method for computing solution of linear equations system with matrix <paramref name="a"/> and constat terms vector <paramref name="b"/>
        /// </summary>
        /// <param name="a">System matrix</param>
        /// <param name="b">Constant terms vector</param>
        /// <returns>Solution of the system</returns>
        SystemSolution Solve(FieldElement[,] a, FieldElement[] b);
    }
}