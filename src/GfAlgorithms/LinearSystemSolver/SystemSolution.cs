namespace AppliedAlgebra.GfAlgorithms.LinearSystemSolver
{
    using System;
    using System.Collections.Generic;
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
        /// Basis of the solution system
        /// </summary>
        public IReadOnlyList<FieldElement[]> SolutionSystemBasis { get; }

        /// <summary>
        /// Constructor for creating system solution
        /// </summary>
        /// <param name="solutionsCount">Solutions count</param>
        /// <param name="variablesValues">Variables values</param>
        /// <param name="solutionSystemBasis">Basis of the solution system</param>
        private SystemSolution(int solutionsCount, FieldElement[] variablesValues, IReadOnlyList<FieldElement[]> solutionSystemBasis)
        {
            _solutionsCount = solutionsCount;

            VariablesValues = variablesValues;
            SolutionSystemBasis = solutionSystemBasis;
        }

        /// <summary>
        /// Method for checking the equality of the current solution to the <paramref name="other"/>
        /// </summary>
        /// <param name="other">Another solution</param>
        /// <returns>Checking result</returns>
        protected bool Equals(SystemSolution other)
        {
            if (_solutionsCount != other._solutionsCount) 
                return false;

            if (IsCorrect)
                return VariablesValues.SequenceEqual(other.VariablesValues);

            if (IsInfinite)
                return SolutionSystemBasis.Count == other.SolutionSystemBasis.Count
                       && SolutionSystemBasis.All(
                           vector => other.SolutionSystemBasis.Any(
                               otherVector => otherVector.SequenceEqual(vector)
                           )
                       );

            return true;
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
            return new SystemSolution(0, null, null);
        }

        /// <summary>
        /// Factory method for single solution creation 
        /// </summary>
        /// <param name="variablesValues">Variables values</param>
        public static SystemSolution OneSolution(FieldElement[] variablesValues)
        {
            if(variablesValues == null)
                throw new ArgumentNullException(nameof(variablesValues));

            return new SystemSolution(1, variablesValues, new FieldElement[0][]);
        }

        /// <summary>
        /// Factory method for infinite solutions set creation
        /// </summary>
        /// <param name="solutionSystemBasis">Basis of the solution system</param>
        public static SystemSolution InfiniteSolution(IReadOnlyList<FieldElement[]> solutionSystemBasis)
        {
            if (solutionSystemBasis == null)
                throw new ArgumentNullException(nameof(solutionSystemBasis));
            if (solutionSystemBasis.Count == 0)
                throw new ArgumentException($"{nameof(solutionSystemBasis)} must not be empty");

            return new SystemSolution(int.MaxValue, solutionSystemBasis[0], solutionSystemBasis);
        }
    }
}