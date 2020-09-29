namespace AppliedAlgebra.GfAlgorithms.LinearSystemSolver
{
    /// <summary>
    /// Common options supported by all ILinearSystemSolver contract implementations
    /// </summary>
    public class SolverOptions
    {
        /// <summary>
        /// Maximum degree of parallelism if it is supported by solver
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; }

        public SolverOptions()
        {
            MaxDegreeOfParallelism = -1;
        }
    }
}