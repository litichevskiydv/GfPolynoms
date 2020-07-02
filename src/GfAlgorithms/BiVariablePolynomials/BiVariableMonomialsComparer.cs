namespace AppliedAlgebra.GfAlgorithms.BiVariablePolynomials
{
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of bivariate monomial comparer 
    /// </summary>
    public class BiVariableMonomialsComparer : IComparer<(int xDegree, int yDegree)>
    {
        /// <summary>
        /// Weights of monomial variable's powers
        /// </summary>
        private readonly (int xWeight, int yWeight) _degreeWeight;

        /// <summary>
        /// Method for comparison one bivariate monomial <paramref name="a"/> to another bivariate monomial <paramref name="b"/>
        /// </summary>
        /// <param name="a">First compared</param>
        /// <param name="b">Second compared</param>
        /// <returns>Comparsion result</returns>
        public int Compare((int xDegree, int yDegree) a, (int xDegree, int yDegree) b)
        {
            var xDegreeWeight = a.xDegree*_degreeWeight.xWeight + a.yDegree*_degreeWeight.yWeight;
            var yDegreeWeight = b.xDegree * _degreeWeight.xWeight + b.yDegree * _degreeWeight.yWeight;

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
        public BiVariableMonomialsComparer((int xWeight, int yWeight) degreeWeight)
        {
            _degreeWeight = degreeWeight;
        }
    }
}