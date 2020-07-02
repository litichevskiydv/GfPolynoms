namespace AppliedAlgebra.GfAlgorithms.BiVariablePolynomials
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of bivariate monomial comparer 
    /// </summary>
    public class BiVariableMonomialsComparer : IComparer<(int xDegree, int yDegree)>
    {
        /// <summary>
        /// Weights of monomial variable's powers
        /// </summary>
        private readonly Tuple<int, int> _degreeWeight;

        /// <summary>
        /// Method for comparison one bivariate monomial <paramref name="a"/> to another bivariate monomial <paramref name="b"/>
        /// </summary>
        /// <param name="a">First compared</param>
        /// <param name="b">Second compared</param>
        /// <returns>Comparsion result</returns>
        public int Compare((int xDegree, int yDegree) a, (int xDegree, int yDegree) b)
        {
            var xDegreeWeight = a.xDegree*_degreeWeight.Item1 + a.yDegree*_degreeWeight.Item2;
            var yDegreeWeight = b.xDegree * _degreeWeight.Item1 + b.yDegree * _degreeWeight.Item2;

            if (xDegreeWeight < yDegreeWeight)
                return -1;
            if (xDegreeWeight > yDegreeWeight)
                return 1;

            if (a.xDegree < b.xDegree)
                return -1;
            if (a.xDegree > b.xDegree)
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