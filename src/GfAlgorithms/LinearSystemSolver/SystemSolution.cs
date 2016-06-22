namespace GfAlgorithms.LinearSystemSolver
{
    using System;
    using System.Linq;
    using GfPolynoms;

    public class SystemSolution
    {
        private readonly int _solutionsCount;

        public FieldElement[] Solution { get; }

        private SystemSolution(int solutionsCount, FieldElement[] solution)
        {
            if ((solution == null || solution.Length == 0) && solutionsCount != 0)
                throw new ArgumentException(nameof(solution));

            _solutionsCount = solutionsCount;
            Solution = solution;
        }

        protected bool Equals(SystemSolution other)
        {
            return _solutionsCount == other._solutionsCount
                   && (IsEmpty || Solution.SequenceEqual(other.Solution));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SystemSolution) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_solutionsCount*397) ^ (!IsEmpty ? Solution.GetHashCode() : 0);
            }
        }

        public bool IsEmpty => _solutionsCount == 0;
        public bool IsCorrect => _solutionsCount == 1;
        public bool IsInfinite => _solutionsCount == int.MaxValue;

        public static SystemSolution EmptySolution()
        {
            return new SystemSolution(0, null);
        }

        public static SystemSolution OneSolution(FieldElement[] solution)
        {
            return new SystemSolution(1, solution);
        }

        public static SystemSolution InfiniteSolution(FieldElement[] solution)
        {
            return new SystemSolution(int.MaxValue, solution);
        }
    }
}