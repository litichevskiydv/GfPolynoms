namespace AppliedAlgebra.CodesResearchTools.Extensions
{
    using System;
    using System.Threading;

    public static class InterlockedExtensions
    {
        private static int UpdateLocation(ref int location, int value, Func<int, int, int> newValueProvider)
        {
            int originalValue, newValue;
            do
            {
                originalValue = location;
                newValue = newValueProvider(originalValue, value);
            } while (Interlocked.CompareExchange(ref location, newValue, originalValue) != originalValue);

            return originalValue;
        }

        public static int Min(ref int location, int value) => UpdateLocation(ref location, value, Math.Min);

        public static int Max(ref int location, int value) => UpdateLocation(ref location, value, Math.Max);
    }
}