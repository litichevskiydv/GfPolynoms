namespace AppliedAlgebra.GfAlgorithms.PolynomialsGcdFinder
{
    using System;
    using GfPolynoms;

    public class GcdExtendedResult
    {
        public Polynomial X { get; set; }

        public Polynomial Y { get; set; }

        public Polynomial Gcd { get; }

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