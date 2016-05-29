namespace RsListDecoding.LinearSystemSolver
{
    using GfPolynoms;

    public class SystemSolution
    {
        public int SolutionsCount { get; }

        public FieldElement[] Solution { get; }

        public SystemSolution(int solutionsCount, FieldElement[] solution)
        {
            SolutionsCount = solutionsCount;
            Solution = solution;
        }
    }
}