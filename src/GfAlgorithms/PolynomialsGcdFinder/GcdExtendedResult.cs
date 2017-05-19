namespace AppliedAlgebra.GfAlgorithms.PolynomialsGcdFinder
{
    using System;
    using GfPolynoms;

    /// <summary>
    /// Class for storing the greatest common divisor coefficients x and y such that a*x+b*y=gcd(a,b)
    /// </summary>
    public class GcdExtendedResult
    {
        public Polynomial X { get; set; }

        public Polynomial Y { get; set; }

        public Polynomial Gcd { get; }

        /// <summary>
        /// Constructor for creating oject for storing greatest common divisor <paramref name="gcd"/> and coefficients x and y such that a*<paramref name="x"/>+b*<paramref name="y"/> = <paramref name="gcd"/>
        /// </summary>
        public GcdExtendedResult(Polynomial x, Polynomial y, Polynomial gcd)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));
            if (gcd == null)
                throw new ArgumentNullException(nameof(gcd));

            X = x;
            Y = y;
            Gcd = gcd;
        }
    }
}