namespace AppliedAlgebra.GfAlgorithms.PolynomialsGcdFinder
{
    using System;
    using System.Collections.Generic;
    using GfPolynoms;

    /// <summary>
    /// Implementation of greatest common divisor finder contract
    /// </summary>
    public class RecursiveGcdFinder : IPolynomialsGcdFinder
    {
        /// <summary>
        /// Method for validation arguments of the algorithm
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        private static void ValidateArguments(Polynomial a, Polynomial b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));
        }

        /// <summary>
        /// Method for calculation greatest common divisor for polynomials <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <returns>Calculated greatest common divisor</returns>
        private static Polynomial GcdInternal(Polynomial a, Polynomial b)
        {
            return b.IsZero ? a : GcdInternal(b, a % b);
        }

        /// <summary>
        /// Method for calculation greatest common divisor for polynomials <paramref name="a"/> and <paramref name="b"/>
        /// </summary>
        /// <returns>Calculated greatest common divisor</returns>
        public Polynomial Gcd(Polynomial a, Polynomial b)
        {
            ValidateArguments(a, b);

            return GcdInternal(a, b);
        }

        /// <summary>
        /// Method for calculating greatest common divisor for polynomials <paramref name="a"/> and <paramref name="b"/> and produces list of quotients <paramref name="quotients"/>
        /// </summary>
        private static Polynomial GcdWithQuotientsInternal(Polynomial a, Polynomial b, ICollection<Polynomial> quotients)
        {
            if (b.IsZero)
                return a;

            var devisionResults = a.DivideExtended(b);
            quotients.Add(devisionResults.Quotient);
            return GcdWithQuotientsInternal(b, devisionResults.Remainder, quotients);
        }

        /// <summary>
        /// Method for calculating greatest common divisor for polynomials <paramref name="a"/> and <paramref name="b"/> and produces list of quotients
        /// </summary>
        public GcdWithQuotients GcdWithQuotients(Polynomial a, Polynomial b)
        {
            ValidateArguments(a, b);

            var quotients = new List<Polynomial>();
            return new GcdWithQuotients(GcdWithQuotientsInternal(a, b, quotients), quotients.ToArray());
        }

        /// <summary>
        /// Method for calculating greatest common divisor for polynomials <paramref name="a"/> and <paramref name="b"/> and coefficients x and y such that <paramref name="a"/>*x+<paramref name="b"/>*y=gcd(a,b)
        /// </summary>
        private static GcdExtendedResult GcdExtndedInternal(Polynomial a, Polynomial b)
        {
            if (b.IsZero)
                return new GcdExtendedResult(new Polynomial(a.Field, 1), new Polynomial(a.Field), a);

            var divisionResult = a.DivideExtended(b);
            var gcdExtendedResult = GcdExtndedInternal(b, divisionResult.Remainder);

            var yOld = gcdExtendedResult.Y;
            gcdExtendedResult.Y = gcdExtendedResult.X - divisionResult.Quotient*yOld;
            gcdExtendedResult.X = yOld;
            return gcdExtendedResult;
        }

        /// <summary>
        /// Method for calculating greatest common divisor for polynomials <paramref name="a"/> and <paramref name="b"/> and coefficients x and y such that <paramref name="a"/>*x+<paramref name="b"/>*y=gcd(a,b)
        /// </summary>
        public GcdExtendedResult GcdExtended(Polynomial a, Polynomial b)
        {
            ValidateArguments(a, b);

            return GcdExtndedInternal(a, b);
        }
    }
}