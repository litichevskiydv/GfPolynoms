namespace AppliedAlgebra.CodesResearchTools.Extensions
{
    using System;
    using System.Threading;

    public static class InterlockedExtensions
    {
        public static int Min(ref int location, int value)
        {
            int originalValue, minimalValue;
            do
            {
                originalValue = location;
                minimalValue = Math.Min(originalValue, value);
            } while (Interlocked.CompareExchange(ref location, minimalValue, originalValue) != originalValue);

            return originalValue;
        }
    }
}