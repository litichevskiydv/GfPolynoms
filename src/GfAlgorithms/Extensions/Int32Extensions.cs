namespace AppliedAlgebra.GfAlgorithms.Extensions
{
    public static class Int32Extensions
    {
        /// <summary>
        /// Raises a <paramref name="value"/> to the power of a <paramref name="degree"/>
        /// </summary>
        public static int Pow(this int value, int degree)
        {
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