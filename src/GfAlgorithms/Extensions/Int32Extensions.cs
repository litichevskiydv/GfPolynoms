namespace AppliedAlgebra.GfAlgorithms.Extensions
{
    using System;

    public static class Int32Extensions
    {
        /// <summary>
        /// Raises a <paramref name="value"/> to the power of a <paramref name="degree"/>
        /// </summary>
        public static int Pow(this int value, int degree)
        {
            if(degree < 0)
                throw new ArgumentException($"{nameof(degree)} must be positive");

            var result = 1;
            while (degree > 0)
            {
                if ((degree & 1) == 1)
                    result *= value;

                value *= value;
                degree >>= 1;
            }

            return result;
        }
    }
}