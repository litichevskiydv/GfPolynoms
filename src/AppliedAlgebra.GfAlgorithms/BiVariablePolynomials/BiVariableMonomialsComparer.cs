namespace AppliedAlgebra.GfAlgorithms.BiVariablePolynomials
{
    using System;
    using System.Collections.Generic;

    public class BiVariableMonomialsComparer : IComparer<Tuple<int, int>>
    {
        private readonly Tuple<int, int> _degreeWeight;

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

        public BiVariableMonomialsComparer(Tuple<int, int> degreeWeight)
        {
            if (degreeWeight == null)
                throw new ArgumentNullException(nameof(degreeWeight));

            _degreeWeight = degreeWeight;
        }
    }
}