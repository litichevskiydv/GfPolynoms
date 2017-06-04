namespace AppliedAlgebra.GfAlgorithms.LinearSystemSolver
{
    using System;
    using System.Linq;
    using GfPolynoms;

    /// <summary>
    /// Class for linear equations system solution
    /// </summary>
    public class SystemSolution
    {
        /// <summary>
        /// System solutions count
        /// </summary>
        private readonly int _solutionsCount;
        /// <summary>
        /// Variables values
        /// </summary>
        public FieldElement[] VariablesValues { get; }

        /// <summary>
        /// Constructor for creating system solution
        /// </summary>
        /// <param name="solutionsCount">Solutions count</param>
        /// <param name="variablesValues">Variables values</param>
        private SystemSolution(int solutionsCount, FieldElement[] variablesValues)
        {
            if ((variablesValues == null || variablesValues.Length == 0) && solutionsCount != 0)
                throw new ArgumentException(nameof(variablesValues));

            _solutionsCount = solutionsCount;
            VariablesValues = variablesValues;
        }

        /// <summary>
        /// Method for checking the equality of the current solution to the <paramref name="other"/>
        /// </summary>
        /// <param name="other">Another solution</param>
        /// <returns>Checking result</returns>
        protected bool Equals(SystemSolution other)
        {
            return _solutionsCount == other._solutionsCount
                   && (IsEmpty || VariablesValues.SequenceEqual(other.VariablesValues));
        }

        /// <summary>
        /// Method for checking the equality of the current solution to the <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">Another object</param>
        /// <returns>Checking result</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SystemSolution) obj);
        }

        /// <summary>
        /// Method for calculation object hash
        /// </summary>
        /// <returns>Calculated hash</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (_solutionsCount*397) ^ (!IsEmpty ? VariablesValues.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Sign of empty solution
        /// </summary>
        public bool IsEmpty => _solutionsCount == 0;
        /// <summary>
        /// Sign of single solution
        /// </summary>
        public bool IsCorrect => _solutionsCount == 1;
        /// <summary>
        /// Sign of infinite solutions count
        /// </summary>
        public bool IsInfinite => _solutionsCount == int.MaxValue;

        /// <summary>
        /// Factory method for empty solution creation 
        /// </summary>
        public static SystemSolution EmptySolution()
        {
            return new SystemSolution(0, null);
        }

        /// <summary>
        /// Factory method for single solution creation 
        /// </summary>
        /// <param name="variablesValues">Variables values</param>
        public static SystemSolution OneSolution(FieldElement[] variablesValues)
        {
            return new SystemSolution(1, variablesValues);
        }

        /// <summary>
        /// Factory method for infinite solutions set creation
        /// </summary>
        /// <param name="variablesValues">One of possible values of variables</param>
        public static SystemSolution InfiniteSolution(FieldElement[] variablesValues)
        {
            return new SystemSolution(int.MaxValue, variablesValues);
        }
    }
}