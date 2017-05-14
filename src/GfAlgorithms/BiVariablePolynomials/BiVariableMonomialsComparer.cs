namespace AppliedAlgebra.GfAlgorithms.BiVariablePolynomials
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of bivariate monomial comparer 
    /// </summary>
    public class BiVariableMonomialsComparer : IComparer<Tuple<int, int>>
    {
        /// <summary>
        /// Weights of monomial variable's powers
        /// </summary>
        private readonly Tuple<int, int> _degreeWeight;

        /// <summary>
        /// Method for comparison one bivariate monomial <paramref name="x"/> to another bivariate monomial <paramref name="y"/>
        /// </summary>
        /// <param name="x">First compared</param>
        /// <param name="y">Second compared</param>
        /// <returns>Comparsion result</returns>
        public int Compare(Tuple<int, int> x, Tuple<int, int> y)
        {
            var xDegreeWeight = x.Item1*_degreeWeight.Item1 + x.Item2*_degreeWeight.Item2;
            var yDegreeWeight = y.Item1 * _degreeWeight.Item1 + y.Item2 * _degreeWeight.Item2;

            if (xDegreeWeight < yDegreeWeight)
                return -1;
            if (xDegreeWeight > yDegreeWeight)
                return 1;

            if (x.Item1 < y.Item1)
                return -1;
            if (x.Item1 > y.Item1)
                return 1;

            return 0;
        }

        /// <summary>
        /// Constructor for creating bivariate monomials comparer with degree weights <paramref name="degreeWeight"/>
        /// </summary>
        /// <param name="degreeWeight">Weights of monomial variable's powers</param>
        public BiVariableMonomialsComparer(Tuple<int, int> degreeWeight)
        {
            if (degreeWeight == null)
                throw new ArgumentNullException(nameof(degreeWeight));

            _degreeWeight = degreeWeight;
        }
    }
}