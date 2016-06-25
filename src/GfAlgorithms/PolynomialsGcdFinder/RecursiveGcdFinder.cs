namespace GfAlgorithms.PolynomialsGcdFinder
{
    using System;
    using System.Collections.Generic;
    using GfPolynoms;

    public class RecursiveGcdFinder : IPolynomialsGcdFinder
    {
        private static void ValidateArguments(Polynomial a, Polynomial b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));
        }

        private static Polynomial GcdInternal(Polynomial a, Polynomial b)
        {
            return b.IsZero ? a : GcdInternal(b, a % b);
        }

        public Polynomial Gcd(Polynomial a, Polynomial b)
        {
            ValidateArguments(a, b);

            return GcdInternal(a, b);
        }

        private static Polynomial GcdWithQuotientsInternal(Polynomial a, Polynomial b, ICollection<Polynomial> quotients)
        {
            if (b.IsZero)
                return a;

            var devisionResults = a.DivideExtended(b);
            quotients.Add(devisionResults.Quotient);
            return GcdWithQuotientsInternal(b, devisionResults.Remainder, quotients);
        }

        public GcdWithQuotients GcdWithQuotients(Polynomial a, Polynomial b)
        {
            ValidateArguments(a, b);

            var quotients = new List<Polynomial>();
            return new GcdWithQuotients(GcdWithQuotientsInternal(a, b, quotients), quotients.ToArray());
        }

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

        public GcdExtendedResult GcdExtended(Polynomial a, Polynomial b)
        {
            ValidateArguments(a, b);

            return GcdExtndedInternal(a, b);
        }
    }
}