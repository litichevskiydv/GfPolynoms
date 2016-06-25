namespace GfAlgorithms.PolynomialsGcdFinder
{
    using System;
    using GfPolynoms;

    public class GcdWithQuotients
    {
        public Polynomial Gcd { get; }

        public Polynomial[] Quotients { get; }

        public GcdWithQuotients(Polynomial gcd, Polynomial[] quotients)
        {
            if(gcd == null)
                throw new ArgumentNullException(nameof(gcd));
            if(quotients == null)
                throw new ArgumentNullException(nameof(quotients));

            Gcd = gcd;
            Quotients = quotients;
        }
    }
}